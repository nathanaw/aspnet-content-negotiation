// Copyright (c) 2021 Nathan Allen-Wagner. All rights reserved.
// Licensed under the MIT license. See License.txt in root of repo.

using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CodecExample.Common;
using Microsoft.Net.Http.Headers;

namespace CodecExample.Common.Tests
{

    // These tests help make sense of how the IsSubsetOf() works.
    [TestClass]
    public class MediaTypeHeaderValueSupportTests
    {
        [TestMethod]
        [DataRow("application/json", "*/*", DisplayName = "01 - Wildcard type/subtype.")]
        [DataRow("application/json", "application/*", DisplayName = "02 - Wildcard subtype.")]
        [DataRow("application/json", "application/json", DisplayName = "03 - Type/Subtype match.")]
        [DataRow("application/json; domain=foo; version=1", "application/json; domain=foo; version=1; pretty=true", DisplayName = "04 - Param not on left.")]
        [DataRow("application/json; Domain=foo; Version=1", "application/json; domain=foo; version=1", DisplayName = "05 - Case of params names does not matter.")]
        [DataRow("application/json; domain=foo; version=1", "application/json; domain=foo; version=1", DisplayName = "06 - Multiple params, all match.")]
        [DataRow("application/json; domain=foo; version=1", "application/json; domain=foo; version=1; charset=utf-8", DisplayName = "07 - Charset on right excluded from compare.")]
        [DataRow("application/json; domain=foo; version=1; charset=utf-8", "application/json; domain=foo; version=1", DisplayName = "08 - Charset on left excluded from compare.")]
        [DataRow("application/json; domain=foo; version=1; charset=utf-8", "application/json; domain=foo; version=1; charset=utf-16", DisplayName = "09 - Charset excluded even if values differ.")]
        [DataRow("application/json; domain=foo; version=1; q=0.5", "application/json; domain=foo; version=1; q=0.9", DisplayName = "10 - Q excluded even if values differ.")]
        [DataRow("application/json;    domain=foo;version=1", "application/json; domain=foo; version=1", DisplayName = "11 - Extra spaces do not matter.")]
        public void IsSubsetOfTests(string left, string right)
        {
            var leftMT = MediaTypeHeaderValue.Parse(left);
            var rightMT = MediaTypeHeaderValue.Parse(right);

            MediaTypeHeaderValueSupport.IsSubsetOf(leftMT, rightMT).Should().BeTrue();
        }

        [TestMethod]
        [DataRow("application/json", "application/xml", DisplayName = "01 - Different subtypes")]
        [DataRow("application/json; Domain=foo; Version=2", "application/json; domain=foo; version=1", DisplayName = "02 - Different param values.")]
        [DataRow("application/json; Domain=foo; Version=1", "application/json; Domain=*; Version=1", DisplayName = "03 - Wildcards not supported in parameters.")]
        [DataRow("application/json; Domain=foo; Version=1;", "application/json; Domain=BAR; Version=2", DisplayName = "04 - Trailing semicolon on left breaks parser.")]
        [DataRow("application/json; Domain=foo; Version=1", "application/json; Domain=BAR; Version=2;", DisplayName = "05 - Trailing semicolon on right breaks parser.")]
        [DataRow("application/json; domain=foo; version=1", "application/json; domain=FOO; version=1", DisplayName = "06 - Case of params values does matter.")]
        [DataRow("application/*", "application/json", DisplayName = "07 - Wildcards on left are not subset.")]
        public void IsNotSubsetOfTests(string left, string right)
        {
            var leftMT =  MediaTypeHeaderValue.Parse(left);
            var rightMT = MediaTypeHeaderValue.Parse(right);

            MediaTypeHeaderValueSupport.IsSubsetOf(leftMT, rightMT).Should().BeFalse();
        }
    }
}