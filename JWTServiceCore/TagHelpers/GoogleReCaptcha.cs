using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Text.Encodings.Web;

namespace JWTServiceCore.TagHelpers
{
    public class GoogleReCaptcha : TagHelper
    {
        #region Ctor

        private readonly IConfiguration _configuration;

        public GoogleReCaptcha(
            IConfiguration configuration)
        {
            _configuration = configuration;
        }

        #endregion

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var siteKey = _configuration.GetSection("GoogleRecaptcha:SiteKey");

            output.TagName = "div";
            output.AddClass("g-recaptcha", HtmlEncoder.Default);
            output.Attributes.Add("data-sitekey", siteKey.Value);

            base.Process(context, output);
        }
    }
}