using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;

namespace OeSystems.ScriptableHttp;

public interface ICultureContext
{
    CultureInfo Current { get; }
    IDisposable Set(string culture);
}

public class CultureContext : ICultureContext
{
    private class CultureScope(CultureContext parent) : IDisposable
    {
        private int _disposed;
        public void Dispose()
        {
            // interlocked to prevent multiple dispose.
            if(Interlocked.CompareExchange(ref _disposed, 1, 0) == 0)
                parent.Revert();
        }
    }
    
    private static CultureInfo FromString(string? culture)
    {
        if (string.IsNullOrWhiteSpace(culture)) 
            return CultureInfo.InvariantCulture;
        try
        {
            return CultureInfo.GetCultureInfo(culture, true);
        }
        catch (Exception)
        {
            // log something?
            return CultureInfo.InvariantCulture;
        }
    }
    
    private readonly Stack<CultureInfo> _stack = new();

    public CultureInfo Current =>
        _stack.Count == 0 ? CultureInfo.InvariantCulture : _stack.Peek();

    public IDisposable Set(string culture)
    {
        _stack.Push(FromString(culture));
        return new CultureScope(this);
    }

    private void Revert()
    {
        if (_stack.Count > 0) 
            _stack.Pop(); 
    }
}