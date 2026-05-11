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

        public bool SendEmail(string userEmail, string userName, string link)
        {
            try
            {
                var emailSettings = _configuration.GetSection("EmailSettings");

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("Your App Name", emailSettings["From"]));
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
                    throw new Exception("Invalid email type");
                }

                // Load và thay thế nội dung template
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
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email: {ex.Message}");
                return false;
            }
        }
    }
}
