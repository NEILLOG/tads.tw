using TADS_Web.Models.DB;
using TADS_Web.Models.Extend;
using TADS_Web.Service.Base;

namespace TADS_Web.Service
{
    public class NewsService : ServiceBase
    {
        public NewsService(DBContext context) : base(context) { }

        public IQueryable<NewsExtend>? GetNewsExtendList(ref string ErrMsg)
        {
            try
            {
                IQueryable<NewsExtend> dataList = (from news in _context.TbNews
                                                   where news.IsDelete == false
                                                   select new NewsExtend { News = news });
                return dataList;
            }
            catch (Exception ex)
            {
                ErrMsg = ex.ToString();
                return null;
            }
        }

        public IQueryable<NewsExtend>? GetNewsExtendItem(ref string ErrMsg, string id)
        {
            try
            {
                IQueryable<NewsExtend> dataList = (from news in _context.TbNews
                                                   where news.IsDelete == false && news.Id == id
                                                   select new NewsExtend { News = news });
                return dataList;
            }
            catch (Exception ex)
            {
                ErrMsg = ex.ToString();
                return null;
            }
        }

        public List<string> GetTopId(ref string ErrMsg)
        {
            try
            {
                return (from news in _context.TbNews
                        where news.IsDelete == false && news.IsKeepTop == true
                        select news.Id).ToList();
            }
            catch (Exception ex)
            {
                ErrMsg = ex.ToString();
                return new List<string>();
            }
        }
    }
}
