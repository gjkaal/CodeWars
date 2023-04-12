using NUnit.Framework;
using System;
using System.Text;

namespace CodeWars
{
    internal static class BreadcrumbGenerator
    {
        private static readonly string[] IgnoreWords = new string[] { "the", "of", "in", "from", "by", "with", "and", "or", "for", "to", "at", "a" };
        private const string HomeSpan = "<span class=\"active\">HOME</span>";
        private const string HomeLink = "<a href=\"/\">HOME</a>";

        public static string GenerateBC(string url, string separator)
        {
            Console.WriteLine(url);

            var elements = FindPathElements(RemoveQueryString(url));
            if (elements.Length == 0) return HomeSpan;
            if (elements[elements.Length - 1].StartsWith("index", StringComparison.OrdinalIgnoreCase))
            {
                Array.Resize(ref elements, elements.Length - 1);
            }
            if (elements.Length <= 1) return HomeSpan;

            var sb = new StringBuilder(HomeLink);
            var lastElement = elements.Length - 1;
            var path = new StringBuilder();
            for (int i = 1; i < elements.Length; i++)
            {
                sb.Append(separator);
                var element = elements[i];
                if (i == lastElement)
                {
                    sb.Append($"<span class=\"active\">{FormatLabel(element)}</span>");
                }
                else
                {
                    path.Append($"/{element}");
                    sb.Append($"<a href=\"{path}/\">{FormatLabel(element)}</a>");
                }
            }
            return sb.ToString();
        }

        private static string[] FindPathElements(string url)
        {
            string[] elements;
            var n = url.IndexOf("//");
            if (n >= 0)
            {
                elements = url.Substring(n + 1).Split('/', StringSplitOptions.RemoveEmptyEntries);
            }
            else
            {
                elements = url.Split('/', StringSplitOptions.RemoveEmptyEntries);
            }

            return elements;
        }

        public static string FormatLabel(string value)
        {
            var s = RemoveExtension( value).Split('-');
            if (s.Length == 1) return s[0].ToUpperInvariant();
            if (value.Length > 30)
            {
                return GetAcronym(s);
            }
            else
            {
                return FormatText(s);
            }
        }

        private static string RemoveQueryString(string s)
        {
            var hyphen = s.LastIndexOf('/');
            if (hyphen < 0) return s;
            var end = s.IndexOfAny( new char[] { '#', '?' }, hyphen);

            return end>0 
                ? s.Substring(0, end)
                : s;                
        }

        private static string RemoveExtension(string s)
        {
            var end = s.LastIndexOf('.');
            return (end >= 0)
                ? s.Substring(0, end)
                : s;
        }

        private static string FormatText(string[] s)
        {
            var sb = new StringBuilder();
            var first = true;
            for (var i = 0; i < s.Length; i++)
            {
                var word = s[i];
                if (first)
                {
                    first = false;
                }
                else
                {
                    sb.Append(' ');
                }
                sb.Append(word.ToUpperInvariant());
            }
            return sb.ToString();
        }

        private static string GetAcronym(string[] s)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < s.Length; i++)
            {
                var word = s[i];
                if (IsIgnoreWord(word)) continue;
                sb.Append(char.ToUpperInvariant(s[i][0]));
            }
            return sb.ToString();
        }

        private static bool IsIgnoreWord(string value)
        {
            for (var x = 0; x < IgnoreWords.Length; x++)
            {
                if (IgnoreWords[x].Equals(value, StringComparison.OrdinalIgnoreCase)) return true;
            }
            return false;
        }
    }

    [TestFixture]
    public class BreadcrumbGeneratorTests
    {
        [TestCase("very-long-url-to-make-a-silly-yet-meaningful-example",  "VLUMSYME")]
        [TestCase("GiacomoSorbi", "GIACOMOSORBI")]
        [TestCase("giacomo-sorbi",  "GIACOMO SORBI")]
        [TestCase("uber-to-research-skin-with-at",  "UBER TO RESEARCH SKIN WITH AT")]
        public void TestFormatLabel(string value, string expected) => Assert.AreEqual(expected, BreadcrumbGenerator.FormatLabel(value));

        private readonly string[] urls = new string[] {"mysite.com/pictures/holidays.html",
                                          "www.codewars.com/users/GiacomoSorbi?ref=CodeWars",
                                          "www.microsoft.com/docs/index.htm#top",
                                          "mysite.com/very-long-url-to-make-a-silly-yet-meaningful-example/example.asp",
                                          "www.very-long-site_name-to-make-a-silly-yet-meaningful-example.com/users/giacomo-sorbi",
                                          "https://www.linkedin.com/in/giacomosorbi",
                                          "www.agcpartners.co.uk/",
                                          "www.agcpartners.co.uk",
                                          "https://www.agcpartners.co.uk/index.html",
                                          "http://www.agcpartners.co.uk"};

        private readonly string[] seps = new string[] { " : ", " / ", " * ", " > ", " + ", " * ", " * ", " # ", " >>> ", " % " };

        private readonly string[] anss = new string[] {"<a href=\"/\">HOME</a> : <a href=\"/pictures/\">PICTURES</a> : <span class=\"active\">HOLIDAYS</span>",
                                          "<a href=\"/\">HOME</a> / <a href=\"/users/\">USERS</a> / <span class=\"active\">GIACOMOSORBI</span>",
                                          "<a href=\"/\">HOME</a> * <span class=\"active\">DOCS</span>",
                                          "<a href=\"/\">HOME</a> > <a href=\"/very-long-url-to-make-a-silly-yet-meaningful-example/\">VLUMSYME</a> > <span class=\"active\">EXAMPLE</span>",
                                          "<a href=\"/\">HOME</a> + <a href=\"/users/\">USERS</a> + <span class=\"active\">GIACOMO SORBI</span>",
                                          "<a href=\"/\">HOME</a> * <a href=\"/in/\">IN</a> * <span class=\"active\">GIACOMOSORBI</span>",
                                          "<span class=\"active\">HOME</span>",
                                          "<span class=\"active\">HOME</span>",
                                          "<span class=\"active\">HOME</span>",
                                          "<span class=\"active\">HOME</span>"};

        //[TestCase("mysite.com/pictures/holidays.html", " : ", "<a href=\"/\">HOME</a> : <a href=\"/pictures/\">PICTURES</a> : <span class=\"active\">HOLIDAYS</span>")]
        //[TestCase("www.microsoft.com/docs/index.htm#top", " * ", "<a href=\"/\">HOME</a> * <span class=\"active\">DOCS</span>")]
        //[TestCase("www.agcpartners.co.uk/", " * ", "<span class=\"active\">HOME</span>")]
        //[TestCase("https://www.agcpartners.co.uk/index.html", " >>> ", "<span class=\"active\">HOME</span>")]
        //[TestCase("https://www.linkedin.com/in/giacomosorbi", " * ", "<a href=\"/\">HOME</a> * <a href=\"/in/\">IN</a> * <span class=\"active\">GIACOMOSORBI</span>")]
        //[TestCase("www.very-long-site_name-to-make-a-silly-yet-meaningful-example.com/users/giacomo-sorbi", " + ", "<a href=\"/\">HOME</a> + <a href=\"/users/\">USERS</a> + <span class=\"active\">GIACOMO SORBI</span>")]
        //[TestCase("http://pippi.pi/files/of-in-biotechnology-cauterization/", " + ", "<a href=\"/\">HOME</a> + <a href=\"/files/\">FILES</a> + <span class=\"active\">BC</span>")]
        [TestCase("facebook.fr/pippi-and-of-a-bioengineering-uber-at-and-to-research/#top?order=desc&filter=adult", " > ", "<a href=\"/\">HOME</a> > <span class=\"active\">PBUR</span>")]
        public void TestBreadcrumb(string value, string seperator, string expected) => Assert.AreEqual(expected, BreadcrumbGenerator.GenerateBC(value, seperator));

        [Test]
        public void ExampleTests()
        {
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine($"\nTest With: {urls[i]}");
                if (i == 5) Console.WriteLine(
                    "\nThe one used in the above test was my LinkedIn Profile; " +
                    "if you solved the kata this far and manage to get it, feel " +
                    "free to add me as a contact, message me about the language " +
                    "that you used and I will be glad to endorse you in that skill " +
                    "and possibly many others :)\n\n ");

                Assert.AreEqual(anss[i], BreadcrumbGenerator.GenerateBC(urls[i], seps[i]));
            }
        }
    }
}
