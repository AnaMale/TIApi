using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using TIAPi;

namespace TIApi
{
  public class Program
  {
    public static void Main(string[] args)
    {
      Program.BuildWebHost(args).Run();
    }

    public static IWebHost BuildWebHost(string[] args) =>
      WebHost.CreateDefaultBuilder(args)
        .UseContentRoot(Directory.GetCurrentDirectory())
        .UseKestrel()
        .UseIISIntegration()
        .UseStartup<Startup>()
        .Build();
  }
}