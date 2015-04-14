# Introduction #

When a generic type or generic parameters is configured to be cached, you declare what closed generic types you want to cache. That is, you may want to cache `Foo<Bar>` - in this case `Foo<Bar2>` won't be cached. The same goes for generic methods.


# Generic types #

Let's say you have
```
public class Foo<T>
{
  public int Bar(T value)
  {
    [...]
    return something;
  }
}
```

If you want to cache `Foo<int>.Bar(int)`, configure MbCache like this
```
var builder = new CacheBuilder(...);
builder.For<Foo<int>>()
  .CacheMethod(m => m.Bar(0))
  .AsImplemented(); 
```


# Generic parameters #

Let's say you have
```
public class Foo
{
  public int Bar<T1, T2>(T1 first, T2 second)
  {
    [...]
    return something;
  }
}
```

If you want to cache Foo.Bar<Person, Order>(Person, Order), configure MbCache like this
```
var builder = new CacheBuilder(...);
builder.For<Foo>()
  .CacheMethod(m => m.Bar(anyPersonInstance, anyOrderInstance))
  .AsImplemented(); 
```

# Note #

Currently, MbCache does **not** support configuration of open generic types. In other words you cannot define MbCache to cache `Foo<T>.Bar()` or `Foo.Bar<T>(T)`.
However - it will most probably be supported in the future.