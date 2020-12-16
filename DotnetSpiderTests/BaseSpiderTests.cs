﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using DotnetSpider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.QualityTools.Testing.Fakes;
using System.Threading;
using System.Diagnostics.CodeAnalysis;

namespace DotnetSpider.Tests
{
    [ExcludeFromCodeCoverage]
    [TestClass()]
    public class BaseSpiderTests
    {
        #region 全局设定
        private IDisposable _shimsContext = null;
        private Fakes.StubBaseSpider _instance = null;
        private Fakes.ShimBaseSpider _instanceShim = null;
        private PrivateObject _private = null;
        private PrivateType _type = null;

        [TestInitialize]
        public void Init()
        {
            _shimsContext = ShimsContext.Create();
            _instance = new Fakes.StubBaseSpider
            {
                CallBase = true
            };
            _instanceShim = new Fakes.ShimBaseSpider(_instance);
            _type = new PrivateType(typeof(BaseSpider));
            _private = new PrivateObject(_instance, _type);
        }

        [TestCleanup]
        public void Clean()
        {
            _shimsContext.Dispose();
            _shimsContext = null;
            _type = null;
            _private = null;
            _instanceShim = null;
            _instance.Dispose();
            _instance = null;
        }
        #endregion

        #region 实现接口的测试
        [TestMethod()]
        public void ContinueTest0()
        {
            _private.SetField("_hasExit", true);
            _private.SetField("_isRunning", false);
            Assert.IsFalse(_instance.Continue());
            Assert.IsFalse((bool)_private.GetField("_isRunning"));
        }

        [TestMethod()]
        public void ContinueTest1()
        {
            _private.SetField("_hasExit", false);
            _private.SetField("_isRunning", true);
            Assert.IsFalse(_instance.Continue());
            Assert.IsTrue((bool)_private.GetField("_isRunning"));
        }

        [TestMethod()]
        public void ContinueTest2()
        {
            _private.SetField("_hasExit", false);
            _private.SetField("_isRunning", false);
            Assert.IsTrue(_instance.Continue());
            Assert.IsTrue((bool)_private.GetField("_isRunning"));
        }

        [TestMethod()]
        public async Task ContinueAsyncTest0()
        {
            int callTimes = 0;
            _instanceShim.Continue = () => 
            { 
                ++callTimes; 
                return false; 
            };
            Assert.IsFalse(await _instance.ContinueAsync());
            Assert.AreEqual(1, callTimes);
        }

        [TestMethod()]
        [Timeout(5000)]
        public void ExitTest0()
        {
            _private.SetField("_hasExit", true);
            _instance.Scheduler = new Scheduler.Fakes.StubIScheduler()
            {
                Clear = Assert.Fail
            };
            Assert.IsFalse(_instance.Exit());
            Assert.IsTrue((bool)_private.GetField("_hasExit"));
        }

        [TestMethod()]
        [Timeout(5000)]
        public void ExitTest1()
        {
            _private.SetField("_hasExit", false);
            int called = 0;
            _instance.Scheduler = new Scheduler.Fakes.StubIScheduler()
            {
                Clear = () => ++called
            };
            Assert.IsTrue(_instance.Exit());
            Assert.AreEqual(1, called);
            Assert.IsTrue((bool)_private.GetField("_hasExit"));
        }

        [TestMethod()]
        [Timeout(5000)]
        public void ExitTest2()
        {
            _private.SetField("_hasExit", false);
            Assert.IsTrue(_instance.Exit());
            Assert.IsTrue((bool)_private.GetField("_hasExit"));
        }

        [TestMethod()]
        [Timeout(5000)]
        public async Task ExitAsyncTest0()
        {
            int callTimes = 0;
            _instanceShim.Exit = () =>
            {
                ++callTimes;
                return false;
            };
            Assert.IsFalse(await _instance.ExitAsync());
            Assert.AreEqual(1, callTimes);
        }

        [TestMethod()]
        public void PauseTest0()
        {
            _private.SetField("_hasExit", true);
            _private.SetField("_isRunning", false);
            Assert.IsFalse(_instance.Pause());
            Assert.IsFalse((bool)_private.GetField("_isRunning"));
        }

        [TestMethod()]
        public void PauseTest1()
        {
            _private.SetField("_hasExit", false);
            _private.SetField("_isRunning", true);
            Assert.IsTrue(_instance.Pause());
            Assert.IsFalse((bool)_private.GetField("_isRunning"));
        }

        [TestMethod()]
        public void PauseTest2()
        {
            _private.SetField("_hasExit", false);
            _private.SetField("_isRunning", false);
            Assert.IsFalse(_instance.Pause());
            Assert.IsFalse((bool)_private.GetField("_isRunning"));
        }

        [TestMethod()]
        [Timeout(5000)]
        public async Task PauseAsyncTest0()
        {
            int callTimes = 0;
            _instanceShim.Pause = () =>
            {
                ++callTimes;
                return false;
            };
            Assert.IsFalse(await _instance.PauseAsync());
            Assert.AreEqual(1, callTimes);
        }

        [TestMethod()]
        public void RunTest0()
        {
            _instanceShim.InitLogger = Assert.Fail;
            _instanceShim.InitSpider = Assert.Fail;
            _instanceShim.Exit = () =>
            {
                Assert.Fail();
                return false;
            };
            _instanceShim.CheckConfiguration = () => 
            { 
                Assert.Fail();
                return false; 
            };
            _private.SetField("_hasStarted", true);
            _instance.Run();
            Assert.IsFalse((bool)_private.GetField("_isRunning"));
        }

        [TestMethod()]
        public void RunTest1()
        {
            int calledIndex = 0;
            _instanceShim.InitLogger = () =>
            {
                Assert.AreEqual(2, ++calledIndex);
                Assert.IsTrue((bool)_private.GetField("_isRunning"));
            };
            _instanceShim.InitSpider = () =>
            {
                Assert.AreEqual(1, ++calledIndex);
                Assert.IsTrue((bool)_private.GetField("_isRunning"));
            };
            _instanceShim.Exit = () =>
            {
                Assert.AreEqual(4, ++calledIndex);
                Assert.IsTrue((bool)_private.GetField("_isRunning"));
                _private.SetField("_isRunning", false);
                return true;
            };
            _instanceShim.CheckConfiguration = () =>
            {
                Assert.AreEqual(3, ++calledIndex);
                Assert.IsTrue((bool)_private.GetField("_isRunning"));
                return false;
            };
            _instance.Run();
            Assert.IsFalse((bool)_private.GetField("_isRunning"));
            Assert.AreEqual(4, calledIndex);
        }

        [TestMethod()]
        public void RunTest2()
        {
            int calledIndex = 0;
            _instanceShim.InitLogger = () =>
            {
                Assert.AreEqual(2, ++calledIndex);
                Assert.IsTrue((bool)_private.GetField("_isRunning"));
            };
            _instanceShim.InitSpider = () =>
            {
                Assert.AreEqual(1, ++calledIndex);
                Assert.IsTrue((bool)_private.GetField("_isRunning"));
            };
            _instanceShim.Exit = () =>
            {
                Assert.AreEqual(4, ++calledIndex);
                Assert.IsTrue((bool)_private.GetField("_isRunning"));
                _private.SetField("_isRunning", false);
                return true;
            };
            _instanceShim.CheckConfiguration = () =>
            {
                Assert.AreEqual(3, ++calledIndex);
                Assert.IsTrue((bool)_private.GetField("_isRunning"));
                return false;
            };
            _instance.Run();
            Assert.IsFalse((bool)_private.GetField("_isRunning"));
            Assert.AreEqual(4, calledIndex);
        }

        [TestMethod()]
        public void RunTest3()
        {
            Assert.AreEqual(1, _instance.Parallels);
            int calledIndex = 0;
            _instanceShim.InitLogger = () =>
            {
                Assert.AreEqual(2, ++calledIndex);
                Assert.IsTrue((bool)_private.GetField("_isRunning"));
            };
            _instanceShim.InitSpider = () =>
            {
                Assert.AreEqual(1, ++calledIndex);
                Assert.IsTrue((bool)_private.GetField("_isRunning"));
            };
            _instanceShim.CheckConfiguration = () =>
            {
                Assert.AreEqual(3, ++calledIndex);
                Assert.IsTrue((bool)_private.GetField("_isRunning"));
                return true;
            };
            _instance.RunSpiderThread01 = () =>
            {
                Assert.AreEqual(4, ++calledIndex);
                Thread.Sleep(50);
            };
            _instanceShim.Exit = () =>
            {
                Assert.AreEqual(5, ++calledIndex);
                Assert.IsTrue((bool)_private.GetField("_isRunning"));
                _private.SetField("_isRunning", false);
                return true;
            };
            _instance.Run();
            Assert.IsFalse((bool)_private.GetField("_isRunning"));
            Assert.AreEqual(5, calledIndex);
        }


        [TestMethod()]
        public void RunTest4()
        {
            Assert.AreEqual(1, _instance.Parallels);
            int calledIndex = 0;
            string[] logs = new string[] { "Spider start.", "Spider exit." };
            int logIndex = 0;
            _instanceShim.InitLogger = () =>
            {
                Assert.AreEqual(2, ++calledIndex);
                Assert.IsTrue((bool)_private.GetField("_isRunning"));
                _instance.Logger = new log4net.Fakes.StubILog()
                {
                    InfoObject = o =>
                    {
                        string msg = o as string;
                        Assert.IsTrue(logIndex < logs.Length);
                        Assert.AreEqual(logs[logIndex++], msg);
                    }
                };
            };
            _instanceShim.InitSpider = () =>
            {
                Assert.AreEqual(1, ++calledIndex);
                Assert.IsTrue((bool)_private.GetField("_isRunning"));
            };
            _instanceShim.CheckConfiguration = () =>
            {
                Assert.AreEqual(3, ++calledIndex);
                Assert.IsTrue((bool)_private.GetField("_isRunning"));
                return true;
            };
            _instance.ConditionOfStop = _ =>
            {
                Assert.Fail();
                return false;
            };
            _instanceShim.StartThreadThreadStart = _ => Assert.AreEqual(4, ++calledIndex);
            _instance.RunSpiderThread01 = () =>
            {
                Assert.AreEqual(5, ++calledIndex);
                Thread.Sleep(50);
            };
            _instanceShim.Exit = () =>
            {
                Assert.AreEqual(6, ++calledIndex);
                Assert.IsTrue((bool)_private.GetField("_isRunning"));
                _private.SetField("_isRunning", false);
                return true;
            };
            _instance.Run();
            Assert.IsFalse((bool)_private.GetField("_isRunning"));
            Assert.AreEqual(6, calledIndex);
            Assert.AreEqual(logs.Length, logIndex);
        }

        [TestMethod()]
        [Timeout(5000)]
        public async Task RunAsyncTest()
        {
            int callTimes = 0;
            _instanceShim.Run = () => ++callTimes;
            await _instance.RunAsync();
            Assert.AreEqual(1, callTimes);
        }
        #endregion

        #region 可以在派生类中重新实现的函数的测试
        [TestMethod]
        public void DisposeOthersTest0()
        {
            int callTimes = 0;
            _instanceShim.Exit = () =>
            {
                Assert.AreEqual(1, ++callTimes);
                return true;
            };
            _private.Invoke("DisposeOthers");
            Assert.AreEqual(1, callTimes);
        }

        [TestMethod]
        public void DisposeOthersTest1()
        {
            int callTimes = 0;
            _instanceShim.Exit = () =>
            {
                Assert.AreEqual(1, ++callTimes);
                return true;
            };
            _instance.Scheduler = new Scheduler.Fakes.StubIScheduler
            {
                Dispose = () => Assert.AreEqual(2, ++callTimes)
            };
            _instance.Downloader = new Downloader.Fakes.StubIDownloader
            {
                Dispose = () => Assert.AreEqual(3, ++callTimes)
            };
            _instance.Pipelines.Add(new Pipeline.Fakes.StubIPipeline
            {
                Dispose = () => Assert.AreEqual(4, ++callTimes)
            });
            _instance.PageProcessors.Add(new Processor.Fakes.StubIResponseProcessor
            {
                Dispose = () => Assert.AreEqual(5, ++callTimes)
            });
            _instance.HttpProxy = new Proxy.Fakes.StubIHttpProxy
            {
                Dispose = () => Assert.AreEqual(6, ++callTimes)
            };
            _private.Invoke("DisposeOthers");
            Assert.AreEqual(6, callTimes);
        }

        [TestMethod]
        public void InitSpiderTest0()
        {
            _private.Invoke("InitSpider");
        }

        [TestMethod]
        public void InitLoggerTest0()
        {
            _instance.Logger = new log4net.Fakes.StubILog();
            _instanceShim.SetLoggerIRecordable = _ => Assert.Fail();
            _private.Invoke("InitLogger");
        }

        [TestMethod]
        public void InitLoggerTest1()
        {
            int callTimes = 0;
            _instanceShim.SetLoggerIRecordable = _ => ++callTimes;
            _instance.Pipelines.Add(new Pipeline.Fakes.StubIPipeline());
            _instance.PageProcessors.Add(new Processor.Fakes.StubIResponseProcessor());
            _private.Invoke("InitLogger");
            Assert.AreEqual(6, callTimes);
        }
        #endregion
    }
}