using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using inti2008.Data;

namespace inti2008.Web
{
    public static class WebControlManager
    {
        public static void SetProperty(Control webControl, string propertyName, string value)
        {
            Type wctype = webControl.GetType();
            PropertyInfo property = wctype.GetProperty(propertyName);

            if (property != null)
            {
                property.SetValue(webControl, Convert.ChangeType(value, property.PropertyType), null);
            }
        }

        public static string GetValue(Control webControl)
        {
            Type wctype = webControl.GetType();

            PropertyInfo property = wctype.GetProperty("Text");

            if (property == null)
            {
                property = wctype.GetProperty("Value");
            }

            if (property == null)
            {
                property = wctype.GetProperty("SelectedValue");
            }

            if (property == null)
            {
                throw new Exception("Cannot get value from " + webControl.ID + " control.");
            }

            return property.GetValue(webControl, null).ToString();
        }


        public static void SetValue(Control webControl, string value)
        {
            Type wctype = webControl.GetType();

            PropertyInfo property = wctype.GetProperty("Text");

            if (property == null)
            {
                property = wctype.GetProperty("Value");
            }
            if (property == null)
            {
                property = wctype.GetProperty("SelectedValue");
            }

            if (property == null)
            {
                throw new Exception("Cannot set value for " + webControl.ID + " control.");
            }

            try
            {
                property.SetValue(webControl, value, null);
            }
            catch (TargetInvocationException targetInvocationException)
            {
                //just a case of trying to set a value to a drop down box where the value does not exist. 
                //move along?
            }
        }

        public static Control GetControl(string controlId, Control form)
        {
            var control = form.FindControl(controlId);

            if (control != null)
            {
                return control;
            }

            //loop child naming containers for the control
            foreach (Control controlToBeLooped in form.Controls)
            {
                if (controlToBeLooped.HasControls())
                {
                    control = GetControl(controlId, controlToBeLooped);
                    if (control != null)
                        return control;
                }
            }

            return null;
        }

        /// <summary>
        /// Writes the data in the given entity object to the given form
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="form"></param>
        public static void EntityToForm(object entity, System.Web.UI.Control form)
        {
            PropertyInfo[] properties = entity.GetType().GetProperties();

            //loop the properties of the entity, try to find a matching webcontrol for each one
            foreach (var property in properties)
            {
                var control = WebControlManager.GetControl(property.Name, form);

                if (control != null)
                {
                    var value = property.GetValue(entity, null);
                    if (value != null)
                        WebControlManager.SetValue(control, value.ToString());
                }

            }
        }

        /// <summary>
        /// Writes the data in the given webform to the given entity object
        /// </summary>
        /// <param name="form"></param>
        /// <param name="entity"></param>
        public static void FormToEntity(System.Web.UI.Control form, object entity)
        {
            PropertyInfo[] properties = entity.GetType().GetProperties();

            //loop the properties of the entity, try to find a matching webcontrol for each one
            foreach (var property in properties)
            {
                var control = WebControlManager.GetControl(property.Name, form);

                if (control != null)
                {
                    var value = WebControlManager.GetValue(control);

                    //convert to correct type...
                    if (property.CanWrite)
                    {
                        var convertedValue = ConvertValue(value, property.PropertyType);

                        //only set value if conversion was successful
                        if (convertedValue.GetType().Equals(property.PropertyType))
                            property.SetValue(entity, ConvertValue(value, property.PropertyType), null);
                    }
                        
                }
            }
        }


        private static object ConvertValue(string value, Type outputType)
        {
            var parseMethod = outputType.GetMethod("Parse", new Type[] { typeof(string) });
            if (parseMethod != null)
            {
                return parseMethod.Invoke(outputType, new object[] { value });
            }
            
            return value;
        }


        public static void MakeFullGridRowClickable(GridViewRow row, ClientScriptManager clientScriptManager)
        {
            if (row.RowType == DataControlRowType.DataRow)
            {
                // Get the LinkButton control in the first cell

                LinkButton _singleClickButton = (LinkButton)row.Cells[0].Controls[0];
                // Get the javascript which is assigned to this LinkButton

                string _jsSingle =
                clientScriptManager.GetPostBackClientHyperlink(_singleClickButton, "");

                // To prevent the first click from posting back immediately
                // (therefore giving the user a chance to double click) 
                // pause the postbackfor 300 milliseconds by 
                // wrapping the postback command in a setTimeout

                _jsSingle = _jsSingle.Insert(11, "setTimeout(\"");
                _jsSingle += "\", 300)";

                // Add this javascript to the onclick Attribute of the row
                row.Attributes["onclick"] = _jsSingle;

                // Get the LinkButton control in the second cell
                LinkButton _doubleClickButton = (LinkButton)row.Cells[1].Controls[0];

                // Get the javascript which is assigned to this LinkButton
                string _jsDouble =
                clientScriptManager.GetPostBackClientHyperlink(_doubleClickButton, "");

                // Add this javascript to the ondblclick Attribute of the row
                row.Attributes["ondblclick"] = _jsDouble;
            }
        }


        private const string redirectParameterPrefix = "rp_";

        /// <summary>
        /// Redirect to page without showing the parameters in the querystring
        /// </summary>
        /// <param name="page">Url to redirect to</param>
        /// <param name="parameters">Array with the parameter names</param>
        /// <param name="values">Array with the parameter values</param>
        public static void RedirectWithQueryString(string page, string[] parameters, string[] values)
        {
            //clear old redirect values
            ClearRedirectCache();

            for (var i = 0; i < parameters.Length; i++)
            {
                HttpContext.Current.Session.Add(redirectParameterPrefix + parameters[i], values[i]);
            }

            HttpContext.Current.Response.Redirect(page);

        }

        /// <summary>
        /// Get a parameter passed with the RedirectWithQueryString method
        /// </summary>
        /// <param name="page"></param>
        /// <param name="parameter">The name of the parameter</param>
        /// <param name="clear">If true, clear the parameter from the session object in this call</param>
        /// <returns></returns>
        public static string GetRedirectParameter(this Page page, string parameter, bool clear)
        {
            if (page.RouteData != null && page.RouteData.Values[parameter] != null)
                return page.RouteData.Values[parameter].ToString();

            if (HttpContext.Current.Request.Params[parameter] != null)
                return HttpContext.Current.Request.Params[parameter];

            foreach (var param in HttpContext.Current.Session.Keys)
            {
                if (param.ToString().StartsWith(redirectParameterPrefix) &&
                    param.ToString().Replace(redirectParameterPrefix,"").Equals(parameter))
                {
                    var value = HttpContext.Current.Session[param.ToString()];

                    if (clear)
                        HttpContext.Current.Session.Remove(param.ToString());

                    return value.ToString();
                }
            }

            return null;
        }

        public static void ClearRedirectCache()
        {

            bool deleted = true;

            while (deleted)
            {
                deleted = false;
                foreach (var param in HttpContext.Current.Session.Keys)
                {
                    if (param.ToString().StartsWith(redirectParameterPrefix))
                    {
                        HttpContext.Current.Session.Remove(param.ToString());
                        deleted = true;
                        break;
                    }
                }
            }
        }

        public static void SendAndLogErrorMessage(Exception exception, string mailSender, string mailRecipient)
        {
            //info on current session


            var messageBody = String.Empty;

            //info of the session
            messageBody += "<h4>Session info:</h4>";
            if (Global.SessionProperties.UserName != null)
            {
                messageBody += "<p>Username: " + Global.SessionProperties.UserName + "</p>";
            }
            else
            {
                messageBody += "<p>Anonymous</p>";
            }

            //info of the request/browser
            if (HttpContext.Current != null)
            {
                var request = HttpContext.Current.Request;
                messageBody += "<h4>Request/browser info:</h4>";
                messageBody += "<ul>";
                
                messageBody += "<li>Path: ";
                messageBody += request.Url;
                messageBody += "</li>";
                messageBody += "<li>UserAgent: ";
                messageBody += request.UserAgent;
                messageBody += "</li>";
                messageBody += "<li>UserHostAddress: ";
                messageBody += request.UserHostAddress;
                messageBody += "</li>";

                messageBody += "</ul>";
                    
            }

            //info of the error
            messageBody += "<h4>Error info:</h4>";
            messageBody += "<p>" + HttpUtility.HtmlEncode(exception.Message).Replace("\n", "<br/>") + "</p>";
            messageBody += "<p><small>" + HttpUtility.HtmlEncode(exception.StackTrace).Replace("\n", "<br/>") + "<small></p>";
            

            MailAndLog.SendMessage("IntiFel ("+exception.GetType().Name+")", messageBody, mailSender, mailRecipient);
        }


        public static string GetClientInfo()
        {
            if (HttpContext.Current != null)
            {
                return String.Format("Browser: {0}({1}), Ip: {2}", HttpContext.Current.Request.Browser.Browser,
                                               HttpContext.Current.Request.Browser.Version,
                                               HttpContext.Current.Request.UserHostAddress);    
            }
            
            return "No HttpContext";
        }

        public static void RefreshPage()
        {
            var responseUrl = HttpContext.Current.Request.Url;
            HttpContext.Current.Response.Redirect(responseUrl.ToString());    

            //if(HttpContext.Current.Session["InRefresh"] == null)
            //{
            //    HttpContext.Current.Session["InRefresh"] = "Y";
                
            //}
            //else
            //{
            //    HttpContext.Current.Session.Remove("InRefresh");
            //}
            
        }
    }
}