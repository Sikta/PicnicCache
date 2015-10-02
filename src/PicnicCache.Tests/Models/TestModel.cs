//The MIT License (MIT)
//https://github.com/DoloSoftware/PicnicCache/blob/master/LICENSE

namespace PicnicCache.Tests.Models
{
    public class TestModel : ITestObject
    {
        #region Constructors

        public TestModel()
        {

        }

        public TestModel(TestModel model)
        {
            Id = model.Id;
            Name = model.Name;
            City = model.City;
            State = model.State;
        }

        #endregion

        #region ITestObject

        public int Id { get; set; }

        public string Name { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        #endregion
    }
}