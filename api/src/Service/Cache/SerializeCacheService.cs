/*
   Copyright 2021-2024 Yvan Razafindramanana

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Azure;

public class SerializeCacheService(IAzureClientFactory<BlobServiceClient> azureClientFactory)
{
	private readonly BlobServiceClient blobServiceClient = azureClientFactory.CreateClient("WebStorageBlobServiceClient");

	internal async Task Serialize(List<object> images)
	{
		const string blobContainerName = "$web";
		const string blobName = "last-images.json";

		var blobContainerClient = blobServiceClient.GetBlobContainerClient(blobContainerName);
		var blobClient = blobContainerClient.GetBlobClient(blobName);

		await blobClient.UploadAsync(BinaryData.FromObjectAsJson(images), overwrite: true);
	}
}
