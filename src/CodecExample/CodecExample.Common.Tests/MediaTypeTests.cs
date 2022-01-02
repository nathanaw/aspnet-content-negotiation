// Copyright (c) 2021 Nathan Allen-Wagner. All rights reserved.
// Licensed under the MIT license. See License.txt in root of repo.

using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CodecExample.Common.Tests
{

    // These tests help make sense of how the IsSubsetOf() works.
    [TestClass]
    public class MediaTypeTests
    {
        [TestMethod]
        [DataRow("application/json", "*/*", DisplayName = "1 - Wildcard type/subtype.")]
        [DataRow("application/json", "application/*", DisplayName = "2 - Wildcard subtype.")]
        [DataRow("application/json", "application/json", DisplayName = "3 - Type/Subtype match.")]
        [DataRow("application/json; domain=foo; version=1; charset=utf-8", "application/json; domain=foo; version=1", DisplayName = "4 - Param not on right.")]
        [DataRow("application/json; Domain=foo; Version=1; charset=utf-8", "application/json; domain=foo; version=1", DisplayName = "5 - Case of params does not matter.")]
        [DataRow("application/json; Domain=foo; Version=1; charset=utf-8", "application/json; domain=foo; version=1; charset=utf-8", DisplayName = "6 - Multiple params, all match.")]
        public void IsSubsetOfTests(string left, string right)
        {
            var leftMT = new MediaType(left);
            var rightMT = new MediaType(right);

            leftMT.IsSubsetOf(rightMT).Should().BeTrue();
        }

        [TestMethod]
        [DataRow("application/json", "application/xml", DisplayName = "1 - Different subtypes")]
        [DataRow("application/json; domain=foo; version=1", "application/json; domain=foo; version=1; charset=utf-8", DisplayName = "2 - Param not on left.")]
        [DataRow("application/json; Domain=foo; Version=2", "application/json; domain=foo; version=1; charset=utf-8", DisplayName = "3 - Different param values.")]
        [DataRow("application/json; Domain=foo; Version=1; charset=utf-8", "application/json; domain=foo; version=1; charset=utf-16", DisplayName = "4 - Different param values.")]
        [DataRow("application/json; Domain=foo; Version=1; charset=utf-8", "application/json; Domain=*; Version=1", DisplayName = "5 - Wildcards not supported in parameters.")]
        public void IsNotSubsetOfTests(string left, string right)
        {
            var leftMT = new MediaType(left);
            var rightMT = new MediaType(right);

            leftMT.IsSubsetOf(rightMT).Should().BeFalse();
        }
    }
}