using NUnit.Framework;

namespace KindBot.Tools.Tests
{
    [TestFixture]
    public class ExtensionMethodsTests
    {
        [TestCase("asdf 123", ExpectedResult = @"asdf\s123")]
        [TestCase("1234", ExpectedResult = "1234")]
        [TestCase("|asd|", ExpectedResult = @"\pasd\p")]
        [TestCase(@"1\qwert", ExpectedResult = @"1\\qwert")]
        [TestCase(@"@/", ExpectedResult = @"@\/")]
        public string ConvertToTeamspeakStringTest(string testCase) => testCase.ConvertToTeamspeakString();

        [TestCase(@"asdf\s123", ExpectedResult = "asdf 123")]
        [TestCase("1234", ExpectedResult = "1234")]
        [TestCase(@"\pasd\p", ExpectedResult = "|asd|")]
        [TestCase(@"1\\qwert", ExpectedResult = @"1\qwert")]
        [TestCase(@"@\/", ExpectedResult = @"@/")]
        public string ConvertTeamspeakToNormalTest(string testCase) => testCase.ConvertTeamspeakToNormal();
    }
}