namespace TruongCongHuy_64130895.Helper
{
    public static class ImageHelper
    {
        public static string UploadImage(IFormFile imageFile, string folder)
        {
            if (imageFile == null || imageFile.Length == 0)
            {
                return string.Empty;
            }

            try
            {
                // Tạo tên file duy nhất kết hợp thời gian và GUID
                var fileExtension = Path.GetExtension(imageFile.FileName);
                var fileName = $"{DateTime.Now:yyyyMMddHHmmss}_{Guid.NewGuid()}{fileExtension}";

                // Đường dẫn lưu file
                var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", folder);

                // Tạo thư mục nếu chưa tồn tại
                if (!Directory.Exists(fullPath))
                {
                    Directory.CreateDirectory(fullPath);
                }

                var filePath = Path.Combine(fullPath, fileName);

                // Upload file
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    imageFile.CopyTo(fileStream);
                }

                return fileName;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error uploading image: {ex.Message}");
                return string.Empty;
            }
        }
    }

}

