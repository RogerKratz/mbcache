# Introduction #

Bundled with MbCache comes one implementation of a [IEventListener](IEventListener.md), the [StatisticsEventListener](http://code.google.com/p/mbcache/source/browse/MbCache/Core/Events/StatisticsEventListener.cs).

## Usage ##

To use the [StatisticsEventListener](http://code.google.com/p/mbcache/source/browse/MbCache/Core/Events/StatisticsEventListener.cs), pass an instance to the `CacheBuilder` and keep a reference to this object.

```
var statEventListener = new StatisticsEventListener();
var cacheBuilder = new CacheBuilder(...);
[...configuration of caching components...]
cacheBuilder.AddEventListener(statEventListener);
```


### Number of cache hits ###

```
var totalNumberOfCacheHits = statEventListener.CacheHits;
```

### Number of cache misses ###

```
var totalNumberOfCacheMisses = statEventListener.CacheMisses;
```


## Note ##

You cannot use `StatisticEventListener` if you need to serialize/deserialize your `IMbCacheFactory`.