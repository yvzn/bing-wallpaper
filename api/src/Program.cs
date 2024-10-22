using System;
using Azure.Data.Tables;
using Ludeo.BingWallpaper.Service.Bing;
using Ludeo.BingWallpaper.Service.Cache;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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

			clientBuilder
				.AddBlobServiceClient(Environment.GetEnvironmentVariable("WEB_STORAGE_CONNECTION_STRING"));
		});

		services.AddSingleton<CacheService>();
		services.AddSingleton<CleanCacheService>();
		services.AddSingleton<HashingService>();
		services.AddSingleton<SerializeCacheService>();
		services.AddSingleton<UpdateCacheService>();
		services.AddSingleton<WallpaperService>();
	})
	.Build();

host.Run();
