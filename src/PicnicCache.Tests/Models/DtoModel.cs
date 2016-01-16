//The MIT License (MIT)
//https://github.com/DoloSoftware/PicnicCache/blob/master/LICENSE

namespace PicnicCache.Tests.Models
{
    public class DtoModel
    {
        #region Properties

        public string Name { get; set; }

        #endregion

        #region Constructors

        public DtoModel(string name)
        {
            Name = name;
        }

        #endregion
    }
}