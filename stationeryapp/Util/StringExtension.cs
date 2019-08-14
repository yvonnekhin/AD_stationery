using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace stationeryapp.Util
{
    public static class StringExtension
    {
        public static string clean(string s)
        {
            StringBuilder sb = new StringBuilder(s);

            sb.Replace("&", "");
            sb.Replace("\"", "");
            sb.Replace(" ", "_");
            sb.Replace("(", "");
            sb.Replace(")", "");
            sb.Replace("/", "");

            return sb.ToString();
        }
    }
}