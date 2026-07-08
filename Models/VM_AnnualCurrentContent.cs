namespace TADS_Web.Models
{
    /// <summary>
    /// 本屆年會共用區塊內容（首頁與 /Annual/Current 共用同一份資料），
    /// 三個區塊各對應 TbPageContent 一筆（AnnualCurrent_Intro / AnnualCurrent_Topics / AnnualCurrent_Award），
    /// 由後台「內頁管理」編輯。
    /// </summary>
    public class VM_AnnualCurrentContent
    {
        /// <summary>最上方介紹（年會主題、日期、主辦單位）</summary>
        public string? IntroHtml { get; set; }

        /// <summary>A. 徵稿主題介紹</summary>
        public string? TopicsHtml { get; set; }

        /// <summary>B. 學位論文獎</summary>
        public string? AwardHtml { get; set; }

        /// <summary>學位論文獎下載檔網址（獨立上傳欄位；無上傳時前台退回預設檔 /static/file.pdf）</summary>
        public string? AwardFileUrl { get; set; }

        /// <summary>學位論文獎下載檔原始檔名（後台顯示用）</summary>
        public string? AwardFileName { get; set; }

        /// <summary>是否顯示「C. 歷屆年會」固定區塊（僅 /Annual/Current 顯示，首頁不顯示）</summary>
        public bool ShowHistoryBlock { get; set; }
    }
}
