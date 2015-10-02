//The MIT License (MIT)
//https://github.com/DoloSoftware/PicnicCache/blob/master/LICENSE

namespace PicnicCache.Tests.Models
{
    public interface ITestObject
    {
        #region Properties

        int Id { get; }
        string Name { get; set; }
        string City { get; set; }
        string State { get; set; }

        #endregion
    }
}