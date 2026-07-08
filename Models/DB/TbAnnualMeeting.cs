namespace TADS_Web.Models.DB
{
    public partial class TbAnnualMeeting
    {
        public string Id { get; set; } = null!;

        /// <summary>標題（歷屆年會名稱）</summary>
        public string Title { get; set; } = null!;

        /// <summary>長內文（HTML，可含表格、文字、圖片，由 CKEditor 產生）</summary>
        public string? Contents { get; set; }

        /// <summary>排序（數字越小越前面）</summary>
        public int SortOrder { get; set; }

        /// <summary>附加檔案（對應 TbFileInfo.FileId）</summary>
        public string? FileId { get; set; }

        public bool IsDelete { get; set; }
        public bool IsPublish { get; set; }

        public string CreateUser { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public string? ModifyUser { get; set; }
        public DateTime? ModifyDate { get; set; }
    }
}
