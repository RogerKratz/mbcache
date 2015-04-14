# Introduction #

You can tell MbCache to keep cache items by instance or by type.


# Details #

Let's say you want to cache results for IMyService.MyMethod(). By default the cache will be shared for all instances of IMyService created by `IMbCacheFactory`. If you don't want this behaviour but instead keep one cache per IMyService instance, you configure MbCache like this
```
cacheBuilder.For<MyService>()
  .CacheMethod(c => c.MyMethod())
  .PerInstance()
  .As<IMyService>();
```

**Note**
This also affects [invalidation](Invalidate.md).
