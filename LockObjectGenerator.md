# Introduction #

When MbCache updates the cache, by default no thread locks are held. If multiple requests simultaneously are querying for a cache entry that does not exist, they will all do their work and fill the cache. Assuming the underlying cache framework is thread safe, this will normally be fine. In some applications this is not good enough though, and it is better to let the first request do its job while the other wait until finished.

A drawback with using an `ILockObjectGenerator` is that every actual cache miss, currently results in two Gets to the cache.

# Details #

CacheBuilder has a method, `SetLockObjectGenerator`, accepting an [ILockObjectGenerator](http://code.google.com/p/mbcache/source/browse/MbCache/Configuration/ILockObjectGenerator.cs) as argument. This interface's responsibility is to return a lock instance based on the cache key.

One implementation of [ILockObjectGenerator](http://code.google.com/p/mbcache/source/browse/MbCache/Configuration/ILockObjectGenerator.cs) is bundled with MbCache - `FixedNumberOfLockObjects`. It holds a defined number of lock objects internally and tries its best to spread these among different cache keys (the same cache key will always return the same lock object).



## Inject to CacheBuilder ##

If you want to use your own implementation of `ILockObjectGenerator` (or use `FixedNumberOfLockObjects)`, pass it to `CacheBuilder` like this
```
var builder = new CacheBuilder(...);
builder.SetLockObjectGenerator(yourImpl);
```