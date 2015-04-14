# Introduction #

MbCache supports different kinds of invalidation. You can either tell `IMbCacheFactory` to release objects from cache or you can explicitly invalidate a cached component.


# Details #

### IMbCacheFactory ###

To invalidate all cached data for your type IService, simply call
```
factory.Invalidate<IService>();
```

To invalidate all cached methods for a specific object (this may affect multiple instances - see PerInstance), call
```
factory.Invalidate(myService);
```

If you want to invalidate entries for a specific method (this may affect multiple instances - see PerInstance), call
```
factory.Invalidate(myService, c => c.MyCachedMethod(value1, value2), false);
```

If you want to invalidate an entry for a specific method with specific parameter value(s) (this may affect multiple instances - see PerInstance), call
```
factory.Invalidate(myService, c => c.MyCachedMethod(value1, value2), true);
```


### ICachingComponent ###

Every object created by `IMbCacheFactory` implements ICachingComponent. Another way to invalidate cache entries is to use this interface.

To invalidate all cached methods for a specific object (this may affect multiple instances - see PerInstance), call
```
((ICachingComponent)yourService).Invalidate();
```

To invalidate a specific method (this may affect multiple instances - see PerInstance), call
```
((ICachingComponent)yourService).Invalidate(c => c.MyCachedMethod(value1, value2), false);
```

If you want to invalidate an entry for a specific method with specific parameter value(s) (this may affect multiple instances - see PerInstance), call
```
((ICachingComponent)yourService).Invalidate(myService, c => c.MyCachedMethod(value1, value2), true);
```


# See also #

MbCache supports caching by instance or by type. Have a look at PerInstance.