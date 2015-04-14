# CacheKeys #

ICacheKey is used to build a cache key based on method input.


## Primitive parameter values ##

If you only cache methods with primitive parameter types (or rather - objects that get unique `ToString()` result based on its value(s)), you can safely use `MbCache.DefaultImpl.ToStringCacheKey`.


## Entity parameter values ##

In cases where you want MbCache to cache a method where eg an entity is an inparameter, `ToStringCacheKey` (probably) isn't enough - you'll need to implement your own ICacheKey. MbCache offers a base class, `MbCache.Configuration.CacheKeyBase`, that should be a good starting point.

Assuming you for example have this method...
```
void Calculate(IOrder order);
```

...and your order's `ToString()` method doesn't return an order specific string (nor it should probably). To let MbCache create a unique cache key based on which order is passed to the method you must give a hint how the cache key should be created. Assuming your entities like IOrder implements IEntity, you could implement your `ICacheKey` like this, assuming IEntity.Id returns some unique value (eg a guid):
```
public class MyCacheKey : CacheKeyBase
{
  protected override string ParameterValue(object parameter)
  {
    var entity = parameter as IEntity;
    if (entity != null)
    {
      return entity.Id.ToString();
    }
    else
    {
      return parameter.ToString();
    }
  }
}
```


## Turn off MbCache for a method in runtime ##

In some cases, eg when you cannot turn an object into a string representation, you might want to turn off MbCache for a specific method with specific parameters values in runtime.
If ICacheKey return null in any of its methods, that tells MbCache to not use its caching for this entry. You can also control this in the `CacheKeyBase`.

Let's say your entities allow the Id property to be null and you cannot/doesn't want to offer any other unique "string representation" (which is needed for MbCache to work properly), you can ignore cache entries for methods where an entity without Id passed to the method like this.
```
public class MyCacheKey : CacheKeyBase
{
  protected override string ParameterValue(object parameter)
  {
    var entity = parameter as IEntity;
    if (entity != null)
    {
      if(entity.Id.HasValue)
        return entity.Id.ToString();
      else
        return null;
    }
    else
    {
      return parameter.ToString();
    }
  }
}
```
In the case above, cached methods accepting an IEntity as argument won't be cached if the entity has no Id set.


## Inject to `CacheBuilder` ##

If you want to use your own implementation of ICacheKey, pass it to `CacheBuilder` like this
```
var builder = new CacheBuilder(...);
builder.SetCacheKey(yourImpl);
```