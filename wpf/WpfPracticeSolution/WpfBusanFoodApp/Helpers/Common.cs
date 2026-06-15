using NLog;
using System.Net;
using System.Text.RegularExpressions;

namespace WpfBusanFoodApp.Helpers
{
    // 여러 화면이나 클래스에서 공통으로 사용할 기능.
    public class Common
    {

        // Common.Logger.Info("메시지")
        public static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        // API 상세 설명에 포함된 HTML 태그를 일반 텍스트로 바꾸는 메서드.
        public static string ConvertHtmlToText(string? html)
        {
            if (string.IsNullOrWhiteSpace(html))
            {
                return string.Empty;
            }

            // <br>, <br/>, <BR> 태그를 줄바꿈으로 변경
            string text = Regex.Replace(html, "<br\\s*/?>", Environment.NewLine, RegexOptions.IgnoreCase);

            // </p> 태그를 줄바꿈으로 변경
            text = Regex.Replace(text, "</p>", Environment.NewLine, RegexOptions.IgnoreCase);

            // 나머지 HTML 태그 제거
            text = Regex.Replace(text, "<.*?>", "", RegexOptions.IgnoreCase);

            // &nbsp; 같은 HTML 특수문자를 일반 문자로 변환
            text = WebUtility.HtmlDecode(text);

            return text.Trim();
        }
    }
}