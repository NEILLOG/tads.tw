using TADS_Web.Models.DB;
using TADS_Web.Service.Base;

namespace TADS_Web.Service
{
    public class PageContentService : ServiceBase
    {
        public PageContentService(DBContext context) : base(context) { }

        /// <summary>後台清單：所有可編輯內頁（固定集合）。</summary>
        public IQueryable<TbPageContent>? GetList(ref string ErrMsg)
        {
            try
            {
                return _context.TbPageContent;
            }
            catch (Exception ex)
            {
                ErrMsg = ex.ToString();
                return null;
            }
        }

        /// <summary>依頁面代碼取得單筆內容（前台與後台編輯共用）。</summary>
        public IQueryable<TbPageContent>? GetByCode(ref string ErrMsg, string pageCode)
        {
            try
            {
                return _context.TbPageContent.Where(x => x.PageCode == pageCode);
            }
            catch (Exception ex)
            {
                ErrMsg = ex.ToString();
                return null;
            }
        }
    }
}
