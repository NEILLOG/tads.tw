using TADS_Web.Models.DB;

namespace TADS_Web.Models.Extend
{
    public class MemberResearchExtend
    {
        public TbMemberResearch Research { get; set; } = null!;

        /// <summary>封面圖顯示路徑（由 TbFileInfo.FilePath 組成）</summary>
        public string? CoverUrl { get; set; }
    }
}
