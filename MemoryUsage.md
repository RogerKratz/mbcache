# Introduction #

If you have a lot of cache entries, the cache keys created by MbCache may take large amount of memory resources. There are different ways to tell MbCache to create shorter cache keys.


## Details ##

The most low level way to control how cache keys are created, is to create your own [ICacheKey](CacheKeys.md) from scratch. That might be overkill though, an easier way that probably is good enough, is to control how a cache key should be created for a certain type.

By default, the cache key uses the component's full type name. However, you can easily override this behavior.
```
cacheBuilder.For<SomeType>("s")
  [...]
```
The configuration above tells MbCache to not use SomeType's full type name when building the cache keys for its methods. Instead the string "s" is used.


### Recommendation ###

If you don't suffer of memory problems because of a huge amount of cache entries, don't give explicit cache key type names for your components. And, of course, if it's **the data** that you cache holds that uses a lot of memory, there isn't any configuration in MbCache that will help you...