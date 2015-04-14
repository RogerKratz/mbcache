# Introduction #

When you've defined what to cache in your CacheBuilder and created your IMbCacheFactory instance, there are two ways for you to create your caching components.


## Create a cached component ##

If you want to use MbCache as a simple factory to get components that cache certain methods, simply use...

```
IMbCacheFactory factory = ...;
var myComponent = factory.Create<IMyComponent>;
```

## Let a component not aware of MbCache start cache configured methods ##

In some cases it might be easier to give MbCache the uncaching component and turn this into a caching component.
One example when this could be handy is if you're using an ioc container and your components have a lot of dependencies that you don't want to specify when the component is created.

```
IMbCacheFactory factory = ...;
var myNonCachingComponent = new MyComponent(...);
var myCachingComponent = factory.ToCachedComponent<IMyComponent>(myNonCachingComponent);
```