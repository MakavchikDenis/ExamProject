using System.Text.Json.Serialization;

namespace LibraryModels.Repository
{
    public class SessionApi
    {
        [JsonPropertyName("access_token")]
        public string Access_token { get; set; }
        [JsonPropertyName("token_type")]
        public string Token_type { get; set; }
        [JsonPropertyName("refresh_token")]
        public string Refresh_token { get; set; }
        [JsonPropertyName("expires_in")]
        public int Expires_in { get; set; }
    }
}
