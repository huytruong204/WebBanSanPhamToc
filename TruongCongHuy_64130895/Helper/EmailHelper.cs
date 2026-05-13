using MailKit.Net.Smtp;
using MimeKit;
using System.IO;

namespace TruongCongHuy_64130895.Helper
{
    public class EmailHelper
    {
        private readonly IConfiguration _configuration;
        private readonly string emailType;

        public EmailHelper(IConfiguration configuration, string emailType)
        {
            _configuration = configuration;
            this.emailType = emailType; 
        }

        public (bool isSuccess, string errorMessage) SendEmail(string userEmail, string userName, string link)
        {
            try
            {
                var emailSettings = _configuration.GetSection("EmailSettings");
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("Shop Online Huy", emailSettings["From"]));
                message.To.Add(new MailboxAddress("", userEmail));
                message.Subject = emailType == "Activation" ? "Kích hoạt tài khoản" : "Đặt lại mật khẩu";

                string templatePath;
                if (emailType == "Activation")
                {
                    templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Templates", "ActivateAccount.html");
                }
                else if (emailType == "ForgotPassword")
                {
                    templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Templates", "ForgotPassword.html");
                }
                else
                {
                    return (false, "Loại email không hợp lệ (Invalid email type)");
                }

                // BẮT LỖI FILE TEMPLATE
                if (!System.IO.File.Exists(templatePath))
                {
                    return (false, $"Không tìm thấy file giao diện mail tại đường dẫn: {templatePath}");
                }

                var htmlContent = System.IO.File.ReadAllText(templatePath);
                htmlContent = htmlContent.Replace("{Name}", userName)
                                         .Replace("{ConfirmationLink}", link)
                                         .Replace("{ResetLink}", link);

                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = htmlContent,
                    TextBody = $"Xin chào {userName}, vui lòng truy cập liên kết sau: {link}"
                };
                message.Body = bodyBuilder.ToMessageBody();

                using (var client = new SmtpClient())
                {
                    client.Connect(emailSettings["Host"], int.Parse(emailSettings["Port"]), MailKit.Security.SecureSocketOptions.StartTls);
                    client.Authenticate(emailSettings["From"], emailSettings["Password"]);
                    client.Send(message);
                    client.Disconnect(true);
                }
                return (true, string.Empty);
            }
            catch (Exception ex)
            {
                return (false, ex.Message); 
            }
        }
    }
}
