using System;
using System.Collections.Generic;
using System.Linq;

namespace inti2008.Data
{
    public abstract class IntiManagementBase
    {
        protected readonly string _connectionString;
        private SessionProperties _sessionProperties;

        public IntiManagementBase(string connectionString, SessionProperties sessionProperties)
        {
            _connectionString = connectionString;
            _sessionProperties = sessionProperties;
        }

        private IntiManagementBase()
        {
            throw new ArgumentNullException("sessionProperties", "Data management classes cannot be called without session properties.");
        }

        internal SessionProperties SessionProperties
        {
            get { return _sessionProperties; }
        }

        internal void LogEvent(Guid objectId, Type objectType, Guid userGuid, string client, EventType action, string message)
        {
            using (var db = new IntiDataContext(_connectionString))
            {
                var logEvent = new Ext_ChangeLog();
                logEvent.ObjectGUID = objectId;
                logEvent.ObjectType = objectType.Name;
                logEvent.UserGUID = userGuid;
                logEvent.Client = client;
                logEvent.Action = action.ToString();
                logEvent.Message = message;
                logEvent.LogDate = DateTime.Now; 
               
                db.Ext_ChangeLog.InsertOnSubmit(logEvent);
                db.SubmitChanges();
            }
        }

        public IList<Ext_ChangeLog> GetChangeLog(IntiDataContext db, Guid objectId)
        {
            var q = from cl in db.Ext_ChangeLog
                    where cl.ObjectGUID == objectId
                    select cl;

            return q.ToList();
        }

        public IList<Ext_ChangeLog> GetChangeLog(IntiDataContext db, Type type)
        {
            var q = from cl in db.Ext_ChangeLog
                    where cl.ObjectType == type.Name
                    select cl;

            return q.ToList();
        }
    }
}