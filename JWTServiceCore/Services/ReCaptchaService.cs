using JWTServiceCore.Services.Interfaces;
using JWTServiceCore.Tools;
using Newtonsoft.Json;

namespace JWTServiceCore.Services
{
    public class ReCaptchaService : IReCaptchaService
    {
        #region Ctor

        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ReCaptchaService(
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        #endregion

        public async Task<bool> IsCaptchaVerifiedAsync()
        {
            var http = new HttpClient();

            var secretKey = _configuration.GetSection("GoogleRecaptcha:SecretKey");

            var response = _httpContextAccessor.HttpContext?.Request.Form["g-recaptcha-response"];

            var result = await http.PostAsync($"https://www.google.com/recaptcha/api/siteverify?secret={secretKey.Value}&response={response}", null);
            if (result.IsSuccessStatusCode)
            {
                var googleReCaptcha = JsonConvert.DeserializeObject<ReCaptchaResponse>(await result.Content.ReadAsStringAsync());
                if (googleReCaptcha == null)
                    return false;

                return googleReCaptcha.Success;
            }

            return false;
        }
    }
}