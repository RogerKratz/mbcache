# Introduction #

By default, all methods you've configured to be cached are enabled when your `IMbCacheFactory` has been created. You can, however, disable caching in run time.


To disable (or enable) caching for a certain type, use
```
factory.DisableCache<IMyComponent>();
[..]
factory.EnableCache<IMyComponent>();
```



## Details ##

Currently you can only disable caching for all the instances and methods for a specific type. This will probably be enhanced in later versions of MbCache - making it possible to disable specific methods with specific parameters.