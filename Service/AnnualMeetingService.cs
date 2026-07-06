using TADS_Web.Models.DB;
using TADS_Web.Service.Base;

namespace TADS_Web.Service
{
    public class AnnualMeetingService : ServiceBase
    {
        public AnnualMeetingService(DBContext context) : base(context) { }

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
