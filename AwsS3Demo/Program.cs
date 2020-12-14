using Amazon.S3;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Threading.Tasks;

namespace AwsS3Demo
{
    class Program
    {
        private const string BUCKET_NAME = "fahim-demo-bucket";
        private const string REGION_NAME = "us-east-1";


        private static ServiceProvider ServiceProvider;
        private static IFileStorageService _storageService;
        public static IConfiguration Configuration { get; private set; }
        
        static void Main(string[] args)
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var builder = new ConfigurationBuilder()
                .AddJsonFile($"appsettings.json", true, true)
                .AddJsonFile($"appsettings.{env}.json", true, true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();

            var _serviceCollection = new ServiceCollection();
            _serviceCollection.AddDefaultAWSOptions(Configuration.GetAWSOptions());
            _serviceCollection.AddAWSService<IAmazonS3>(ServiceLifetime.Singleton);
            _serviceCollection.AddSingleton<IFileStorageService>(
                x => new S3BucketStorageService(x.GetRequiredService<IAmazonS3>(),
                    BUCKET_NAME,
                    REGION_NAME
                )
            );

            ServiceProvider = _serviceCollection.BuildServiceProvider();

            MainAsync().Wait();
            
        }

        static async Task MainAsync()
        {
            _storageService = ServiceProvider.GetService<IFileStorageService>();

            var fileInfo = new FileInfo(@"D:\development\dotnet\AwsPractice\AwsS3Demo\Data\00015.jpg");
            var url = await _storageService.StoreFileAsync(fileInfo);

            Console.WriteLine(url);
        }
    }
}
