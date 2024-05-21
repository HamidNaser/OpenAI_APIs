using System;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace OpenAI_APIs;

public class ServiceRegistration: IServiceRegistration
{
    public virtual void RegisterAll(IServiceCollection services)
    {
        RegisterServices(services);

        using var sp = services.BuildServiceProvider();
        var configuration = sp.GetRequiredService<IConfiguration>();
    }

   public virtual void RegisterServices(IServiceCollection services)
   {
       services.AddHttpClient("connect")
           .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler()
           {
               ServerCertificateCustomValidationCallback =
                   HttpClientUtils.GenerateServerCertificateCustomValidationCallback()
           });
       
       
        services.AddSingleton<IOpenAiService, OpenAiService>();        
    }
}


public interface IServiceRegistration
{
    public void RegisterAll(IServiceCollection services);
    public void RegisterServices(IServiceCollection services);
}

public static class HttpClientUtils
{
    public static Func<HttpRequestMessage, X509Certificate2, X509Chain, SslPolicyErrors, bool> GenerateServerCertificateCustomValidationCallback()
    {
        return (requestMessage, certificate, chain, sslPolicyErrors) =>
        {
            return true; 
        };
    }
}
