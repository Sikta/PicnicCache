[![Build status](https://ci.appveyor.com/api/projects/status/st6uhut579chxx58/branch/master?svg=true)](https://ci.appveyor.com/project/Sikta/picniccache/branch/master)

# PicnicCache
A simple caching library

Example usage:

``` csharp
public class MyModel
{
  public int Id { get; set; }
  public int GroupId { get; set; }
  public string Name { get; set; }
}

public class MyModelRepository : IMyModelRepository, ICacheable<int, MyModel>
{
   public MyModel Fetch(int key)
   {
      //Method loading MyModel with Id = key
   }
   
   public IEnumerable<MyModel> FetchAll()
   {
      //Method returning all MyModel
      //Can get creative and have this method filter to a certain type
      //and create a separate cache for each filter. Could also wrap
      //your repository in a PicnicCacheMapping so you can use
      //one repository but map your filtered FetchAll methods to different
      //caches.
   }
   
   public void Save(IEnumerable<MyModel> added, IEnumerable<MyModel> deleted, IEnumerable<MyModel> updated)
   {
      //Method to save all modified items
   }
}

public class DataAccessService
{
  private ICache<int, MyModel> _myModelCache;
  
  public DataAccessService()
  {
    _myModelCache = new PicnicCache<int, MyModel>(new MyModelRepository());
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
    _myModelCache.ClearAll();
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
```
