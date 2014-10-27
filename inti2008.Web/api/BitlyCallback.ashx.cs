using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace inti2008.Web.api
{
    /// <summary>
    /// Summary description for BitlyCallback
    /// </summary>
    public class BitlyCallback : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            context.Response.Write("Hello World");
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}