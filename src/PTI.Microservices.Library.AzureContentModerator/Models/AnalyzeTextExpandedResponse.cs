using Microsoft.Azure.CognitiveServices.ContentModerator.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PTI.Microservices.Library.Models.AzureContentModeratorService
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class AnalyzeTextExpandedResponse: Screen
    {
        /*
         * Category1 refers to potential presence of language that may be considered sexually explicit or adult in certain situations.
         * Category2 refers to potential presence of language that may be considered sexually suggestive or mature in certain situations.
         * Category3 refers to potential presence of language that may be considered offensive in certain situations.
         * Score is between 0 and 1. The higher the score, the higher the model is predicting that the category may be applicable. 
         * This feature relies on a statistical model rather than manually coded outcomes. 
         * We recommend testing with your own content to determine how each category aligns to your requirements.
         * ReviewRecommended is either true or false depending on the internal score thresholds. Customers should assess whether to use this value or decide on custom thresholds based on their content policies.
         * */
        public bool IsSexuallyExplicit
        {
            get {
                return (this.Classification?.Category1.Score > 0.5);
            }
        }

        public bool IsSexuallySuggestive
        {
            get
            {
                return (this.Classification?.Category2.Score > 0.5);
            }
        }

        public bool IsOffensive
        {
            get
            {
                return (this.Classification?.Category3.Score > 0.5);
            }
        }

        public bool HasProfanityTerms
        {
            get
            {
                return this.Profanity?.Count > 0;
            }
        }

        public IList<DetectedTerms> Profanity
        {
            get
            {
                return this.Terms;
            }
        }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
