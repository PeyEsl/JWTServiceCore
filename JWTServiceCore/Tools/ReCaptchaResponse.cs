using Newtonsoft.Json;

namespace JWTServiceCore.Tools
{
    public class ReCaptchaResponse
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("challenge_ts")]
        public DateTimeOffset ChallengeTs { get; set; }

        [JsonProperty("hostname")]
        public string? HostName { get; set; }
    }
}
