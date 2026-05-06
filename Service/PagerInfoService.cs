using Microsoft.EntityFrameworkCore;
using TADS_Web.Models;

namespace TADS_Web.Service
{
    [Serializable]
    public static class PagerInfoService
    {
        public static async Task<List<T>?> GetRange<T>(IQueryable<T> _modules, PagerInfo pager) where T : class
        {
            if (_modules == null)
                return new List<T>();

            try
            {
                if (pager.m_Search == true)
                    pager.SetDedault();
                else
                    pager.m_Search = true;

                pager.m_iDataCount = _modules.Count();
                pager.m_iPageTotal = pager.m_iDataCount / pager.m_iPageCount;

                if ((pager.m_iDataCount % pager.m_iPageCount) > 0)
                    pager.m_iPageTotal += 1;

                if (pager.m_iPageIndex == 0)
                    pager.m_iPageIndex = 1;

                pager.m_iPrePage = pager.m_iPageIndex - 1;
                pager.m_iNextPage = pager.m_iPageIndex + 1;

                if (pager.m_iPageIndex >= pager.m_iPageTotal)
                {
                    pager.m_iPageIndex = pager.m_iPageTotal;
                    pager.m_iNextPage = pager.m_iPageIndex;
                }

                if (pager.m_iPrePage < 1)
                    pager.m_iPrePage = 1;

                Int32 firstIndex = (pager.m_iPageIndex - 1) * pager.m_iPageCount;

                if (pager.m_iPageTotal == 0)
                {
                    pager.m_iPageTotal = 1;
                    firstIndex = 0;
                }

                try
                {
                    return await _modules.Skip(firstIndex).Take(pager.m_iPageCount).ToListAsync();
                }
                catch
                {
                    return _modules.ToList().Skip(firstIndex).Take(pager.m_iPageCount).ToList();
                }
            }
            catch
            {
                return null;
            }
        }
    }
}
