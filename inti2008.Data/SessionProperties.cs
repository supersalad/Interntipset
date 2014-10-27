using System;
using System.Collections.Generic;

namespace inti2008.Data
{
    public class SessionProperties
    {
        public static SessionProperties NullSessionProperties()
        {
            return new SessionProperties(true, String.Empty);
        }

        public SessionProperties(bool isAnonymous, string clientInfo)
        {
            if (isAnonymous)
            {
                //todo: populate permissions list with anonymous rights
                
            }
            ClientInfo = clientInfo;

            //set footer text
            FooterText = "anonym";
        }

        public void SignOut()
        {
            userName = null;
            userGuid = Guid.Empty;
            permissions = new List<Sys_Permission>();
            FooterText = "anonym";
        }

        private string userName = null;

        public string UserName
        { 
            get { 
                return userName;
            }
            set { 
                userName = value;
            } 
        }

        private Guid userGuid = Guid.Empty;

        public Guid UserGuid
        {
            get
            {
                return userGuid;
            }
            set
            {
                userGuid = value;
            }
        }

        //private string _twitterAccessToken = ""; //"vguf9PfB4VYzxAm9nyLRRrpOeb9Jywb2E0Vn8UaWg2A";
        //public string TwitterAccessToken
        //{
        //    get { return _twitterAccessToken; }
        //    set { _twitterAccessToken = value; }
        //}

        //private string _twitterAccessTokenSecret = "6bxzbGMEY0DAQifT5XdFvhzJFERlJRfr2r9S3O6XE";
        //public string TwitterAccessTokenSecret
        //{
        //    get { return _twitterAccessTokenSecret; }
        //    set { _twitterAccessTokenSecret = value; }
        //}

        private IList<Sys_Permission> permissions = new List<Sys_Permission>();
        private Inti_Tournament _selectedTournament;

        public IList<Sys_Permission> Permissions
        {
            get { return permissions; }
            set
            {
                permissions = value;   
            }
        }

        public bool HasPermission(string permissionPrefix)
        {
            foreach(var permission in Permissions)
            {
                if (permission.Name.StartsWith(permissionPrefix))
                    return true;
            }

            return false;
        }

        public Inti_Tournament SelectedTournament
        {
            get { return _selectedTournament  ?? DefaultTournament; }
            set { _selectedTournament = value; }
        }

        public Inti_Tournament DefaultTournament { get; set; }

        public string FooterText { get; set; }

        public string ClientInfo { get; set; }
    }
}