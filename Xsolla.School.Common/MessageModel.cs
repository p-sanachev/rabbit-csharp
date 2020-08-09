using System;
using System.Text.Json.Serialization;

namespace Xsolla.School.Common
{
    public class MessageModel
    {
        [JsonPropertyName("uri")]
        public string Uri { get; set; }
    }
}