// Principles:
// - Not using regex for performance issues with regex.
// - Not using linq out of principle, and having the exercise.
// - explicit types, although you could probably use 'var' on most variables.
using NUnit.Framework;
using System;

namespace CodeWars
{
    public class StripCommentsTest
    {
        [Test]
        public void StripComments()
        {
            Assert.AreEqual(
                    "apples, pears\ngrapes\nbananas",
                    StripCommentsSolution.StripComments("apples, pears # and bananas\ngrapes\nbananas !apples", new string[] { "#", "!" }));

            Assert.AreEqual("a\nc\nd", StripCommentsSolution.StripComments("a #b\nc\nd $e f g", new string[] { "#", "$" }));
        }

        [Test]
        public void Edged()
        {
            var egdeCase = "a \n b \nc ";
            var actual = StripCommentsSolution.StripComments(egdeCase, new string[] { "#", "$" });
            Assert.AreEqual("a\n b\nc", actual);
        }
    }

    public class StripCommentsSolution
    {
        public static string StripComments(string text, string[] commentSymbols)
        {            
            char[] symbols = ValidateSymbols(commentSymbols);
            bool inComment = false;
            int p = 0;
            char[] result = new char[text.Length];
            foreach (char c in text)
            {
                if (c == '\n')
                {
                    // move back pointer to non space character
                    while (p > 0 && result[p - 1] == ' ') p--;
                }
                if (inComment)
                {
                    if (c == '\n')
                    {
                        inComment = false;
                        result[p++] = '\n';
                    }
                }
                else
                {
                    if (Contains(symbols, c))
                    {                        
                        inComment = true;
                    }
                    else
                    {
                        result[p++] = c;
                    }
                }
            }

            // remove trailing space character
            while (p > 0 && result[p - 1] == ' ') p--;
            return new string(result, 0, p);
        }

        private static bool Contains(char[] symbols, char c)
        {
            for (int x = 0; x < symbols.Length; x++)
            {
                if (symbols[x] == c) return true;
            }
            return false;
        }

        private static char[] ValidateSymbols(string[] commentSymbols)
        {
            var result = new char[commentSymbols.Length];
            for (int x = 0; x < commentSymbols.Length; x++)
            {
                var s = commentSymbols[x];
                if (s.Length != 1)
                {
                    throw new ArgumentOutOfRangeException($"Symbol list contains a string, which is not a symbol: {s}");
                }
                result[x] = s[0];
            }
            return result;
        }
    }
}
