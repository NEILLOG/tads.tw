using TADS_Web.Models;
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

        /// <summary>
        /// 取得本屆年會三個區塊的內容與學位論文獎下載檔（首頁與 /Annual/Current 共用）。
        /// 查詢失敗時回傳空 VM，前台會退回顯示 View 內的預設靜態內容。
        /// </summary>
        public VM_AnnualCurrentContent GetAnnualCurrentContent(ref string ErrMsg)
        {
            try
            {
                string[] codes = { "AnnualCurrent_Intro", "AnnualCurrent_Topics", "AnnualCurrent_Award" };
                var rows = (from page in _context.TbPageContent
                            where codes.Contains(page.PageCode)
                            join file in _context.TbFileInfo.Where(x => !x.IsDelete)
                                on page.FileId equals file.FileId into fileGroup
                            from file in fileGroup.DefaultIfEmpty()
                            select new
                            {
                                page.PageCode,
                                page.Contents,
                                FileUrl = file != null ? "/" + file.FilePath : null,
                                FileName = file != null ? file.FileName : null,
                            }).ToDictionary(x => x.PageCode);

                var award = rows.GetValueOrDefault("AnnualCurrent_Award");
                return new VM_AnnualCurrentContent
                {
                    IntroHtml = rows.GetValueOrDefault("AnnualCurrent_Intro")?.Contents,
                    TopicsHtml = rows.GetValueOrDefault("AnnualCurrent_Topics")?.Contents,
                    AwardHtml = award?.Contents,
                    AwardFileUrl = award?.FileUrl,
                    AwardFileName = award?.FileName,
                };
            }
            catch (Exception ex)
            {
                ErrMsg = ex.ToString();
                return new VM_AnnualCurrentContent();
            }
        }
    }
}
