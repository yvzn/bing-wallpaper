using System;
using Azure.Data.Tables;
using Ludeo.BingWallpaper.Service.Bing;
using Ludeo.BingWallpaper.Service.Cache;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var host = new HostBuilder()
	.ConfigureFunctionsWebApplication()
	.ConfigureServices(services =>
	{
		services.AddHttpClient();

		services.AddAzureClients(clientBuilder =>
		{
			clientBuilder
				.AddTableServiceClient(Environment.GetEnvironmentVariable("AzureWebJobsStorage"));

			clientBuilder
				.AddClient<TableClient, TableClientOptions>(
					(_, _, provider) => provider.GetService<TableServiceClient>()!.GetTableClient("ImageCache"))
				.WithName("ImageCacheTableClient");
		});

		services.AddSingleton<CacheService>();
		services.AddSingleton<CleanCacheService>();
		services.AddSingleton<HashingService>();
		services.AddSingleton<UpdateCacheService>();
		services.AddSingleton<WallpaperService>();
	})
	.ConfigureLogging(logging =>
	{
		logging.SetMinimumLevel(LogLevel.Warning);
		logging.AddFilter("Function", LogLevel.Warning);
		logging.AddFilter("System.Net.Http.HttpClient", LogLevel.Warning);
	})
	.Build();

host.Run();
