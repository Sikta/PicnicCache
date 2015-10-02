//The MIT License (MIT)
//https://github.com/DoloSoftware/PicnicCache/blob/master/LICENSE

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PicnicCache.Tests
{
    public class ExpectedExceptionWithMessageAttribute : ExpectedExceptionBaseAttribute
    {
        #region Properties

        public string ExpectedMessage { get; set; }

        public Type ExceptionType { get; set; }

        #endregion

        #region Constructors

        public ExpectedExceptionWithMessageAttribute(Type exceptionType)
        {
            ExceptionType = exceptionType;
        }

        public ExpectedExceptionWithMessageAttribute(Type exceptionType, string expectedMessage)
        {
            ExceptionType = exceptionType;

            ExpectedMessage = expectedMessage;
        }

        #endregion

        #region Methods

        protected override void Verify(Exception e)
        {
            if (e.GetType() != ExceptionType)
            {
                Assert.Fail(String.Format(
                                "ExpectedExceptionWithMessageAttribute failed. Expected exception type: {0}. Actual exception type: {1}. Exception message: {2}",
                                ExceptionType.FullName,
                                e.GetType().FullName,
                                e.Message
                                )
                            );
            }

            var actualMessage = e.Message.Trim();

            if (ExpectedMessage != null)
            {
                Assert.AreEqual(ExpectedMessage, actualMessage);
            }
        }

        #endregion
    }
}