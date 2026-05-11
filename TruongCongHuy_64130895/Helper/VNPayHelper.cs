using Microsoft.AspNetCore.Components.Forms;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using TruongCongHuy_64130895.Controllers;

namespace TruongCongHuy_64130895.Helper
{
    public class VNPayHelper
    {
        private readonly IConfiguration configuration;

        public VNPayHelper(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        
        public string CreatePaymentUrl(int orderId, decimal amount, HttpContext context)
        {
            string tmnCode = configuration["VNPAY:TmnCode"];
            string hashSecret = configuration["VNPAY:HashSecret"];
            string url = configuration["VNPAY:Url"];
            string returnUrl = configuration["VNPAY:ReturnUrl"];

            var vnpay = new VnPayLibrary();
            vnpay.AddRequestData("vnp_Version", "2.1.0");
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", tmnCode);
            vnpay.AddRequestData("vnp_Amount", ((int)amount * 100).ToString());
            vnpay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", vnpay.GetIpAddress(context));
            vnpay.AddRequestData("vnp_Locale", "vn");
            vnpay.AddRequestData("vnp_OrderInfo", $"Thanh toán đơn hàng {orderId}");
            vnpay.AddRequestData("vnp_OrderType", "other");
            vnpay.AddRequestData("vnp_ReturnUrl", returnUrl);
            vnpay.AddRequestData("vnp_TxnRef", orderId.ToString());

            string paymentUrl = vnpay.CreateRequestUrl(url, hashSecret);
            Console.WriteLine(paymentUrl);
            return paymentUrl;
        }

        public VNPayResponse ProcessReturnRequest(IQueryCollection queryParams)
        {
            string hashSecret = configuration["VNPAY:HashSecret"];

            var vnpayLibrary = new VnPayLibrary();

            foreach (var kvp in queryParams.Where(kvp => kvp.Key.StartsWith("vnp_")))
            {
                vnpayLibrary.AddResponseData(kvp.Key, kvp.Value.ToString());
            }

            if (!queryParams.TryGetValue("vnp_SecureHash", out var secureHash))
            {
                return new VNPayResponse
                {
                    Success = false,
                    Message = "Thiếu thông tin vnp_SecureHash."
                };
            }

            
            bool isSignatureValid = vnpayLibrary.ValidateSignature(secureHash, hashSecret);
            if (isSignatureValid)
            {
                string responseCode = vnpayLibrary.GetResponseData("vnp_ResponseCode");
                bool success = responseCode == "00"; 

                return new VNPayResponse
                {
                    Success = success,
                    Message = success ? "Thanh toán thành công!" : $"Lỗi thanh toán: Mã lỗi {responseCode}."
                };
            }

            return new VNPayResponse
            {
                Success = false,
                Message = "Sai chữ ký!"
            };
        }



        public class VNPayResponse
        {
            public bool Success { get; set; }
            public string Message { get; set; }
        }
    }




}
