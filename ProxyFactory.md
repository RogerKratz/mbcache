# Introduction #

MbCache uses an underlying proxy framework to create proxy objects. Two implementations are included in this package, `MbCache.ProxyImpl.Castle` and `MbCache.ProxyImpl.LinFu`.

You can also do your own implementation for your proxy framework by implement [IProxyFactory](http://code.google.com/p/mbcache/source/browse/MbCache/Configuration/IProxyFactory.cs). [Contributions](Contribution.md) are welcome!


# Details #

If you eg want to use Castle's DynamicProxy as your proxy framework, do
  * Add a reference to `MbCache.ProxyImpl.Castle` where you configure MbCache.
  * Pass in an instance of the proxy factory to CacheBuilder ctor, eg
```
var builder = new CacheBuilder(new CastleProxyFactory());
```

Note! In distributed nuget package only `LinFuProxyFactory` is bundled.