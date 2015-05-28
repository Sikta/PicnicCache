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
    _myModelRepository = new MyModelRepository();
    var cacheConfig = new PicnicCacheConfiguration(_myModelRepository.GetMyModelById,
                                                   _myModelRepository.GetAllMyModel,
                                                   _myModelRepository.SaveModels);
    _myModelCache = new ConfiguredPicnicCache<int, MyModel>(x => x.Id, cacheConfig);
  }
  
  public MyModel GetMyModelById(int id)
  {
    return _myModelCache.Fetch(id);
  }
  
  public IEnumerable<MyModel> GetAllMyModel()
  {
    return _myModelCache.FetchAll();
  }
  
  public void Add(MyModel model)
  {
    _myModelCache.Add(model);
  }
  
  public void Clear()
  {
    _myModelCache.Clear();
  }
  
  public void Update(MyModel model)
  {
    _myModelCache.Update(model);
    //also supports selective updating of properties
    //if you pass in a Func<T, TValue, TValue> method
    //using one of the Update overrides.
  }
  
  public void Delete(MyModel model)
  {
    _myModelCache.Delete(model);
    //also supports deleting by key
  }
  
  public void Save()
  {
    _myModelCache.Save();
  }
}
