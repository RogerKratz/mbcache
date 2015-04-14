MbCache, or Method based Cache, is a [memoization](http://en.wikipedia.org/wiki/Memoization) library for .net. It is used to define certain methods to be cached in a centralized way, independed on the underlying caching framework.

How often have you written c# similar to this?
```
public class Calculator : ICalculator
{
 public SomeType Calculate()
 {
   var cachedValue = tryGetFromCache(yourKey);
   if(cachedValue != null)
     return cachedValue;
   var value = someExpansiveOperation();
   putInCache(yourKey, value);
   return value;
 }
 [...]
}
```

It sure works. There are some drawback though:
  * A snippet like this tends to repeat itself the next time you want to cache some return value in some other method. Not really [DRY](http://en.wikipedia.org/wiki/Don't_repeat_yourself)-ish.
  * You (might) have a strong dependency to your cache framework. With code like this it's not easy to reuse code amoung different sort of clients. You might, for example, want to use asp.net cache when code is run in web enviroment but something completely different in a desktop application.
  * What if you suddenly want to turn off caching for a method? Or some of them? All of them?
  * [SRP](http://en.wikipedia.org/wiki/Single_responsibility_principle) is broken. It can eg be solved using decorator pattern and split calculation and caching part, but it will lead to even more handwritten code.
  * Can be hard to test.

There's an alternative that solves this for you - MbCache!


  1. Write your business logic. No need to mess this up with cache logic.
```
public SomeType Calculate()
{
  return someExpansiveOperation();  
}
```
  1. Tell MbCache at start up what methods to cache (preferably together with your favorite ioc container).
```
var builder = new CacheBuilder(new LinFuProxyFactory());
builder.For<Calculator>()
    .CacheMethod(c => c.Calculate())
    .As<ICalculator>();
_factory = builder.BuildFactory();
```
  1. Use your objects. ICalculate.Calculate() will now use caching.
```
var yourService =_factory.Create<ICalculator>()
var value = yourService.Calculate();
```