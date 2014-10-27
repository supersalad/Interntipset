using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;

namespace inti2008.Data
{
    public class UserManagement : IntiManagementBase
    {
        public UserManagement(string connectionString, SessionProperties sessionProperties) : base(connectionString, sessionProperties)
        {
        }

        public void RegisterUser(string firstName, string lastName, string email, string password)
        {
            if (String.IsNullOrEmpty(firstName))
                throw new IntiGeneralException("Du måste ange ett förnamn");

            if (String.IsNullOrEmpty(lastName))
                throw new IntiGeneralException("Du måste ange ett efternamn");

            if (String.IsNullOrEmpty(email))
                throw new IntiGeneralException("Du måste ange en epostadress");

            if (String.IsNullOrEmpty(password))
                throw new IntiGeneralException("Du måste ange ett lösenord");

            using (var db = new IntiDataContext(_connectionString))
            {
                //username/email must be unique
                var users = from u in db.Sys_User
                            where u.UserName.ToLower() == email.ToLower()
                            select u;

                if (users.ToList().Count > 0)
                    throw new IntiGeneralException("Det finns redan en användare med den epostadressen");


                var user = new Sys_User { FirstName = firstName, LastName = lastName, UserName = email, Password = password };

                db.Sys_User.InsertOnSubmit(user);

                //default permissions
                var defaultPermissions = from p in db.Sys_Permission
                                         where p.Name == "USER" || p.Name == "USER_PVTTOUR"
                                         select p;

                foreach (var perm in defaultPermissions.ToList())
                {
                    var userPerm = new Sys_UserPermission();
                    userPerm.UserGUID = user.GUID;
                    userPerm.PermissionGUID = perm.GUID;
                    db.Sys_UserPermission.InsertOnSubmit(userPerm);
                }
                
                db.SubmitChanges();    
            }

        }


        public bool ValidateUser(string email, string password)
        {
            if (String.IsNullOrEmpty(email))
                throw new IntiGeneralException("Du måste ange en epostadress");

            if (String.IsNullOrEmpty(password))
                throw new IntiGeneralException("Du måste ange ett lösenord");

            using (var db = new IntiDataContext(_connectionString))
            {
                var users = from u in db.Sys_User
                            where u.UserName == email
                            select u;

                if (users.Count() != 1)
                {
                    //todo: log failed sign in attempt
                    throw new IntiGeneralException("Inloggningen misslyckades");
                }

                var user = users.ToList()[0];

                if (!user.Password.Equals(password))
                {
                    //todo: log failed sign in attempt
                    throw new IntiGeneralException("Inloggningen misslyckades");
                }
            }

            return true;
        }

        public IList<Sys_Permission> GetUserPermissions(string email)
        {
            using (var db = new IntiDataContext(_connectionString))
            {
                var permissions = from perm in db.Sys_UserPermission
                                  where perm.Sys_User.UserName == email
                                  select perm.Sys_Permission;

                return permissions.ToList();

            }
        }

        public IList GetPermissionsToEdit(Guid userGuid)
        {
            using (var db= new IntiDataContext(_connectionString))
            {
                var permissionProjection = from perm in db.Sys_Permission
                                           select new
                                                      {
                                                          perm.GUID,
                                                          perm.Name,
                                                          perm.Description,
                                                          HasPermission =
                                               (perm.Sys_UserPermission.Where(up => up.UserGUID == userGuid).Count() > 0)
                                                      };

                return permissionProjection.ToList();
            }
        }

        public string ForgotPassword(string email)
        {
            if (String.IsNullOrEmpty(email))
                throw new IntiGeneralException("Du måste ange en epostadress");

            using (var db = new IntiDataContext(_connectionString))
            {
                var users = from u in db.Sys_User
                            where u.UserName == email
                            select u;

                if (users.Count() != 1)
                {
                    throw new IntiGeneralException("Hittar ingen användare med den epostadressen");
                }

                var user = users.ToList()[0];

                user.Password = GetRandomPassword();
                db.SubmitChanges();

                //send mail
                MailAndLog.SendMessage(Parameters.Instance.ForgotPasswordSubject,
                    String.Format(Parameters.Instance.ForgotPasswordBody, user.Password),
                    Parameters.Instance.MailSender, email);

                return user.Password;

            }
        }

        private string GetRandomPassword()
        {
            var rnd = new Random(DateTime.Now.Millisecond);

            var sOut = String.Empty;
            var utfCode = rnd.Next(48, 90);
            for (var i = 0; i < 7; i++)
            {
                //avoid non-alphanumerics
                utfCode = rnd.Next(48, 90);
                while (utfCode > 57 && utfCode < 65)
                {
                    utfCode = rnd.Next(48, 90);
                }
                sOut += char.ConvertFromUtf32(utfCode);
            }

            return sOut;
        }

        private class Crypto
        {
            public static string Encrypt(string stringToEncrypt)
            {
                return (new MD5CryptoServiceProvider().ComputeHash(stringToEncrypt.ToByteArray())).ToString();
            }

            public static string Decrypt(string stringToDecrypt)
            {
                return (new MD5CryptoServiceProvider().ComputeHash(stringToDecrypt.ToByteArray())).ToString();
            }
        }

        public Sys_User GetUserByName(string name)
        {
            using (var db = new IntiDataContext(_connectionString))
            {


                var userQ = from u in db.Sys_User
                            where u.UserName == name
                            select u;

                return userQ.ToList()[0];


            }

        }

        public Sys_User GetUserByGuid(Guid userGuid)
        {
            using (var db = new IntiDataContext(_connectionString))
            {
                var userQ = from u in db.Sys_User
                            where u.GUID == userGuid
                            select u;

                return userQ.ToList()[0];

            }
        }

        public bool HasNewMessages(Guid userGuid)
        {
            using (var db = new IntiDataContext(_connectionString))
            {
                var messages = from mrec in db.Ext_MessageRecipient
                               where mrec.Sys_User.GUID == userGuid &&
                                     mrec.ReadOn == null
                               select mrec;

                if (messages.ToList().Count > 0)
                    return true;
            }

            return false;
        }

        public IList<Sys_User> GetAllUsers()
        {
            IList<Sys_User> userList = null;
            using (var db=new IntiDataContext(_connectionString))
            {
                var users = from u in db.Sys_User
                            orderby u.UserName
                            select u;

                userList = users.ToList();
            }

            return userList;
        }

        public void SaveUser(Guid userGuid, string userName, string firstName, string lastName, List<Guid> permissionGuids)
        {
            using(var db = new IntiDataContext(_connectionString))
            {
                var user = db.Sys_User.Single(u => u.GUID == userGuid);

                //is user name still unique?
                if (userName != user.UserName)
                {
                    var users = from u in db.Sys_User
                                where u.UserName == userName &&
                                      u.GUID != userGuid
                                select u;

                    if(users.ToList().Count > 0)
                    {
                        throw new Exception(String.Format("The Username {0} not unique", userName));
                    }
                }

                var message = "";
                if (user.UserName != userName)
                {
                    message += String.Format("Changed {0} from {1} to {2}. ", "UserName", user.UserName, userName);
                    user.UserName = userName;
                }
                
                if (user.FirstName!= firstName)
                {
                    message += String.Format("Changed {0} from {1} to {2}. ", "FirstName", user.FirstName, firstName);
                    user.FirstName = firstName;
                }

                if (user.LastName != lastName)
                {
                    message += String.Format("Changed {0} from {1} to {2}. ", "LastName", user.LastName, lastName);
                    user.LastName = lastName;
                }

                if(!String.IsNullOrEmpty(message))
                    LogEvent(userGuid, user.GetType(), SessionProperties.UserGuid, SessionProperties.ClientInfo, EventType.Change, message);

                //clear permissions
                var userPermissions = from up in db.Sys_UserPermission
                                      where up.UserGUID == userGuid
                                      select up;

                foreach (Sys_UserPermission userPermission in userPermissions.ToList())
                {
                    if (permissionGuids.Contains(userPermission.PermissionGUID))
                    {
                        permissionGuids.Remove(userPermission.PermissionGUID);
                    }
                    else
                    {
                        db.Sys_UserPermission.DeleteOnSubmit(userPermission);
                        LogEvent(userGuid, user.GetType(), SessionProperties.UserGuid, SessionProperties.ClientInfo, EventType.Change, "Removed permission " + userPermission.Sys_Permission.Name);
                    }
                }

                foreach (Guid permissionGuid in permissionGuids)
                {
                    var userPermission = new Sys_UserPermission();
                    userPermission.UserGUID = userGuid;
                    userPermission.PermissionGUID = permissionGuid;

                    Guid guid = permissionGuid;
                    var permission = db.Sys_Permission.Single(p => p.GUID == guid);
                    LogEvent(userGuid, user.GetType(), SessionProperties.UserGuid, SessionProperties.ClientInfo, EventType.Change, "Added permission " + permission.Name);

                    db.Sys_UserPermission.InsertOnSubmit(userPermission);
                }

                db.SubmitChanges();

            }
        }
    }


    public class IntiGeneralException : Exception
    {
        public IntiGeneralException(string message)
            : base(message)
        {
        }

        public IntiGeneralException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }

    public sealed class Parameters
    {
        

        #region Thread-safe, lazy Singleton

        /// <summary>
        /// This is a thread-safe, lazy singleton.  See http://www.yoda.arachsys.com/csharp/singleton.html
        /// for more details about its implementation.
        /// </summary>
        public static Parameters Instance
        {
            get
            {
                return Nested.Parameters;
            }
        }

        public static void Init(string connectionString)
        {
            Instance.SetConnectionString(connectionString);
        }

        private void SetConnectionString(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Private constructor to enforce singleton
        /// </summary>
        private Parameters() 
        {
            try
            {


            }
            catch (Exception exception)
            {
                throw new Exception("Could not initiate Parameters, configuration is missing?", exception);
            }
            
        }

        /// <summary>
        /// Assists with ensuring thread-safe, lazy singleton
        /// </summary>
        private class Nested
        {
            static Nested() { }
            internal static readonly Parameters Parameters =
                new Parameters();
        }

        #endregion

        private string mailSender;
        private string mailServer;
        private string forgotPasswordSubject;
        private string forgotPasswordBody;
        private string supportMail;
        private string mailServerPort;
        private string mailServerAuthenticate;
        private string mailServerUserName;
        private string mailServerPassword;
        private string _connectionString;
        private string twitterAccessToken;
        private string twitterAccessTokenSecret;

        public string MailServer
        {
            get
            {
                if (String.IsNullOrEmpty(mailServer))
                    InitParameters();
                return mailServer;
            }
        }

        public int MailServerPort
        {
            get
            {
                if(String.IsNullOrEmpty(mailServerPort))
                    InitParameters();

                try
                {
                    return int.Parse(mailServerPort);
                }
                catch(Exception ex)
                {
                    //do nothing
                }
                //return default value
                return 587;
            }
        }

        public bool MailServerAuthenticate
        {
            get
            {
                if (String.IsNullOrEmpty(mailServerAuthenticate))
                    InitParameters();

                return (mailServerAuthenticate.Trim() == "1");
            }
        }

        public string MailServerUserName
        {
            get
            {
                if (String.IsNullOrEmpty(mailServerUserName))
                    InitParameters();

                return mailServerUserName;
            }
        }

        public string MailServerPassword
        {
            get
            {
                if (String.IsNullOrEmpty(mailServerPassword))
                    InitParameters();

                return mailServerPassword;
            }
        }

        public string MailSender
        {
            get
            {
                if (String.IsNullOrEmpty(mailSender))
                    InitParameters();
               
                return mailSender;
            }
        }

        public string ForgotPasswordBody
        {
            get
            {
                if (String.IsNullOrEmpty(forgotPasswordBody))
                    InitParameters();

                return forgotPasswordBody;
            }
        }

        public string ForgotPasswordSubject
        {
            get
            {
                if (String.IsNullOrEmpty(forgotPasswordSubject))
                    InitParameters();
                return forgotPasswordSubject;
            }
        }

        public string SupportMail
        {
            get 
            {
                if (String.IsNullOrEmpty(supportMail))
                    InitParameters();
                return supportMail;
            }
        }

        public string TwitterAccessToken
        {
            get
            {
                if (String.IsNullOrEmpty(twitterAccessToken))
                    InitParameters();
                return twitterAccessToken;
            }
            set
            {
                UpdateParameter("TwitterAccessToken", value);
                twitterAccessToken = value;
            }
        }


        public string TwitterAccessTokenSecret
        {
            get
            {
                if (String.IsNullOrEmpty(twitterAccessTokenSecret))
                    InitParameters();
                return twitterAccessTokenSecret;
            }
            set
            {
                UpdateParameter("TwitterAccessTokenSecret", value);
                twitterAccessTokenSecret = value;
            }
        }


        private void UpdateParameter(string parameterName, string value)
        {
            using (var db = new IntiDataContext(_connectionString))
            {
                var parameter = db.Sys_Parameter.SingleOrDefault(p => p.Name == parameterName);
                if (parameter == null) return;

                parameter.Value = value;

                db.SubmitChanges();
            }
        }

        public void UpdateParameters()
        {
            using (var db = new IntiDataContext(_connectionString))
            {
                //done
                //AddParameter(db, "TwitterAccessToken");
                //AddParameter(db, "TwitterAccessTokenSecret");
            }
                
        }


        private void InitParameters()
        {
            using (var db = new IntiDataContext(_connectionString))
            {
                var parameters = from param in db.Sys_Parameter
                            select param;

                foreach(var parameter in parameters)
                {
                    switch(parameter.Name)
                    {
                        case "MailServer":
                            mailServer = parameter.Value;
                            break;
                        case "MailServerPort" :
                            mailServerPort = parameter.Value;
                            break;
                        case "MailServerAuthenticate" :
                            mailServerAuthenticate = parameter.Value;
                            break;
                        case "MailServerUserName" :
                            mailServerUserName = parameter.Value;
                            break;
                        case "MailServerPassword" :
                            mailServerPassword = parameter.Value;
                            break;
                        case "MailSender" :
                            mailSender = parameter.Value;
                            break;
                        case "ForgotPasswordSubject" :
                            forgotPasswordSubject = parameter.Value;
                            break;
                        case "ForgotPasswordBody" :
                            forgotPasswordBody = parameter.Value;
                            break;
                        case "SupportMail" :
                            supportMail = parameter.Value;
                            break;
                        case "TwitterAccessToken" :
                            twitterAccessToken = parameter.Value;
                            break;
                        case "TwitterAccessTokenSecret" :
                            twitterAccessTokenSecret = parameter.Value;
                            break;
                    }
                }

            }
        }

        /// <summary>
        /// Adds a new parameter if it doesn't exist
        /// </summary>
        /// <param name="db"></param>
        /// <param name="parameterName"></param>
        private static void AddParameter(IntiDataContext db, string parameterName)
        {
            if (!db.Sys_Parameter.Any(p => p.Name == parameterName))
            {
                var parameter = new Sys_Parameter();
                parameter.GUID = Guid.NewGuid();
                parameter.Name = parameterName;
                parameter.Description = parameterName;
                

                db.Sys_Parameter.InsertOnSubmit(parameter);

                db.SubmitChanges();
            }
        }
    }

    public static class MailAndLog
    {
        public static void SendMessage(string subject, string body, string mailSender, string mailRecipient)
        {
            try
            {
                //create mail to user
                var message = new MailMessage(mailSender, mailRecipient);

                message.Subject = subject;
                message.Body = body;
                message.IsBodyHtml = true;

                //Send the message.
                var client = new SmtpClient(Parameters.Instance.MailServer,Parameters.Instance.MailServerPort);
                client.EnableSsl = Parameters.Instance.MailServerAuthenticate;
                // Add credentials if the SMTP server requires them.
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(Parameters.Instance.MailServerUserName, Parameters.Instance.MailServerPassword);
                client.Send(message);
            }
            catch (Exception exception)
            {
                
                //do nothing
            }


        }
    }

    public static class StringExtension
    {
        public static byte[] ToByteArray(this string input)
        {
            var outputBuffer = new byte[input.ToCharArray().Length];
            var i = 0;
            foreach (var ch in input.ToCharArray())
            {
                outputBuffer[i] = (byte)ch;
                i++;
            }

            return outputBuffer;
        }

        public static string ToString(this byte[] input)
        {
            string output = "";

            for (var i = 0; i < input.Length; i++)
            {
                output += char.ConvertFromUtf32(input[i]);
            }

            return output;
        }
    }



}
