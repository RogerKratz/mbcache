# Introduction #

MbCache itself does not contain any cache logic. Instead it offers an interface, [ICache](http://code.google.com/p/mbcache/source/browse/MbCache/Configuration/ICache.cs), that works as an intermediate object between MbCache and the cache framework.
Bundled with MbCache follows one implementation of this interface, `InMemoryCache`, that uses `System.Runtime.Caching.MemoryCache`. This ICache implementation will be used by default but can be overriden (see below).


## Details ##

The ICache is used internally by MbCache. It contains methods to Get, Put or Delete from the cache.


## Inject to CacheBuilder ##

If you want to use your own implementation of ICache, pass it to `CacheBuilder` like this
```
var builder = new CacheBuilder(...);
builder.SetCache(yourImpl);
```