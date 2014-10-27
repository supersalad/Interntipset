using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace inti2008.Web.api
{
    /// <summary>
    /// Summary description for CalendarLink
    /// </summary>
    public class CalendarLink : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            var startdate = context.Request.Params["startdate"];
            var enddate = context.Request.Params["enddate"];
            var name = context.Request.Params["name"];
            var id = context.Request.Params["id"];
            if (String.IsNullOrEmpty(id) || String.IsNullOrEmpty(startdate) || String.IsNullOrEmpty(enddate) || String.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Missing parameter");
            }

            var startDate = DateTime.Parse(startdate);
            var endDate = DateTime.Parse(enddate);


            context.Response.ContentType = "text/calendar";
            var responseText = @"BEGIN:VCALENDAR
VERSION:2.0
PRODID:-//interntipset/calendar//NONSGML v1.0//EN
BEGIN:VEVENT
UID:{0}
DTSTAMP:{1}
ORGANIZER;CN=Klaz Interntipset:MAILTO:interntipset@gmail.com
DTSTART:{2}
DTEND:{3}
SUMMARY:{4}
END:VEVENT
END:VCALENDAR";


            context.Response.Write(String.Format(responseText, 
                id, 
                DateTime.UtcNow.ToString("o"),
                startDate.ToUniversalTime().ToString("o"),
                endDate.ToUniversalTime().ToString("o"),
                name));

        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        public static string GetCalendarLink(string name, DateTime startDate, DateTime endDate, string id)
        {
            return String.Format("<a class='btn btn-primary' href='api/CalendarLink.ashx?startdate={0}&enddate={1}&name={2}&id={3}' target='_blank'>Lägg till i din kalender</a>",
                HttpUtility.UrlEncode(startDate.ToString()), HttpUtility.UrlEncode(endDate.ToString()),
                HttpUtility.UrlEncode(name), HttpUtility.UrlEncode(id));
        }
    }
}