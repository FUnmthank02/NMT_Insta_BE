using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Helper
{
    public class PasswordHelper
    {
        public static string HashPassword(string password)
        {
            // Tạo salt ngẫu nhiên
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);

            // Sử dụng PBKDF2 để mã hóa mật khẩu
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
            byte[] hash = pbkdf2.GetBytes(20);

            // Kết hợp salt và hash để lưu trữ trong cơ sở dữ liệu
            byte[] hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);

            // Chuyển đổi thành chuỗi Base64 để lưu trữ
            string savedPasswordHash = Convert.ToBase64String(hashBytes);

            return savedPasswordHash;
        }

        public static bool VerifyPassword(string password, string savedPasswordHash)
        {
            // Chuyển đổi chuỗi Base64 thành byte array
            byte[] hashBytes = Convert.FromBase64String(savedPasswordHash);

            // Lấy salt từ hashBytes
            byte[] salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);

            // Sử dụng PBKDF2 để tính toán hash từ password và salt
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
            byte[] hash = pbkdf2.GetBytes(20);

            // Kiểm tra hash tính toán và hash lưu trữ có khớp nhau không
            for (int i = 0; i < 20; i++)
            {
                if (hashBytes[i + 16] != hash[i])
                {
                    return false;
                }
            }
            return true;
        }
    }
}
