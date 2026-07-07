namespace TADS_Web.Extensions
{
    /// <summary>數字格式化：中文數字（第 N 屆用）與英文序數（Nth 用）。</summary>
    public static class NumberFormat
    {
        /// <summary>阿拉伯數字轉中文數字（支援 1~99，足夠理監事屆數使用）。</summary>
        public static string ToChineseNumeral(int n)
        {
            if (n <= 0) return n.ToString();
            string[] digits = { "零", "一", "二", "三", "四", "五", "六", "七", "八", "九" };
            if (n < 10) return digits[n];
            if (n < 20) return n == 10 ? "十" : "十" + digits[n % 10];
            if (n < 100)
            {
                int tens = n / 10, ones = n % 10;
                return digits[tens] + "十" + (ones == 0 ? "" : digits[ones]);
            }
            return n.ToString();
        }

        /// <summary>阿拉伯數字轉英文序數字（1 -> 1st、2 -> 2nd、7 -> 7th）。</summary>
        public static string ToOrdinal(int n)
        {
            int mod100 = n % 100;
            if (mod100 >= 11 && mod100 <= 13) return n + "th";
            return (n % 10) switch
            {
                1 => n + "st",
                2 => n + "nd",
                3 => n + "rd",
                _ => n + "th"
            };
        }
    }
}
