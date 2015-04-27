# PicnicCache
A simple caching library

Example usage:

```
public class MyModel
{
  public int Id { get; set; }
  public int GroupId { get; set; }
  public string Name { get; set; }
}

public class DataAccessService
{
  private ICache<int, MyModel> _myModelCache;
  private IDictionary<int, ICache<int, MyModel>> _myModelByGroupCaches;
  
  public DataAccessService()
  {
    _myModelCache = new PicnicCache<int, MyModel>(x => x.Id);
    _myModelByGroupCaches = new Dictionary<int, ICache<int, MyModel>>();
  }
  
  public MyModel GetMyModelById(int id)
  {
    return _myModelCache.Fetch(id, () => _myModelRepository.GetMyModelById(id));
  }
  
  public IEnumerable<MyModel> GetAllMyModel()
  {
    return _myModelCache.FetchAll(() => _myModelRepository.GetAllMyModel());
  }
  
  public IEnumerable<MyModel> GetAllMyModelForGroup(int groupId)
  {
    ICache<int, MyModel> myModelByGroupCache;
    if (!_myModelByGroupCaches.TryGetValue(groupId, out myModelByGroupCache))
    {
      myModelByGroupCache = new PicnicCache<int, MyModel>(x => x.Id);
      _myModelByGroupCaches.Add(groupId, myModelByGroupCache);
    }
    return myModelByGroupCache.FetchAll(() => _myModelRepository.GetAllMyModelForGroup(groupId));
  }
}
