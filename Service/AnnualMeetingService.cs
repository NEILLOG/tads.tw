using TADS_Web.Models.DB;
using TADS_Web.Models.Extend;
using TADS_Web.Service.Base;

namespace TADS_Web.Service
{
    public class AnnualMeetingService : ServiceBase
    {
        public AnnualMeetingService(DBContext context) : base(context) { }

        /// <summary>取得單筆年會資料並帶入附件資訊（前台內頁用）</summary>
        public IQueryable<AnnualMeetingExtend>? GetItemExtend(ref string ErrMsg, string id)
        {
            try
            {
                IQueryable<AnnualMeetingExtend> dataList =
                    from annual in _context.TbAnnualMeeting
                    where annual.IsDelete == false && annual.Id == id
                    join file in _context.TbFileInfo.Where(x => !x.IsDelete)
                        on annual.FileId equals file.FileId into fileGroup
                    from file in fileGroup.DefaultIfEmpty()
                    select new AnnualMeetingExtend
                    {
                        AnnualMeeting = annual,
                        FileUrl = file != null ? "/" + file.FilePath : null,
                        FileName = file != null ? file.FileName : null
                    };
                return dataList;
            }
            catch (Exception ex)
            {
                ErrMsg = ex.ToString();
                return null;
            }
        }

        public IQueryable<TbAnnualMeeting>? GetList(ref string ErrMsg)
        {
            try
            {
                return _context.TbAnnualMeeting.Where(x => x.IsDelete == false);
            }
            catch (Exception ex)
            {
                ErrMsg = ex.ToString();
                return null;
            }
        }

        public IQueryable<TbAnnualMeeting>? GetItem(ref string ErrMsg, string id)
        {
            try
            {
                return _context.TbAnnualMeeting.Where(x => x.IsDelete == false && x.Id == id);
            }
            catch (Exception ex)
            {
                ErrMsg = ex.ToString();
                return null;
            }
        }
    }
}
