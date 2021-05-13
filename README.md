# Latest Bing Wallpaper

An application to display the latest wallpapers from [Bing.com](https://www.bing.com/).

This is a simple HTTP redirection to the URI of the latest wallpaper images. Use of these images is restricted to wallpaper only, as per Bing terms of use.

Implemented as a serverless Azure Function.

## Run the application locally

### Requirements

- [.NET SDK 3.x](https://dotnet.microsoft.com/download) or higher
- [Azure Function Core Tools v3.x](https://docs.microsoft.com/en-us/azure/azure-functions/functions-run-local#install-the-azure-functions-core-tools)

### Build & run

```bash
cd api ↲
dotnet clean && dotnet build ↲
func host start ↲
```

Then open http://localhost:7071/api/redirection-to/latest in browser of choice.

<kbd>Ctrl + C</kbd> to stop the app.

## License

Licensed under [Apache License 2.0](LICENSE) (as project uses .NET / ASP.NET)
