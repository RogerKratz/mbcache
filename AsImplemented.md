# Introduction #

Normally MbCache will create proxies of your interfaces. The syntax for this is `As<IYourInterface>()` when you declare what to cache on your CacheBuilder instance.

However, if you want your component to be a proxy of your implementation/class instead, you can use `AsImplemented()`.


# Details #

We recommend you to let MbCache create your proxies out of an interface. Remember though, that you cannot cast your component back to your implementation type in this case.

Illegal - will throw
```
var builder = new CacheBuilder(...)
  .For<MyComponent>()
  .CacheMethod(m => m.Something())
  .As<IMyComponent>();
var factory = builder.BuildFactory();
var svc = factory.Create<IMyComponent>();
((MyComponent)svc).DoIt();
```

The above code **would** have worked if `AsImplemented()` instead if `As<IMyComponent>()`were used. A drawback of this would have been that methods on MyComponent were forced to be virtual.