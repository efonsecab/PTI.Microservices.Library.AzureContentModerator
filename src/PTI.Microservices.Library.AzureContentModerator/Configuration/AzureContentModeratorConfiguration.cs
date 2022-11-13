using System;
using System.Collections.Generic;
using System.Text;

namespace PTI.Microservices.Library.Configuration
{
    /// <summary>
    /// Azure Content Moderator Configuration
    /// </summary>
    public class AzureContentModeratorConfiguration
    {
        /// <summary>
        /// Service endpoint
        /// </summary>
        public string Endpoint { get; set; }
        /// <summary>
        /// Service Access Key
        /// </summary>
        public string Key { get; set; }
    }
}
