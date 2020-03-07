﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using DotnetSpider.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotnetSpider.Common.Tests
{
    [TestClass()]
    public class HashCodeTests
    {
        [TestMethod()]
        public void GetHashCodeTest0()
        {
            int expected = 0;
            int actual = HashCode.GetHashCode(0, 0);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void GetHashCodeTest1()
        {
            int expected = 84696351;
            int actual = HashCode.GetHashCode(HashCode.BeginCode, null);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void GetHashCodeTest2()
        {
            int expected = 1539727482;
            int actual = HashCode.GetHashCode(HashCode.BeginCode, new Dictionary<int, int>() { { 0, 0} });
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void GetHashCodeTest3()
        {
            int expected = -2079276838;
            int actual = HashCode.GetHashCode(HashCode.BeginCode, new Dictionary<int, int>());
            Assert.AreEqual(expected, actual);
        }
    }
}