
namespace PicnicCache.Tests.Models
{
    public class TestModel
    {
        public int GroupId { get; set; }

        public int Id { get; set; }

        public string Text { get; set; }

        public TestModel()
        {

        }

        public TestModel(int id, int groupId, string text)
        {
            Id = id;
            GroupId = groupId;
            Text = text;
        }
    }
}