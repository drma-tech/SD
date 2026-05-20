using System.Text;
using System.Text.Json;

namespace SD.API.Core
{
    public class ZeptoMailClient(string apiKey)
    {
        private readonly HttpClient _httpClient = new();
        private readonly string _apiKey = apiKey;

        public async Task SendOtpEmail(string toEmail, string reference, string? otp, CancellationToken cancellationToken)
        {
            var appName = "Streaming Discovery";
            var supportEmail = "support@streamingdiscovery.com";

            var payload = new
            {
                from = new { address = "noreply@drma-tech.com", name = "DRMA Tech" },
                to = new[] { new { email_address = new { address = toEmail, name = "" } } },
                subject = "DRMA Tech - Your OTP Code",
                htmlbody = @$"<table cellspacing=""0"" cellpadding=""0"" style="" width: 100%;font-size: 14px;"">
                    <tbody>
                        <tr>
                            <td style=""padding:32px"">
                                <div>
                                    <h1 style=""margin: 0 0 32px;font-size:20px;text-align:center""><span>{appName}</span>
                                    </h1>
                                </div>
                                <div style=""background: #fff;border-radius: 10px;overflow: hidden;border: solid 1px #E5E5E5;border-radius: 10px;"">
                                    <table cellspacing=""0"" cellpadding=""0"" style=""width:100%;font-size: 14px;"">
                                        <tbody>
                                            <tr>
                                                <td>
                                                    <div style=""padding: 32px 26px;text-align: center;"">
                                                        <h2 style=""margin:0 0 20px;font-size: 20px;"">Verification code</h2>
                                                        <p style=""font-size: 14px; margin: 0;line-height: 1.6;"">
                                                        Enter the below one time password to verify your {appName} account:
                                                        </p>
                                                        <p style=""font-size: 24px; margin: 24px 0;color:#264DED"">{otp}</p>
                                                        <p style=""font-size: 13px; margin: 0;line-height: 1.5;"">The verification code expires in <span style=""color: #AA2222"">10 min</span></p>

                                                        <hr style=""border-style: dashed;border-width: 1px 0 0;border-color: #CECECE;margin: 24px 0"">

                                                        <p style=""font-size:14px;line-height: 1.6;margin: 24px 0 0"">If you have further questions, write to us at <a href=""#"" style=""color:#006CFF;text-decoration: none;"">{supportEmail}</a> and our team will get back to you.</p>
                                                        <div style=""margin-top: 32px;line-height: 1.6;"">
                                                            <p style=""font-size: 13px; margin: 0;"">Have a great day!</p>
                                                            <h3 style=""font-size: 15px; margin: 4px 0 0;"">Team DRMA Tech</h3>
                                                        </div>
                                                    </div>
                                                </td>
                                            </tr>
                                        </tbody>
                                    </table>
                                </div>
                            </td>
                        </tr>
                    </tbody>
                </table>",
                client_reference = reference
            };

            var json = JsonSerializer.Serialize(payload);

            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.zeptomail.com/v1.1/email");

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
