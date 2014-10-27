using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace inti2008.Data
{
    public class NewsManagement : IntiManagementBase
    {
        public NewsManagement(string connectionString, SessionProperties sessionProperties) : base(connectionString, sessionProperties)
        {
        }

        public IList<Ext_News> GetAllNews(int pageSize, int pageIndex)
        {
            using (var db = new IntiDataContext(_connectionString))
            {
                var news = from n in db.Ext_News
                           select n;

                return news.OrderByDescending(order => order.ValidFrom).Take(pageSize).Skip(pageSize * pageIndex).ToList();
            }
        }

        public IList<Ext_News> GetNews(int pageSize, int pageIndex)
        {
            using (var db = new IntiDataContext(_connectionString))
            {
                var news = from n in db.Ext_News
                           where n.ValidFrom <= DateTime.Now
                                 && (n.ValidTo == null || n.ValidTo >= DateTime.Now)
                           select n;

                return news.OrderByDescending(order => order.ValidFrom).Skip(pageSize * pageIndex).Take(pageSize).ToList();
            }
        }

        public int GetNumberOfNews()
        {
            using (var db = new IntiDataContext(_connectionString))
            {
                var news = from n in db.Ext_News
                           where n.ValidFrom <= DateTime.Now
                                 && (n.ValidTo == null || n.ValidTo >= DateTime.Now)
                           select n;

                return news.ToList().Count;
            }
        }

        public IList<Ext_News> GetTopThreeNews()
        {
            return GetNews(3, 0);
        }

        public Ext_News GetNewsItem(Guid newsGuid)
        {
            using (var db = new IntiDataContext(_connectionString))
            {
                var news = from n in db.Ext_News
                           where n.GUID == newsGuid
                           select n;

                return news.ToList()[0];
            }
        }

        public Guid StoreNewNews(string header, string body, string picture, DateTime? validFrom, DateTime? validTo)
        {
            using (var db = new IntiDataContext(_connectionString))
            {
                var news = new Ext_News();
                news.Header = header;
                news.Body = body;
                news.Picture = picture;
                news.ValidFrom = validFrom;
                news.ValidTo = validTo;

                db.Ext_News.InsertOnSubmit(news);

                LogEvent(news.GUID, news.GetType(), SessionProperties.UserGuid, SessionProperties.ClientInfo, EventType.Create, "New news");

                db.SubmitChanges();

                return news.GUID;
            }
        }

        public void UpdateNews(Guid guid, string header, string body, string picture, DateTime? validFrom, DateTime? validTo)
        {
            using (var db = new IntiDataContext(_connectionString))
            {
                var news = db.Ext_News.Single(n => n.GUID == guid);

                news.Header = header;
                news.Body = body;
                news.Picture = picture;
                news.ValidFrom = validFrom;
                news.ValidTo = validTo;

                LogEvent(news.GUID, news.GetType(), SessionProperties.UserGuid, SessionProperties.ClientInfo, EventType.Update, "Update news");

                db.SubmitChanges();
            }
        }
    }
}
