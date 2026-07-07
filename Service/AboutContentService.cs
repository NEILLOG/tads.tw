using TADS_Web.Models.DB;
using TADS_Web.Service.Base;

namespace TADS_Web.Service
{
    public class AboutContentService : ServiceBase
    {
        public const string AboutId = "About";

        public AboutContentService(DBContext context) : base(context) { }

        /// <summary>取得「關於我們」主記錄（單筆）。</summary>
        public TbAboutContent? GetAbout(ref string ErrMsg)
        {
            try
            {
                return _context.TbAboutContent.FirstOrDefault(x => x.Id == AboutId);
            }
            catch (Exception ex)
            {
                ErrMsg = ex.ToString();
                return null;
            }
        }

        /// <summary>取得會費價目表（6 張，依 SortOrder）。</summary>
        public List<TbAboutPricing> GetPricing(ref string ErrMsg)
        {
            try
            {
                return _context.TbAboutPricing.OrderBy(x => x.SortOrder).ToList();
            }
            catch (Exception ex)
            {
                ErrMsg = ex.ToString();
                return new List<TbAboutPricing>();
            }
        }
    }
}
