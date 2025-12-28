using System;
using System.Linq;
using Newtonsoft.Json.Linq;
using OeSystems.ScriptableHttp.Configuration;
using OeSystems.ScriptableHttp.Regex;

namespace OeSystems.ScriptableHttp.Json;

public interface IJsonResponseReadSingle
{
    public void Read(JObject document, JsonPathMap map, Values values, Errors errors);
}

public class JsonResponseReadSingle(IRegexMapper regex,
    IJsonTokenConverter converter) : IJsonResponseReadSingle
{
    public void Read(JObject document, JsonPathMap mapping, Values values, Errors errors)
    {
        var query = regex.Replace(values, mapping.Regex, mapping.Query);
        var tokens = document.SelectTokens(query).ToList();

        if (tokens.Count == 0)
        {
            if(mapping.Required)
                errors.Add(new($"The Query '{query}' did not have any matches."));
            return;
        }
        
        var result = converter.Convert(tokens[0], mapping);
        if (result.IsSuccess)
        {
            values[mapping.Key] = result.Value;
        }
        else
        {
            errors.AddRange(result.Errors);
        }
    }
}