using Application.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Services
{
    using System.Security.Cryptography;
    using System.Text;

    public class PasswordHasher : IPasswordHasher
    {
        private const int SaltSize = 16; // 128-bit salt

        // Hash the password using SHA512 with a salt
        public string HashPassword(string password)
        {
            // Generate a random salt
            var salt = new byte[SaltSize];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }

            // Combine the password and salt and hash them
            using (var sha512 = SHA512.Create())
            {
                var saltedPassword = Combine(password, salt);
                var hash = sha512.ComputeHash(saltedPassword);

                // Combine salt and hash into a single array
                var saltedHash = new byte[salt.Length + hash.Length];
                Buffer.BlockCopy(salt, 0, saltedHash, 0, salt.Length);
                Buffer.BlockCopy(hash, 0, saltedHash, salt.Length, hash.Length);

                // Return as base64 string for storage
                return Convert.ToBase64String(saltedHash);
            }
        }

        // Verify the password using the stored hashed password
        public bool VerifyPassword(string password, string storedHashedPassword)
        {
            // Decode the stored hash
            var saltedHashBytes = Convert.FromBase64String(storedHashedPassword);

            // Extract the salt (first 16 bytes)
            var salt = new byte[SaltSize];
            Buffer.BlockCopy(saltedHashBytes, 0, salt, 0, SaltSize);

            // Hash the incoming password with the extracted salt
            using (var sha512 = SHA512.Create())
            {
                var saltedPassword = Combine(password, salt);
                var incomingHash = sha512.ComputeHash(saltedPassword);

                // Compare the stored hash (after the salt) with the incoming hash
                for (var i = 0; i < incomingHash.Length; i++)
                {
                    if (saltedHashBytes[SaltSize + i] != incomingHash[i])
                    {
                        return false; // Passwords do not match
                    }
                }
            }

            return true; // Passwords match
        }

        // Helper method to combine password and salt into a single byte array
        private byte[] Combine(string password, byte[] salt)
        {
            var passwordBytes = Encoding.UTF8.GetBytes(password);
            var saltedPassword = new byte[salt.Length + passwordBytes.Length];
            Buffer.BlockCopy(salt, 0, saltedPassword, 0, salt.Length);
            Buffer.BlockCopy(passwordBytes, 0, saltedPassword, salt.Length, passwordBytes.Length);
            return saltedPassword;
        }
    }


}
