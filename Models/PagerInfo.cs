namespace TADS_Web.Models
{
    [Serializable]
    public class PagerInfo
    {
        public PagerInfo()
        {
            m_iPageIndex = 1;
            m_iPageCount = 30;
            m_iDataCount = 1;
            m_iPageRange = 4;
            this.SetPageCountList();
        }

        public PagerInfo(int pageCount)
        {
            m_iPageIndex = 1;
            m_iPageCount = pageCount;
            m_iDataCount = 1;
            m_iPageRange = 4;
            this.SetPageCountList();
        }

        private void SetPageCountList()
        {
            m_iPageCountList = new List<int> { 5, 10, 15, 30, 50, 100 };
            m_iPageCountList.Sort();
        }

        public void SetDedault()
        {
            m_iPageIndex = 1;
        }

        public int m_iPageIndex { get; set; }
        public int m_iPageCount { get; set; }
        public int m_iDataCount { get; set; }
        public int m_iPageTotal { get; set; }
        public int m_iPrePage { get; set; }
        public int m_iNextPage { get; set; }
        public List<int> m_iPageCountList { get; set; } = new();
        public bool m_Search { get; set; }
        public int m_iPageRange { get; set; }

        public int m_iPageMin => (m_iPageIndex - m_iPageRange >= 1) ? m_iPageIndex - m_iPageRange : 1;
        public int m_iPageMax => (m_iPageIndex + m_iPageRange <= m_iPageTotal) ? m_iPageIndex + m_iPageRange : m_iPageTotal;
        public bool isShowFirst => (m_iPageIndex != 1 && m_iDataCount > 0);
        public bool isShowPrevious => (m_iPageIndex > 1 && m_iDataCount > 0);
        public bool isShowNext => (m_iPageIndex < m_iPageTotal && m_iDataCount > 0);
        public bool isShowLast => (m_iPageIndex != m_iPageTotal && m_iDataCount > 0);
    }
}
