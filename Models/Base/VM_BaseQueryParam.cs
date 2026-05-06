namespace TADS_Web.Models.Base
{
    public class VM_BaseQueryParam
    {
        public VM_BaseQueryParam()
        {
            this.PagerInfo = new PagerInfo();
            this.PagerInfo.m_iPageCount = 10;
        }

        public string SortOrder { get; set; } = string.Empty;
        public PagerInfo PagerInfo { get; set; }
    }
}
