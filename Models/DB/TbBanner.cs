namespace TADS_Web.Models.DB
{
    public partial class TbBanner
    {
        public string Id { get; set; } = null!;

        /// <summary>橫幅圖片（對應 TbFileInfo）</summary>
        public string? FileId { get; set; }

        /// <summary>標題／圖片替代文字（alt）</summary>
        public string? Title { get; set; }

        /// <summary>點擊導向網址（留空則不可點擊）</summary>
        public string? LinkUrl { get; set; }

        /// <summary>排序（數字越小越前面，同時決定輪播切換順序）</summary>
        public int SortOrder { get; set; }

        public bool IsDelete { get; set; }
        public bool IsPublish { get; set; }

        public string CreateUser { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public string? ModifyUser { get; set; }
        public DateTime? ModifyDate { get; set; }
    }
}
