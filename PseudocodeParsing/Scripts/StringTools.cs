using System;

namespace fuliu.pseudocode
{
    public static class StringTools
    {
        public static int ContainsCount(this string str, string seperater)
        {
            var ss = str.Split(new []{seperater}, StringSplitOptions.None);
            return ss.Length - 1;
        }
        
        public static int ContainsCount(this string str, char seperater)
        {
            var ss = str.Split(new []{seperater}, StringSplitOptions.None);
            return ss.Length - 1;
        }
        
        /// <summary>
        /// 返回被第一次出现的符号 firstSymbol 和最后一次出现的符号 endSymbol 截取的中间一段字符串
        /// </summary>
        public static string SplitByFirstAndLastSymbol(this string str, string firstSymbol, string endSymbol)
        {
            var s = str.Substring(str.IndexOf(firstSymbol) + 1);
            return s.Substring(0, s.LastIndexOf(endSymbol));
        }
    }
}