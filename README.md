# Latest Bing Wallpaper

An application to display the latest wallpapers from [Bing.com](https://www.bing.com/).

**Use of these images is restricted to wallpaper only**, as per Bing terms of use.

## How?

The front-end is a [React](https://reactjs.org/) website packaged with [ViteJS](https://vitejs.dev/)

The back-end is a serverless [Azure Function app](https://docs.microsoft.com/en-us/azure/azure-functions/).

## Run locally

### Run the front-end

#### Requirements

- [NodeJS 12](https://nodejs.org/en/download/) or higher

#### Start

Copy `.env.local.sample` to `.env.local`, then:

```bash
cd app
npm install
npm run dev
```

Then open http://localhost:3000/ in browser of choice.

<kbd>Ctrl + C</kbd> to exit

### Run the back-end

#### Requirements

- [.NET SDK 3.x](https://dotnet.microsoft.com/download) or higher
- [Azure Function Core Tools v3.x](https://docs.microsoft.com/en-us/azure/azure-functions/functions-run-local#install-the-azure-functions-core-tools)
- Optional: [Azure Storage Emulator](https://docs.microsoft.com/en-us/azure/storage/common/storage-use-emulator#get-the-storage-emulator)

#### Configuration

An *Azure storage account* is required to store and cache the wallpaper URI and avoid multiple requests to Bing.com. The storage account connection string has to be configured in the `AzureWebJobsStorage` setting:

The storage account is cleaned up regularly from old entries - so the storage account size should never be too large.

##### Option 1

Use the connection string from the storage account of the deployed *Azure Function App*

##### Option 2

Install [Azure Storage Emulator](https://docs.microsoft.com/en-us/azure/storage/common/storage-use-emulator#get-the-storage-emulator) and set the connection string to `UseDevelopmentStorage=true`

#### Build & start

Copy `local.settings.json.sample` to `local.settings.json`, then:

```bash
cd api/src
dotnet clean && dotnet build
func host start
```

Then open http://localhost:7071/api/redirection-to/latest in browser of choice.

<kbd>Ctrl + C</kbd> to stop the app.

## License

Licensed under [Apache License 2.0](https://choosealicense.com/licenses/apache-2.0/)

### Third parties

The wallpaper themselves are hosted by Bing.com and copyrighted by their respective owners. **Use of these images is restricted to wallpaper only**, as per Bing terms of use.

This project uses open-source, third party software:

- [.NET SDK 3.x](https://github.com/dotnet/sdk): MIT License, Copyright (c) .NET Foundation
- [Azure Function Core Tools v3.x](https://github.com/Azure/azure-functions-core-tools): MIT License, Copyright (c) .NET Foundation
- [ViteJS](https://github.com/vitejs/vite): MIT License, Copyright (c) 2019-present Evan You & Vite Contributors
- [React](): MIT License, Copyright (c) Facebook, Inc. and its affiliates.

This project uses graphics under Creative Commons:

- Fav icon by [Oh Rian](https://thenounproject.com/ohrianid/): Creative Commons (CCBY) license
