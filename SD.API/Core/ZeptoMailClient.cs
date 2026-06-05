using System.Text;
using System.Text.Json;

namespace SD.API.Core
{
    public class ZeptoMailClient(string apiKey)
    {
        private readonly HttpClient _httpClient = new();
        private readonly string _apiKey = apiKey;
        private readonly string _uri = "https://api.zeptomail.com/v1.1/email";

        private const string domain = "streamingdiscovery";
        private const string appName = "Streaming Discovery";
        private const string supportEmail = $"support@{domain}.com";
        private static string year => DateTime.Now.Year.ToString();

        private static readonly string CssBase = @"
        /* BASE */
        body { margin: 0; padding: 0; background: #f3f5f7; font-family: Arial, Helvetica, sans-serif; color: #111827; }
        * { box-sizing: border-box; }
        a { text-decoration: none; }
        .email-wrapper { padding: 24px 12px; background: #f3f5f7; }
        .email-container { max-width: 600px; margin: 0 auto; background: #ffffff; border: 1px solid #e5e7eb; border-radius: 18px; overflow: hidden; }
        /* HEADER */
        .email-header { background-color: #5a4be2; padding: 16px 16px; text-align: center; }
        .email-logo { width: 48px; height: 48px; }
        .email-app-name { margin: 0; color: #ffffff; font-size: 30px; font-weight: 700; }
        .email-tagline { margin-top: 10px; color: rgba(255,255,255,0.85); font-size: 14px; }
        /* CONTENT */
        .email-content { padding: 24px 16px; text-align: center; }
        .email-title { margin: 0 0 20px; font-size: 28px; color: #111827; }
        .email-text { margin: 0 0 20px; font-size: 15px; line-height: 1.7; color: #4b5563; }
        .email-highlight { color: #5a4be2; font-weight: 700; }
        .email-button { display: inline-block; margin-top: 16px; padding: 14px 22px; background: #5a4be2; color: #ffffff !important; border-radius: 10px; font-size: 15px; font-weight: 700; }
        .email-divider { margin: 32px 0; border: 0; border-top: 1px solid #e5e7eb; }
        .email-support { font-size: 14px; color: #6b7280; }
        .email-support a { color: #5a4be2; font-weight: 600; }
        .email-signature { margin-top: 24px; }
        .email-signature small { display: block; color: #9ca3af; font-size: 13px; margin-bottom: 6px; }
        .email-signature strong { font-size: 15px; color: #111827; }
        /* FOOTER */
        .email-footer { padding: 16px 16px; background: #eaebed; border-top: 1px solid #e5e7eb; text-align: center; }
        .footer-socials { margin-bottom: 16px; display: none; }
        .footer-socials a { display: inline-block; margin: 0 4px; }
        .footer-socials img { width: 20px; height: 20px; }
        .footer-apps { margin-bottom: 16px; }
        .footer-apps a { display: inline-block; margin: 0 4px; }
        .footer-apps img { height: 20px; margin: 0 6px; vertical-align: bottom; }
        .footer-links { margin-bottom: 16px; }
        .footer-links a { margin: 0 10px; font-size: 13px; color: #6b7280; }
        .footer-copy { font-size: 12px; color: #9ca3af; line-height: 1.6; }
        @media only screen and (max-width: 600px) {
        .email-title { font-size: 24px; }
        .email-app-name { font-size: 26px; }
        }";

        private static readonly string CssSections = ".email-section { margin-top: 32px; } .section-title { margin-bottom: 16px; font-size: 18px; font-weight: 700; color: #5a4be2; }";
        private static readonly string CssQuickLinks = ".link-grid { text-align: center; } .link-card { display: inline-block; margin: 4px; padding: 8px 12px; background: #6b728024; border: 1px solid #e5e7eb; border-radius: 999px; color: #ff4081 !important; font-size: 14px; font-weight: 600; transition: 0.2s ease; }";
        private static readonly string CssProducts = ".product-list { margin-top: 12px; } .product-card { display: block; margin-bottom: 10px; padding: 10px; background: #6b728024; border: 1px solid #e5e7eb; border-radius: 12px; text-align: left; } .product-name { margin-bottom: 4px; color: #ff4081; font-size: 15px; font-weight: 700; } .product-name img { height: 20px; margin: 0 6px; vertical-align: bottom; } .product-description { color: #6b7280; font-size: 14px; line-height: 1.5; }";

        private static readonly string CssOtp = ".otp { display: inline-block; padding: 16px; margin: 12px 0; background: #eff6ff; border: 1px solid #bfdbfe; border-radius: 14px; font-size: 36px; font-weight: 700; letter-spacing: 4px; color: #5a4be2; }";

        private static readonly string HtmlHeader = @$"
        <div class=""email-header"">
            <img class=""email-logo"" src=""https://www.{domain}.com/icon/icon-71.webp"" alt=""{appName} Logo"">
            <h1 class=""email-app-name"">
                {appName}
            </h1>
            <div class=""email-tagline"">
                Discover Movies and Series on Streaming Platforms
            </div>
        </div>";

        private static readonly string HtmlProducts = @$"
        <div class=""email-section"">
            <div class=""section-title"">
                More from DRMA Tech
            </div>
            <div class=""product-list"">
                <a href=""https://www.modern-matchmaker.com"" class=""product-card"" target=""_blank"">
                    <div class=""product-name"">
                        <img src=""https://www.{domain}.com/logo/modern-matchmaker.webp"" alt=""Modern Matchmaker"">Modern Matchmaker
                    </div>
                    <div class=""product-description"">
                        Find a compatible partner through Smart Matchmaking
                    </div>
                </a>
                <a href=""https://www.my-next-spot.com"" class=""product-card"" target=""_blank"">
                    <div class=""product-name"">
                        <img src=""https://www.{domain}.com/logo/next-spot.webp"" alt=""My Next Spot"">My Next Spot
                    </div>
                    <div class=""product-description"">
                        Find the Best Cities and Countries to Live or Travel
                    </div>
                </a>
                <a href=""https://www.web-standards.com"" class=""product-card"" target=""_blank"">
                    <div class=""product-name"">
                        <img src=""https://www.{domain}.com/logo/webstandards.webp"" alt=""Web Standards"">Web Standards
                    </div>
                    <div class=""product-description"">
                        Web Standards Generator for Websites and PWAs
                    </div>
                </a>
            </div>
        </div>";

        private static readonly string HtmlFooter = @$"
        <div class=""email-footer"">
            <!-- SOCIALS -->
            <div class=""footer-socials"">
                <a href=""{{instagramUrl}}"">
                    <img src=""{{instagramIcon}}"" alt=""Instagram"">
                </a>
                <a href=""{{twitterUrl}}"">
                    <img src=""{{twitterIcon}}"" alt=""Twitter"">
                </a>
                <a href=""{{youtubeUrl}}"">
                    <img src=""{{youtubeIcon}}"" alt=""YouTube"">
                </a>
            </div>

            <!-- APP STORES -->
            <div class=""footer-apps"">
                <a href=""https://apps.microsoft.com/detail/9pb1pkrdd8l0"" target=""_blank"">
                    <img src=""https://www.{domain}.com/logo/microsoft-store.webp"" alt=""Microsoft Store"">Microsoft Store
                </a>
                <a href=""https://play.google.com/store/apps/details?id=com.streamingdiscovery.www.twa"" target=""_blank"">
                    <img src=""https://www.{domain}.com/logo/google-play.webp"" alt=""Google Play"">Google Play
                </a>
                <a href=""https://apps.apple.com/us/app/id6749285238"" target=""_blank"">
                    <img src=""https://www.{domain}.com/logo/app-store.webp"" alt=""App Store"">App Store
                </a>
                <a href=""https://www.{domain}.com/en/help#get-the-app"" target=""_blank"">
                    <img src=""https://www.{domain}.com/logo/website-logo.webp"" alt=""More"">More
                </a>
            </div>

            <!-- LINKS -->
            <div class=""footer-links"">
                <a href=""https://www.{domain}.com"" target=""_blank"">
                    Website
                </a>
                <a href=""https://www.{domain}.com/legal/terms"" target=""_blank"">
                    Terms
                </a>
                <a href=""https://www.{domain}.com/legal/privacy"" target=""_blank"">
                    Privacy
                </a>
            </div>

            <!-- COPYRIGHT -->
            <div class=""footer-copy"">
                © {year} DRMA Tech.<br>
                All rights reserved.
            </div>
        </div>";

        public async Task SendOtpEmail(string toEmail, string reference, string? otp, CancellationToken cancellationToken)
        {
            var payload = new
            {
                from = new { address = "noreply@drma-tech.com", name = "DRMA Tech" },
                to = new[] { new { email_address = new { address = toEmail, name = "" } } },
                subject = "DRMA Tech - Your OTP Code",
                htmlbody = @$"
                 <!DOCTYPE html>
                <html lang=""en"">
                <head>
                    <meta charset=""utf-8"">
                    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                    <style>
                        {CssBase}
                        {CssOtp}
                    </style>
                </head>
                <body>
                    <div class=""email-wrapper"">
                        <div class=""email-container"">

                            <!-- HEADER -->
                            {HtmlHeader}

                            <!-- CONTENT -->
                            <div class=""email-content"">
                                <h2 class=""email-title"">
                                    Verification Code
                                </h2>
                               
                                <p>Use the code below to confirm your {appName} account.</p>
                                <div style=""text-align:center"">
                                    <div class=""otp"">{otp}</div>
                                </div>
                                <p>This code expires in <span style=""color: #ff4081; font-weight: bold;"">10 minutes</span>.</p>
                                                             
                                <hr class=""email-divider"">
                                <div class=""email-support"">
                                    Questions? Contact
                                    <a href=""mailto:{supportEmail}"">
                                        {supportEmail}
                                    </a>
                                </div>
                                <div class=""email-signature"">
                                    <small>Have a great day,</small>
                                    <strong>Team DRMA Tech</strong>
                                </div>
                            </div>

                            <!-- FOOTER -->
                            {HtmlFooter}
                        </div>
                    </div>
                </body>
                </html>",
                client_reference = reference
            };

            var json = JsonSerializer.Serialize(payload);

            var request = new HttpRequestMessage(HttpMethod.Post, _uri);

            request.Headers.Add("Authorization", _apiKey);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request, cancellationToken);

            var body = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                throw new NotificationException($"ZeptoMail error: {response.StatusCode} - {body}");
            }
        }

        public async Task SendWelcomeEmail(string toEmail, string reference, CancellationToken cancellationToken)
        {
            var payload = new
            {
                from = new { address = "noreply@drma-tech.com", name = "DRMA Tech" },
                to = new[] { new { email_address = new { address = toEmail, name = "" } } },
                subject = $"DRMA Tech - Welcome to {appName}",
                htmlbody = @$"
                 <!DOCTYPE html>
                <html lang=""en"">
                <head>
                    <meta charset=""utf-8"">
                    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                    <style>
                        {CssBase}
                        {CssSections}
                        {CssQuickLinks}
                        {CssProducts}
                    </style>
                </head>
                <body>
                    <div class=""email-wrapper"">
                        <div class=""email-container"">

                            <!-- HEADER -->
                            {HtmlHeader}

                            <!-- CONTENT -->
                            <div class=""email-content"">
                                <h2 class=""email-title"">
                                    Welcome to {appName}!
                                </h2>
                                <p class=""email-text"">
                                    Hello {toEmail.Split("@")[0]}, we are very glad you joined
                                    <span class=""email-highlight"">{appName}</span>.
                                </p>
                                <p class=""email-text"">
                                    Track your streaming platforms, manage your watch progress, and discover what to watch next with curated lists, award winners, and critic picks.
                                </p>
                                <!-- QUICK LINKS -->
                                <div class=""email-section"">
                                    <div class=""section-title"">
                                        Explore {appName}
                                    </div>
                                    <div class=""link-grid"">
                                        <a href=""https://www.{domain}.com/platforms"" class=""link-card"" target=""_blank"">
                                            Streaming Platforms
                                        </a>
                                        <a href=""https://www.{domain}.com/compare"" class=""link-card"" target=""_blank"">
                                            Compare Services
                                        </a>
                                        <a href=""https://www.{domain}.com/regions"" class=""link-card"" target=""_blank"">
                                            Free Movies to Watch
                                        </a>
                                        <a href=""https://www.{domain}.com/best-awards-year"" class=""link-card"" target=""_blank"">
                                            Best awards of the {year}
                                        </a>
                                        <a href=""https://www.{domain}.com/list/8544544"" class=""link-card"" target=""_blank"">
                                            Most Expected Movies of {year}
                                        </a>
                                        <a href=""https://www.{domain}.com/help"" class=""link-card"" target=""_blank"">
                                            Help Center
                                        </a>
                                    </div>
                                </div>
                                <hr class=""email-divider"">
                                <!-- OTHER PRODUCTS -->
                                {HtmlProducts}
                                <hr class=""email-divider"">
                                <div class=""email-support"">
                                    Questions? Contact
                                    <a href=""mailto:{supportEmail}"">
                                        {supportEmail}
                                    </a>
                                </div>
                                <div class=""email-signature"">
                                    <small>Have a great day,</small>
                                    <strong>Team DRMA Tech</strong>
                                </div>
                            </div>

                            <!-- FOOTER -->
                            {HtmlFooter}
                        </div>
                    </div>
                </body>
                </html>",
                client_reference = reference
            };

            var json = JsonSerializer.Serialize(payload);

            var request = new HttpRequestMessage(HttpMethod.Post, _uri);

            request.Headers.Add("Authorization", _apiKey);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request, cancellationToken);

            var body = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                throw new NotificationException($"ZeptoMail error: {response.StatusCode} - {body}");
            }
        }
    }
}
