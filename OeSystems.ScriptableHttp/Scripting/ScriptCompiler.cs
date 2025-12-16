using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using OeSystems.ScriptableHttp.Configuration;
using OeSystems.ScriptableHttp.Telemetry;

namespace OeSystems.ScriptableHttp.Scripting;

public interface IScriptCompiler
{
    Script<object> Compile(ScriptableHttpConfig config);
}

public class ScriptCompiler : IScriptCompiler
{
    public Script<object> Compile(ScriptableHttpConfig config)
    {
        using var _ = new CompileActivity(config);
        
        var options = ScriptOptions.Default
            .WithReferences(typeof(IReadOnlyValues).Assembly);
        return CSharpScript.Create(config.Script, options, typeof(ScriptGlobals));
    }
}