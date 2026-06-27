namespace TADS_Web.Models.DB
{
    public partial class TbMemberResearch
    {
        public string Id { get; set; } = null!;

        /// <summary>標題：論文/專書名稱</summary>
        public string Title { get; set; } = null!;

        /// <summary>封面圖案（對應 TbFileInfo）</summary>
        public string? FileId { get; set; }

        /// <summary>作者資訊：單位–姓名–職稱（標題下方顯示行）</summary>
        public string? AuthorInfo { get; set; }

        /// <summary>簡介標題（粗體提問句）</summary>
        public string? IntroTitle { get; set; }

        /// <summary>簡介內容</summary>
        public string? IntroContent { get; set; }

        /// <summary>學術引用格式（APA）</summary>
        public string? Citation { get; set; }

        /// <summary>作者簡介：姓名（中英）</summary>
        public string? AuthorBioName { get; set; }

        /// <summary>作者簡介：現職</summary>
        public string? AuthorBioPosition { get; set; }

        /// <summary>作者簡介：研究專長</summary>
        public string? AuthorBioExpertise { get; set; }

        /// <summary>排序（數字越小越前面）</summary>
        public int SortOrder { get; set; }

        public bool IsDelete { get; set; }
        public bool IsPublish { get; set; }

        public string CreateUser { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public string? ModifyUser { get; set; }
        public DateTime? ModifyDate { get; set; }
    }
}
