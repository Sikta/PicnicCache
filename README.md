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
  private IConfiguredCache<int, MyModel> _myModelCache;
  private IMyModelRepository _myModelRepository;
  
  public DataAccessService()
  {
    var cacheConfig = new PicnicCacheConfiguration(_myModelRepository.GetMyModelById,
                                                   _myModelRepository.GetAllMyModel,
                                                   _myModelRepository.SaveModels);
    _myModelCache = new ConfiguredPicnicCache<int, MyModel>(x => x.Id);
    _myModelRepository = new MyModelRepository();
  }
  
  public MyModel GetMyModelById(int id)
  {
    return _myModelCache.Fetch(id);
  }
  
  public IEnumerable<MyModel> GetAllMyModel()
  {
    return _myModelCache.FetchAll();
  }
  
  public void Save()
  {
    _myModelCache.Save();
  }
}
