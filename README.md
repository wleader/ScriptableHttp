# ScriptableHttp
This library is a way for user configuration to define and control a series
of HTTP operations.

**⚠️ There are parts of this library that are not implemented yet. ⚠️**

**⚠️ It is not yet 'production ready'. ⚠️**

**⚠️ You have been Warned. ⚠️**

## Getting Started

### Dependency Injection
This library is written to be Dependency Injection friendly.
You will want to call the `.AddScriptableHttp()` extension method
on your IServiceCollection.

#### Requires Microsoft.Extensions.Http
This library uses the IHttpClientFactory to create instances of HttpClient.
You will need to register IHttpClientFactory with your IServiceCollection.
The Microsoft.Extensions.Http library provides an extension method
to do this: `.AddHttpClient()`.
The library does a name when getting the HttpClient from the factory "ScriptableHttpClient".
This will allow you to optionally customize the HttpClient instance used by the library.
See: https://learn.microsoft.com/en-us/dotnet/core/extensions/httpclient-factory

### Invoking the Client
You can invoke the client by getting an instance of IScriptableHttpClient from
Dependency Injection.

`var client = serviceProvider.GetInstance<IScriptableHttpClient>();`

Invoke the client by providing a Configuration and a set of Values.

`var result = client.Invoke(config, values);`

   * Configuration is an instance of 
     [ScriptableHttpConfig](OeSystems.ScriptableHttp/Configuration/OeSystems.ScriptableHttp.Configuration.cs).
     The configuration class has an overloaded static .Load() method that will allow you to load the XML from a string or stream.
   * Values is an instance of `IReadOnlyDictionary<string, object?>`. 
     These are the values that your application will make available to the client.
   * The result is a [Result](OeSystems.ScriptableHttp/Result.cs) object that contains a Status,
     and another instance of `IReadOnlyDictionary<string, object?>` that has been populated 
     from the HTTP responses according the configuration.

### Configuration XML
This library makes use of a configuration XML to control its behavior at runtime.
The content and structure of this XML is determined by [Configuration.xsd](OeSystems.ScriptableHttp/Configuration/Configuration.xsd).
A configuration XML is composed of three things.
* A Name
* One or More Operations
* Optionally a C# script

The name used should be unique if your application is going to have multiple configuration that are used at different times.

### Operations
An operation defines one HTTP request, the rules used to build that request, and 
the rules to parse that request. 
* A request can be either a GET or a POST.
* A request has an HTTP or HTTPS URI.
    * You can use regular expressions to replace parts of the URI.
* You can add custom headers to the request.
    * You can use regular expressions to replace parts of the custom headers.
* A POST request can define a body.
    * A request body can be XML, JSON, or any arbitrary text.
    * The request body can be altered before sending, by way of finding and replacing
      parts of the request using XPath, JsonPath, or Regular Expressions.
* An operation includes a response mapping. The response mapping determines how to extract
    specific data from the HTTP response and store the matched data in the Values.
    * Headers can be read, and partially matched using Regular Expressions.
    * The Body Can be read using XPath, JsonPath, or Regular Expressions.

### Order of Operations & Scripts
If a C# script is not provided in the configuration, each operation is performed
in the order that they are defined in the configuration.
If any operation does not return HTTP Status OK (200),
the client stops and subsequent operations are not performed.
Values that are set from the response of one operation are available to be used when
creating the request for subsequent operations.

If a C# script is provided, then that script is used to control the order of operations.
Consider the following script:
```
<Script><![CDATA[
    await OperationAsync("RetrieveData");
    if(LastOperationSuccessful)
        return;
    await OperationAsync("GetToken");
    if(!LastOperationSuccessful)
        return;    
    SetPersisted("Token");
    await OperationAsync("RetrieveData");
]]></Script>
```
This script uses the `OperationAsync()` method to call operations defined in the
configuration by their name. After an operation `LastOperationSuccessful` can be checked.
This script also makes use of Persisted Value. Persisted Values are kept in memory
and made available when the same configuration is run a second time.
If the RetrieveData operation fetches some data from a service that uses Token Authorization,
and the token is not present in the values (and is valid), then the first operation will succeed,
and the script will return. However, if the token was not present in the values.
This script will fail the firsts operation, then call the GetToken operation.
If the token could not be obtained, it returns as a failure. If it did get the token,
it uses `SetPersisted()` to remember the token for when the script is run again. It then
retries the original RetrieveData operation with the refreshed token.
