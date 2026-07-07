namespace TADS_Web.Models.DB
{
    /// <summary>
    /// 「關於我們」會費價目表卡片（固定 6 張，以 SortOrder 對應版面上的固定色帶/位置）。
    /// 分類色帶（入會費/年度會費/終身會費）與顏色固定於樣板，僅名稱/價格/中英說明開放編輯。
    /// </summary>
    public partial class TbAboutPricing
    {
        public string Id { get; set; } = null!;

        /// <summary>排序（1~6，對應樣板上的固定卡片位置）</summary>
        public int SortOrder { get; set; }

        /// <summary>會員別 / 名稱（中文）</summary>
        public string? NameZh { get; set; }
        /// <summary>會員別 / 名稱（英文）</summary>
        public string? NameEn { get; set; }
        /// <summary>金額（純數字字串，例：1,000；「NTD$」與期間文字固定於樣板）</summary>
        public string? Price { get; set; }
        /// <summary>說明（中文）</summary>
        public string? DescZh { get; set; }
        /// <summary>說明（英文）</summary>
        public string? DescEn { get; set; }
    }
}
