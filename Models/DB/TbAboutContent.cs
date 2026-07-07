namespace TADS_Web.Models.DB
{
    /// <summary>
    /// 「關於我們」頁面內容（單筆記錄，Id 固定為 "About"）。每個區塊拆成獨立欄位供後台編輯。
    /// 圖片 / PDF 以 FileId 參照 TbFileInfo；理監事名單、章程、以文會友說明為純文字。
    /// </summary>
    public partial class TbAboutContent
    {
        public string Id { get; set; } = null!;

        // === Introduce 介紹 ===
        /// <summary>學會名稱（中文）</summary>
        public string? IntroNameZh { get; set; }
        /// <summary>學會名稱（英文）</summary>
        public string? IntroNameEn { get; set; }
        /// <summary>左側簡介短文</summary>
        public string? IntroSummary { get; set; }
        /// <summary>右側詳細內文（以空行分段）</summary>
        public string? IntroDetail { get; set; }
        /// <summary>介紹圖片（FileId，上傳覆蓋）</summary>
        public string? IntroImageFileId { get; set; }

        // === Organizational Chart 組織圖 ===
        /// <summary>組織結構圖（FileId，上傳覆蓋）</summary>
        public string? OrgChartFileId { get; set; }
        /// <summary>理監事屆數（第 N 屆）</summary>
        public int? BoardTermNo { get; set; }
        public string? BoardPresident { get; set; }
        public string? BoardVicePresident { get; set; }
        public string? BoardExecDirectors { get; set; }
        public string? BoardDirectors { get; set; }
        public string? BoardExecSupervisor { get; set; }
        public string? BoardSupervisors { get; set; }
        public string? BoardSecretaryGeneral { get; set; }
        public string? BoardExecSecretary { get; set; }
        /// <summary>歷屆理監事名單下載連結</summary>
        public string? PastBoardListUrl { get; set; }

        // === Membership Application 入會申請 ===
        public string? MembershipInviteZh { get; set; }
        public string? MembershipInviteEn { get; set; }

        // === Constitution 章程 ===
        /// <summary>章程內文（純文字，保留換行）</summary>
        public string? ConstitutionText { get; set; }
        /// <summary>章程下載檔（FileId，上傳覆蓋）</summary>
        public string? ConstitutionPdfFileId { get; set; }

        // === Member Research 以文會友 ===
        /// <summary>以文會友說明文字（純文字）</summary>
        public string? MemberResearchDesc { get; set; }

        public string? CreateUser { get; set; }
        public DateTime CreateDate { get; set; }
        public string? ModifyUser { get; set; }
        public DateTime? ModifyDate { get; set; }
    }
}
