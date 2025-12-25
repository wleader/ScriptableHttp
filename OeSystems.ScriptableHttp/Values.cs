using System;
using System.Collections.Generic;

namespace OeSystems.ScriptableHttp;

public class Values() : Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase)
{
    public static readonly IReadOnlyValues Empty = new Values();

     public void Add(IReadOnlyValues other)
     {
         foreach (var kvp in other)
         {
             this[kvp.Key] = kvp.Value;
         }
     }
}
