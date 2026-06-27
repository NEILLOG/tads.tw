using TADS_Web.Models.DB;
using TADS_Web.Models.Extend;
using TADS_Web.Service.Base;

namespace TADS_Web.Service
{
    public class MemberResearchService : ServiceBase
    {
        public MemberResearchService(DBContext context) : base(context) { }

        public IQueryable<MemberResearchExtend>? GetExtendList(ref string ErrMsg)
        {
            try
            {
                IQueryable<MemberResearchExtend> dataList =
                    from research in _context.TbMemberResearch
                    where research.IsDelete == false
                    join file in _context.TbFileInfo.Where(x => !x.IsDelete)
                        on research.FileId equals file.FileId into fileGroup
                    from file in fileGroup.DefaultIfEmpty()
                    select new MemberResearchExtend
                    {
                        Research = research,
                        CoverUrl = file != null ? "/" + file.FilePath : null
                    };
                return dataList;
            }
            catch (Exception ex)
            {
                ErrMsg = ex.ToString();
                return null;
            }
        }

        public IQueryable<MemberResearchExtend>? GetExtendItem(ref string ErrMsg, string id)
        {
            try
            {
                IQueryable<MemberResearchExtend> dataList =
                    from research in _context.TbMemberResearch
                    where research.IsDelete == false && research.Id == id
                    join file in _context.TbFileInfo.Where(x => !x.IsDelete)
                        on research.FileId equals file.FileId into fileGroup
                    from file in fileGroup.DefaultIfEmpty()
                    select new MemberResearchExtend
                    {
                        Research = research,
                        CoverUrl = file != null ? "/" + file.FilePath : null
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
