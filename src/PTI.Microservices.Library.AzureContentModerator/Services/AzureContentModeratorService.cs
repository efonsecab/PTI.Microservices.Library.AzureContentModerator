using Microsoft.Azure.CognitiveServices.ContentModerator;
using Microsoft.Azure.CognitiveServices.ContentModerator.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PTI.Microservices.Library.Configuration;
using PTI.Microservices.Library.Interceptors;
using PTI.Microservices.Library.Models.AzureContentModeratorService;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PTI.Microservices.Library.Services
{
    /// <summary>
    /// Service in cahrge of exposing access to Azure Content Moderator
    /// </summary>
    public sealed class AzureContentModeratorService
    {
        private ILogger<AzureContentModeratorService> Logger { get; }
        private AzureContentModeratorConfiguration AzureContentModeratorConfiguration { get; }
        private CustomHttpClient CustomHttpClient { get; }
        private ContentModeratorClient ContentModeratorClient { get; }

        /// <summary>
        /// Createsa new instance of <see cref="AzureContentModeratorService"/>
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="azureContentModeratorConfiguration"></param>
        /// <param name="customHttpClient"></param>
        public AzureContentModeratorService(ILogger<AzureContentModeratorService> logger,
            AzureContentModeratorConfiguration azureContentModeratorConfiguration,
            CustomHttpClient customHttpClient)
        {
            this.Logger = logger;
            this.AzureContentModeratorConfiguration = azureContentModeratorConfiguration;
            this.CustomHttpClient = customHttpClient;
            this.ContentModeratorClient =
                new ContentModeratorClient(new ApiKeyServiceClientCredentials(this.AzureContentModeratorConfiguration.Key),
                customHttpClient, false)
                {
                    Endpoint=this.AzureContentModeratorConfiguration.Endpoint
                };
        }

        /// <summary>
        /// Text Type
        /// </summary>
        public enum TextType
        {
            /// <summary>
            /// Plain Text
            /// </summary>
            PlainText,
            /// <summary>
            /// HHtml
            /// </summary>
            Html,
            /// <summary>
            /// Xml
            /// </summary>
            Xml,
            /// <summary>
            /// Markdown
            /// </summary>
            Markdown
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="textType"></param>
        /// <param name="textLanguage">
        /// Use "auto" for autodetecting the language, otherwise, specify a supported language
        /// https://docs.microsoft.com/en-us/azure/cognitive-services/content-moderator/language-support
        /// </param>
        /// <param name="cancellationToken"></param>
        /// <returns>
        /// Check categories info here: 
        /// https://docs.microsoft.com/en-us/azure/cognitive-services/content-moderator/text-moderation-api#classification
        /// </returns>
        public async Task<AnalyzeTextExpandedResponse> AnalyzeTextAsync(string text, TextType textType, 
            string textLanguage="auto", CancellationToken cancellationToken=default )
        {
            try
            {
                string contentType = GetContentTypeString(textType);
                string language = string.Empty;
                if (textLanguage == "auto")
                {
                    language = await this.DetectLanguageAsync(text, textType, cancellationToken);
                }
                byte[] requestBytes = Encoding.UTF8.GetBytes(text);
                MemoryStream requestStream = new MemoryStream(requestBytes);
                var response = await
                this.ContentModeratorClient.TextModeration.ScreenTextAsync(contentType, requestStream, language,
                    autocorrect: false, pII: true, classify: true, cancellationToken: cancellationToken);
                AnalyzeTextExpandedResponse result = JsonConvert.DeserializeObject<AnalyzeTextExpandedResponse>(
                    JsonConvert.SerializeObject(response));
                /*
                 * Category1 refers to potential presence of language that may be considered sexually explicit or adult in certain situations.
                 * Category2 refers to potential presence of language that may be considered sexually suggestive or mature in certain situations.
                 * Category3 refers to potential presence of language that may be considered offensive in certain situations.
                 * Score is between 0 and 1. The higher the score, the higher the model is predicting that the category may be applicable. This feature relies on a statistical model rather than manually coded outcomes. We recommend testing with your own content to determine how each category aligns to your requirements.
                 * ReviewRecommended is either true or false depending on the internal score thresholds. Customers should assess whether to use this value or decide on custom thresholds based on their content policies.
                 * */
                return result;
            }
            catch (Exception ex)
            {
                this.Logger?.LogError(ex, ex.Message);
                throw;
            }
        }

        private static string GetContentTypeString(TextType textType)
        {
            string contentType = string.Empty;
            switch (textType)
            {
                case TextType.Html: contentType = "text/html"; break;
                case TextType.Markdown: contentType = "text/markdown"; break;
                case TextType.PlainText: contentType = "text/plain"; break;
                case TextType.Xml: contentType = "text/xml"; break;
            }

            return contentType;
        }

        /// <summary>
        /// Evaluates a specified image 
        /// </summary>
        /// <param name="imageStream"></param>
        /// <returns></returns>
        public async Task<Evaluate> AnalyzeImageAsync(Stream imageStream)
        {
            try
            {
                var response = await
                this.ContentModeratorClient.ImageModeration.EvaluateFileInputAsync(imageStream);
                return response;
            }
            catch (Exception ex)
            {
                this.Logger?.LogError(ex.Message, ex);
                throw;
            }
        }

        public async Task<string> DetectLanguageAsync(string text, TextType textType, CancellationToken cancellationToken=default)
        {
            try
            {
                var textContentType = GetContentTypeString(textType);
                byte[] requestBytes = Encoding.UTF8.GetBytes(text);
                MemoryStream requestStream = new MemoryStream(requestBytes);
                var response = await this.ContentModeratorClient.TextModeration
                    .DetectLanguageAsync(textContentType, requestStream, cancellationToken);
                if (response.Status.Description != "OK")
                {
                    if (response.Status.Exception != null)
                        throw new Exception(response.Status.Exception);
                    else
                        throw new Exception(response.Status.Description);
                }
                return response.DetectedLanguageProperty;
            }
            catch (Exception ex)
            {
                this.Logger?.LogError(ex.Message, ex);
                throw;
            }
        }
    }
}
