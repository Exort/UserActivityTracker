using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace UserActivityTracker.Utils
{
    public static class PasswordUtils
    {
        // Security parameters (aligned with modern recommendations)
        private const int SaltSize = 16;        // 128-bit salt
        private const int KeySize = 32;         // 256-bit hash output
        private const int Iterations = 600000;  // Work factor / iteration count
        private static readonly HashAlgorithmName HashAlgorithm = HashAlgorithmName.SHA256;

        private const char Delimiter = '.';

        public static string GetSaltedPassword(string password)
        {
            // 1. Generate a cryptographically secure random salt
            byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);

            // 2. Derive the hash using PBKDF2
            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                Iterations,
                HashAlgorithm,
                KeySize);

            // 3. Format as: {iterations}.{salt_base64}.{hash_base64}
            return $"{Iterations}{Delimiter}{Convert.ToBase64String(salt)}{Delimiter}{Convert.ToBase64String(hash)}";
        }

        /// <summary>
        /// Verifies a plain-text password against a stored hashed password string.
        /// </summary>
        public static bool VerifyPassword(string password, string storedHash)
        {
            try
            {
                // Parse the stored components
                string[] parts = storedHash.Split(Delimiter);
                if (parts.Length != 3) return false;

                int iterations = int.Parse(parts[0]);
                byte[] salt = Convert.FromBase64String(parts[1]);
                byte[] hash = Convert.FromBase64String(parts[2]);

                // Hash the incoming password attempt with the extracted salt and parameters
                byte[] computedHash = Rfc2898DeriveBytes.Pbkdf2(
                    password,
                    salt,
                    iterations,
                    HashAlgorithm,
                    hash.Length);

                // Compare hashes securely in constant time to prevent timing side-channel attacks
                return CryptographicOperations.FixedTimeEquals(computedHash, hash);
            }
            catch
            {
                // Fail safely if the stored string format is corrupted or invalid
                return false;
            }
        }
    }
}
