using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.SessionState;
using inti2008.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace inti2008.Web.api.inti
{
    /// <summary>
    /// Summary description for GetUserTeam
    /// </summary>
    public class GetUserTeam : IHttpHandler, IReadOnlySessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            var userTeamId = context.Request.Params["UserTeamId"];
            if (userTeamId == null)
            {
                //send error
                SendException(context, new BadRequestException("GetUserTeam called without UserTeamId"));
            }
            Guid userTeamGuid;
            if (!Guid.TryParse(userTeamId, out userTeamGuid))
            {
                //send error
                SendException(context, new BadRequestException("GetUserTeam called without valid UserTeamId"));
            }


            var userTeamManagement = new UserTeamManagement(Global.ConnectionString, Global.SessionProperties);
            var userTeam = userTeamManagement.GetTeam(userTeamGuid);

            var userTeamDto = new UserTeamDTO();

            userTeamDto.Id = userTeam.GUID.ToString();
            userTeamDto.Name = userTeam.Name;
            userTeamDto.Description = userTeam.Presentation;
            
            //are we in a transfer period?
            var transferPeriod = new CommonDataFetches(Global.ConnectionString, Global.SessionProperties);



            JsonResponse(context, userTeamDto);

        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        private static void JsonResponse(HttpContext context, object data)
        {
            context.Response.ContentType = "text/json";

            var jsonSerializerSettings = new JsonSerializerSettings
            {
                DateTimeZoneHandling = DateTimeZoneHandling.Local
            };

            jsonSerializerSettings.Converters.Add(new IsoDateTimeConverter {  DateTimeFormat = System.Globalization.CultureInfo.InvariantCulture.DateTimeFormat.ShortDatePattern });

            jsonSerializerSettings.Converters.Add(new StringEnumConverter());


            context.Response.Write(JsonConvert.SerializeObject(data, jsonSerializerSettings));
        }

        private static void SendException(HttpContext context, Exception exception)
        {
            //global error handling for all handlers - will return a json response in stead of the standard html error page
            context.Response.StatusCode = exception is BadRequestException ? (int)HttpStatusCode.BadRequest : (int)HttpStatusCode.InternalServerError;

            //do this to avoid default error response?
            context.Response.TrySkipIisCustomErrors = true;

            //assume localized exception.Message?
            var errorModel = exception.Message;

            JsonResponse(context, errorModel);
        }
    }


    public class BadRequestException : Exception
    {
        public BadRequestException(string message)
            : base(message)
        {
        }
    }


}