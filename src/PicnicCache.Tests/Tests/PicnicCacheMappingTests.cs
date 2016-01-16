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
    /// Summary description for PicnicCacheMappingTests
    /// </summary>
    [TestClass]
    public class PicnicCacheMappingTests
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

        public PicnicCacheMappingTests()
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
            var save = new Mock<Action<IEnumerable<ITestObject>, IEnumerable<ITestObject>, IEnumerable<ITestObject>>>().Object;

            //Act
            var mapping = new PicnicCacheMapping<int, ITestObject>(null, fetchAll, save);
        }

        [TestMethod]
        [ExpectedExceptionWithMessage(typeof(ArgumentNullException), "Value cannot be null.\r\nParameter name: fetchAll")]
        public void Constructor_PassNullFetchAll_ThrowsException()
        {
            //Arrange
            var fetch = new Mock<Func<int, ITestObject>>().Object;
            var save = new Mock<Action<IEnumerable<ITestObject>, IEnumerable<ITestObject>, IEnumerable<ITestObject>>>().Object;

            //Act
            var mapping = new PicnicCacheMapping<int, ITestObject>(fetch, null, save);
        }

        [TestMethod]
        [ExpectedExceptionWithMessage(typeof(ArgumentNullException), "Value cannot be null.\r\nParameter name: save")]
        public void Constructor_PassNullSave_ThrowsException()
        {
            //Arrange
            var fetch = new Mock<Func<int, ITestObject>>().Object;
            var fetchAll = new Mock<Func<IEnumerable<ITestObject>>>().Object;

            //Act
            var mapping = new PicnicCacheMapping<int, ITestObject>(fetch, fetchAll, null);
        }

        #endregion

        #region Fetch Tests

        [TestMethod]
        public void Fetch_AllScenarios_FuncIsCalled()
        {
            //Arrange
            bool fetchCalled = false;
            bool fetchAllCalled = false;
            bool saveCalled = false;
            Func<int, ITestObject> fetch = x => { fetchCalled = true; return new Mock<ITestObject>().Object; };
            Func<IEnumerable<ITestObject>> fetchAll = () => { fetchAllCalled = true; return new Mock<IEnumerable<ITestObject>>().Object; };
            Action<IEnumerable<ITestObject>, IEnumerable<ITestObject>, IEnumerable<ITestObject>> save = (a, b, c) => { saveCalled = true; };
            var mapping = new PicnicCacheMapping<int, ITestObject>(fetch, fetchAll, save);

            //Act
            var result = mapping.Fetch(0);

            //Assert
            Assert.IsTrue(fetchCalled);
            Assert.IsFalse(fetchAllCalled);
            Assert.IsFalse(saveCalled);
        }

        #endregion

        #region FetchAll Tests

        [TestMethod]
        public void FetchAll_AllScenarios_FuncIsCalled()
        {
            //Arrange
            bool fetchCalled = false;
            bool fetchAllCalled = false;
            bool saveCalled = false;
            Func<int, ITestObject> fetch = x => { fetchCalled = true; return new Mock<ITestObject>().Object; };
            Func<IEnumerable<ITestObject>> fetchAll = () => { fetchAllCalled = true; return new Mock<IEnumerable<ITestObject>>().Object; };
            Action<IEnumerable<ITestObject>, IEnumerable<ITestObject>, IEnumerable<ITestObject>> save = (a, b, c) => { saveCalled = true; };
            var mapping = new PicnicCacheMapping<int, ITestObject>(fetch, fetchAll, save);

            //Act
            var result = mapping.FetchAll();

            //Assert
            Assert.IsTrue(fetchAllCalled);
            Assert.IsFalse(fetchCalled);
            Assert.IsFalse(saveCalled);
        }

        #endregion

        #region Save Tests

        [TestMethod]
        public void Save_AllScenarios_FuncIsCalledAndCorrectItemsPassed()
        {
            //Arrange
            IEnumerable<ITestObject> addedItems = new List<ITestObject>();
            IEnumerable<ITestObject> deletedItems = new List<ITestObject>();
            IEnumerable<ITestObject> modifiedItems = new List<ITestObject>();
            IEnumerable<ITestObject> arg1 = null;
            IEnumerable<ITestObject> arg2 = null;
            IEnumerable<ITestObject> arg3 = null;
            bool fetchCalled = false;
            bool fetchAllCalled = false;
            bool saveCalled = false;
            Func<int, ITestObject> fetch = x => { fetchCalled = true; return new Mock<ITestObject>().Object; };
            Func<IEnumerable<ITestObject>> fetchAll = () => { fetchAllCalled = true; return new Mock<IEnumerable<ITestObject>>().Object; };
            Action<IEnumerable<ITestObject>, IEnumerable<ITestObject>, IEnumerable<ITestObject>> save = (a, b, c) => { saveCalled = true; arg1 = a; arg2 = b; arg3 = c; };
            var mapping = new PicnicCacheMapping<int, ITestObject>(fetch, fetchAll, save);

            //Act
            mapping.Save(addedItems, deletedItems, modifiedItems);

            //Assert
            Assert.IsTrue(saveCalled);
            Assert.AreEqual(addedItems, arg1);
            Assert.AreEqual(deletedItems, arg2);
            Assert.AreEqual(modifiedItems, arg3);
            Assert.IsFalse(fetchCalled);
            Assert.IsFalse(fetchAllCalled);
        }

        #endregion
    }
}
