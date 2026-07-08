namespace TADS_Web.Models.DB
{
    /// <summary>
    /// 靜態內頁的可編輯內容（固定頁面集合，一頁一筆，以 PageCode 為主鍵）。
    /// </summary>
    public partial class TbPageContent
    {
        /// <summary>頁面代碼（主鍵）：Contact / DevelopmentDynamics / RelevantLinks / Newsletters</summary>
        public string PageCode { get; set; } = null!;

        /// <summary>後台清單顯示名稱（例：聯絡我們）</summary>
        public string PageName { get; set; } = null!;

        /// <summary>頁面內容（HTML，由 CKEditor 產生；前台以 @Html.Raw 輸出）</summary>
        public string? Contents { get; set; }

        /// <summary>附加檔案（對應 TbFileInfo.FileId；例：學位論文獎下載檔）</summary>
        public string? FileId { get; set; }

        public string? CreateUser { get; set; }
        public DateTime CreateDate { get; set; }
        public string? ModifyUser { get; set; }
        public DateTime? ModifyDate { get; set; }
    }
}
