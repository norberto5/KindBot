using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace KindBot.Tools.Tests
{
    [TestFixture]
    public class TeamspeakToolsTests
    {
        [TestCase(@"id=123", "id", 123)]
        [TestCase(@"test=asdf\s1234", "test", "asdf 1234")]
        [TestCase(@"flag=1", "flag", true)]
        [TestCase(@"flag=0", "flag", false)]
        [TestCase(@"cid=92 client_idle_time=830580 client_version=3.1.7\s[Build:\s666] client_input_hardware=1", "cid", 92)]
        [TestCase(@"cid=92 client_idle_time=830580 client_version=3.1.7\s[Build:\s666] client_input_hardware=1", "client_idle_time", 830580)]
        [TestCase(@"cid=92 client_idle_time=830580 client_version=3.1.7\s[Build:\s666] client_input_hardware=1", "client_version", "3.1.7 [Build: 666]")]
        [TestCase(@"cid=92 client_idle_time=830580 client_version=3.1.7\s[Build:\s666] client_input_hardware=1", "client_input_hardware", true)]
        public void GetParameterTest<T>(string output, string param, T expectedResult)
        {
            T result = TeamspeakTools.GetParameter<T>(output, param);
            Assert.AreEqual(result, expectedResult);
        }

        private static readonly object[] expectedResultsOfGetListOfGroupsTest =
        {
            new object[]
            {
                "5,13,23", new List<int>() { 5, 13, 23 }
            },
            new object[]
            {
                "asdf", new List<int>()
            },
            new object[]
            {
                "asdf,123456,3", new List<int>() { 123456, 3 }
            }
        };

        [Test, TestCaseSource("expectedResultsOfGetListOfGroupsTest")]
        public void GetListOfGroupsTest(string testCase, List<int> expectedResults)
        {
            IEnumerable<int> result = TeamspeakTools.GetListOfGroups(testCase);
            Assert.IsTrue(Enumerable.SequenceEqual(result, expectedResults));
        }
    }
}