# Introduction #

MbCache offers an interface, [IEventListener](http://code.google.com/p/mbcache/source/browse/MbCache/Core/Events/IEventListener.cs), giving the user information when a cache item has been fetched, added or removed.


## Details ##

The events fire **after** the respective action has happened. The events all contains a [EventInformation](http://code.google.com/p/mbcache/source/browse/MbCache/Core/Events/EventInformation.cs) holding information of the generated cache key, the caching component's type, the method info and the argument values.

### Usage ###

To use this interface, pass in an implementation to the CacheBuilder like this
```
var cacheBuilder = new CacheBuilder(...);
cacheBuidler
  .For<MyCachingComponent>()
    .CacheMethod(c => c.MyMethod(null, null))
    .AsImplemented()
  .AddEventListener(new MyEventListener());
```

One obvious usage of the interface is getting statistics out of MbCache. A statistics event listener is bundled with MbCache, the [StatisticsEventListener](Statistics.md).

Another example where this interface could be useful is if some cache entries should be invalidated when some other event occurs.