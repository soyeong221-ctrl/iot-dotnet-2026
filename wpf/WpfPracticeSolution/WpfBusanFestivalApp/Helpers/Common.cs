using NLog;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace WpfBusanFestivalApp.Helpers
{
    public class Common
    {
        // 전체 프로젝트에서 사용할 수 있는 NLog 객체
        public static readonly Logger logger = LogManager.GetCurrentClassLogger();


        // 불필요한 HTML 태그 삭제하는 메서드
        public static string ConvertHtmlToText(string? html)
        {
            if (string.IsNullOrWhiteSpace(html)) return string.Empty;

            string text = Regex.Replace(html, "<br\\s*/?>", Environment.NewLine, RegexOptions.IgnoreCase);   // <BR> <br> <BR/> <br/>
            text = Regex.Replace(text, "</p>", Environment.NewLine, RegexOptions.IgnoreCase);
            text = Regex.Replace(text, "<.*?>", "", RegexOptions.IgnoreCase); // <p> <html> <body> <h1> ... 등 모든 태그 전부 삭제

            return WebUtility.HtmlDecode(text).Trim();
        }
    }
}
