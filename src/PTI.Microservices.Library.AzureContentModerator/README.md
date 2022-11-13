# PTI.Microservices.Library.AzureContentModerator

Facilitates the consumption of the APIs in Azure Content Moderator Service

**Examples:**

**Note: The examples below are passing null for the logger, if you want to use the logger make sure to pass the parameter with a value other than null**
	
## Analyze Text
    AzureContentModeratorService azureContentModeratorService =
       new AzureContentModeratorService(null,
       this.AzureContentModeratorConfiguration, new Microservices.Library.Interceptors.CustomHttpClient(
       new Microservices.Library.Interceptors.CustomHttpClientHandler(null)));
    var result = await azureContentModeratorService.AnalyzeTextAsync(ModeratingSpanishText,
       AzureContentModeratorService.TextType.PlainText, 
       AzureContentModeratorService.TextLanguage.Spanish);

## Analyze Image
    AzureContentModeratorService azureContentModeratorService =
       new AzureContentModeratorService(null,
       this.AzureContentModeratorConfiguration, new Microservices.Library.Interceptors.CustomHttpClient(
       new Microservices.Library.Interceptors.CustomHttpClientHandler(null)));
    Stream imageStream = await new HttpClient().GetStreamAsync(ModeratingImageUrl);
    var result = await azureContentModeratorService.AnalyzeImageAsync(imageStream);