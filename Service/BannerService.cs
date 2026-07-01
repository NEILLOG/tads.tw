using TADS_Web.Models.DB;
using TADS_Web.Models.Extend;
using TADS_Web.Service.Base;

namespace TADS_Web.Service
{
    public class BannerService : ServiceBase
    {
        public BannerService(DBContext context) : base(context) { }

        public IQueryable<BannerExtend>? GetExtendList(ref string ErrMsg)
        {
            try
            {
                IQueryable<BannerExtend> dataList =
                    from banner in _context.TbBanner
                    where banner.IsDelete == false
                    join file in _context.TbFileInfo.Where(x => !x.IsDelete)
                        on banner.FileId equals file.FileId into fileGroup
                    from file in fileGroup.DefaultIfEmpty()
                    select new BannerExtend
                    {
                        Banner = banner,
                        ImageUrl = file != null ? "/" + file.FilePath : null
                    };
                return dataList;
            }
            catch (Exception ex)
            {
                ErrMsg = ex.ToString();
                return null;
            }
        }

        public IQueryable<BannerExtend>? GetExtendItem(ref string ErrMsg, string id)
        {
            try
            {
                IQueryable<BannerExtend> dataList =
                    from banner in _context.TbBanner
                    where banner.IsDelete == false && banner.Id == id
                    join file in _context.TbFileInfo.Where(x => !x.IsDelete)
                        on banner.FileId equals file.FileId into fileGroup
                    from file in fileGroup.DefaultIfEmpty()
                    select new BannerExtend
                    {
                        Banner = banner,
                        ImageUrl = file != null ? "/" + file.FilePath : null
                    };
                return dataList;
            }
            catch (Exception ex)
            {
                ErrMsg = ex.ToString();
                return null;
            }
        }
    }
}
