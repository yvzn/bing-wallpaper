# Latest Bing Wallpaper

An application to display the latest wallpapers from [Bing.com](https://www.bing.com/).

This is a simple HTTP redirection to the URI of the latest wallpaper images. **Use of these images is restricted to wallpaper only**, as per Bing terms of use.

Implemented as a serverless Azure Function.

## Run locally

### Requirements

- [.NET SDK 3.x](https://dotnet.microsoft.com/download) or higher
- [Azure Function Core Tools v3.x](https://docs.microsoft.com/en-us/azure/azure-functions/functions-run-local#install-the-azure-functions-core-tools)
- Optional: [Azure Storage Emulator](https://docs.microsoft.com/en-us/azure/storage/common/storage-use-emulator#get-the-storage-emulator)

### Configuration

An *Azure storage account* is required to store and cache the wallpaper URI and avoid multiple requests to Bing.com. The storage account connection string has to be configured in the `AzureWebJobsStorage` setting:

#### Option 1

Use the connection string from the storage account of the deployed *Azure Function App*

#### Option 2

Install [Azure Storage Emulator](https://docs.microsoft.com/en-us/azure/storage/common/storage-use-emulator#get-the-storage-emulator) and set the connection string to `UseDevelopmentStorage=true`

### Build & run

```bash
cd api/src ↲
dotnet clean && dotnet build ↲
func host start ↲
```

Then open http://localhost:7071/api/redirection-to/latest in browser of choice.

<kbd>Ctrl + C</kbd> to stop the app.

## License

Licensed under [Apache License 2.0](https://choosealicense.com/licenses/apache-2.0/) 
