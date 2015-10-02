//The MIT License (MIT)
//https://github.com/DoloSoftware/PicnicCache/blob/master/LICENSE

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PicnicCache.Tests.Models;

namespace PicnicCache.Tests.Tests
{
    /// <summary>
    /// Summary description for ReadOnlyPicnicCacheMappingTests
    /// </summary>
    [TestClass]
    public class ReadOnlyPicnicCacheMappingTests : PicnicCacheTestBase
    {
        #region Fields

        private TestContext testContextInstance;

        #endregion

        #region Properties

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #endregion

        #region Constructors

        public ReadOnlyPicnicCacheMappingTests()
        {
        }

        #endregion

        #region Additional test attributes

        // Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext) { }

        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }

        // Use TestInitialize to run code before running each test 
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}

        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }

        #endregion

        #region Constructor Tests

        [TestMethod]
        [ExpectedExceptionWithMessage(typeof(ArgumentNullException), "Value cannot be null.\r\nParameter name: fetch")]
        public void Constructor_PassNullFetch_ThrowsException()
        {
            //Arrange
            var fetchAll = new Mock<Func<IEnumerable<ITestObject>>>().Object;
          
            //Act
            var mapping = new ReadOnlyPicnicCacheMapping<int, ITestObject>(null, fetchAll);
        }

        [TestMethod]
        [ExpectedExceptionWithMessage(typeof(ArgumentNullException), "Value cannot be null.\r\nParameter name: fetchAll")]
        public void Constructor_PassNullFetchAll_ThrowsException()
        {
            //Arrange
            var fetch = new Mock<Func<int, ITestObject>>().Object;

            //Act
            var mapping = new ReadOnlyPicnicCacheMapping<int, ITestObject>(fetch, null);
        }

        #endregion

        #region Fetch Tests

        [TestMethod]
        public void Fetch_AllScenarios_FuncIsCalled()
        {
            //Arrange
            bool fetchCalled = false;
            bool fetchAllCalled = false;
            Func<int, ITestObject> fetch = x => { fetchCalled = true; return new Mock<ITestObject>().Object; };
            Func<IEnumerable<ITestObject>> fetchAll = () => { fetchAllCalled = true; return new Mock<IEnumerable<ITestObject>>().Object; };
            var mapping = new ReadOnlyPicnicCacheMapping<int, ITestObject>(fetch, fetchAll);

            //Act
            var result = mapping.Fetch(0);

            //Assert
            Assert.IsTrue(fetchCalled);
            Assert.IsFalse(fetchAllCalled);
        }

        #endregion

        #region FetchAll Tests

        [TestMethod]
        public void FetchAll_AllScenarios_FuncIsCalled()
        {
            //Arrange
            bool fetchCalled = false;
            bool fetchAllCalled = false;
            Func<int, ITestObject> fetch = x => { fetchCalled = true; return new Mock<ITestObject>().Object; };
            Func<IEnumerable<ITestObject>> fetchAll = () => { fetchAllCalled = true; return new Mock<IEnumerable<ITestObject>>().Object; };
            var mapping = new ReadOnlyPicnicCacheMapping<int, ITestObject>(fetch, fetchAll);

            //Act
            var result = mapping.FetchAll();

            //Assert
            Assert.IsTrue(fetchAllCalled);
            Assert.IsFalse(fetchCalled);
        }

        #endregion
    }
}