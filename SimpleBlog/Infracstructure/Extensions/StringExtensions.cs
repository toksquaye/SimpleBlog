using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;

namespace SimpleBlog.Infracstructure.Extensions
{
    public static class StringExtensions
    {
        public static string Slugify(this string that)
        {
            that = Regex.Replace(that, @"[^a-zA-Z0-9\s]", "");
            that = that.ToLower();
            that = Regex.Replace(that, @"\s", "-");
            return that;

        }
    }
}