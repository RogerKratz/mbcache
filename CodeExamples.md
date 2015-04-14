# Introduction #

To get MbCache to work properly you need to
  * inject an [IProxyFactory](ProxyFactory.md)
  * configure MbCache (preferable at start up or similar)

# Example #

  1. Configure what to cache, in this case ClassToCache.CachedMethod() and Foo.Bar().
```
var builder = new CacheBuilder(new LinFuProxyFactory());
builder
  .For<ClassToCache>()
    .CacheMethod(c => c.CachedMethod())
    .As<IClassToCache>();
  .For<Foo>()
    .CacheMethod(c => c.Bar())
    .As<IFoo>();
```
  1. Build IMbCacheFactory once. Keep a reference to this factory instance.
```
factory = builder.BuildFactory();
```
  1. Get your objects from MbCache later in your code
```
var objectUsingMbCache = factory.Create<IClassToCache>();
var objectUsingMbCache2 = factory.Create<IClassToCache>();
```
  1. Use your object
```
var result = objectUsingMbCache.CachedMethod();
var result2_1 = objectUsingMbCache2.CachedMethod();
var result2_2 = objectUsingMbCache2.CachedMethod();
Assert.AreEqual(result, result2_1);
Assert.AreEqual(result, result2_2);
```


For more examples, have a look at this projects [unit tests](http://code.google.com/p/mbcache/source/browse/#hg%2FMbCacheTest%2FLogic%253Fstate%253Dclosed)