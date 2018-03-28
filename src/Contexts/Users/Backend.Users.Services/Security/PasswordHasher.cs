using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using Backend.Users.Domain.Aggregates.UserAggregate;
using Backend.Users.Domain.Services;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace Backend.Users.Services.Security
{
    public class PasswordHasher : IPasswordHasher, IPasswordValidator
    {
        private readonly int _iterCount;
        private readonly RandomNumberGenerator _rng;

        public PasswordHasher(PasswordHasherOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));
            _iterCount = options.IterationCount;
            _rng = options.Rng;
        }

        public string ComputeHash(string password)
        {
            var passwordBytes = HashPasswordInternal(password, _rng);
            var passwordHash = Convert.ToBase64String(passwordBytes);
            return passwordHash;
        }

        public bool IsPasswordValid(User user, string providedPassword)
        {
            return VerifyPassword(providedPassword, user.PasswordHash);
        }

        private bool VerifyPassword(string providedPassword, string passwordHash)
        {
            var providedPasswordHashBytes = Convert.FromBase64String(passwordHash);
            var isValid = VerifyHashedPasswordInternal(providedPasswordHashBytes, providedPassword);
            return isValid;
        }

        private byte[] HashPasswordInternal(string password, RandomNumberGenerator rng)
        {
            return HashPasswordInternal(
                password,
                rng,
                KeyDerivationPrf.HMACSHA256,
                _iterCount,
                128 / 8,
                256 / 8);
        }

        private static byte[] HashPasswordInternal(
            string password,
            RandomNumberGenerator rng, KeyDerivationPrf prf,
            int iterCount,
            int saltSize,
            int numBytesRequested)
        {
            // Produce a version 3 (see comment above) text hash.
            var salt = new byte[saltSize];
            rng.GetBytes(salt);
            var subkey = KeyDerivation.Pbkdf2(password, salt, prf, iterCount, numBytesRequested);

            var outputBytes = new byte[13 + salt.Length + subkey.Length];
            outputBytes[0] = 0x01; // format marker
            WriteNetworkByteOrder(outputBytes, 1, (uint) prf);
            WriteNetworkByteOrder(outputBytes, 5, (uint) iterCount);
            WriteNetworkByteOrder(outputBytes, 9, (uint) saltSize);
            Buffer.BlockCopy(salt, 0, outputBytes, 13, salt.Length);
            Buffer.BlockCopy(subkey, 0, outputBytes, 13 + saltSize, subkey.Length);
            return outputBytes;
        }

        private static bool VerifyHashedPasswordInternal(byte[] hashedPassword, string password)
        {
            try
            {
                // Read header information
                var prf = (KeyDerivationPrf) ReadNetworkByteOrder(hashedPassword, 1);
                var iterCount = (int) ReadNetworkByteOrder(hashedPassword, 5);
                var saltLength = (int) ReadNetworkByteOrder(hashedPassword, 9);

                // Read the salt: must be >= 128 bits
                if (saltLength < 128 / 8) return false;

                var salt = new byte[saltLength];
                Buffer.BlockCopy(hashedPassword, 13, salt, 0, salt.Length);

                // Read the subkey (the rest of the payload): must be >= 128 bits
                var subkeyLength = hashedPassword.Length - 13 - salt.Length;
                if (subkeyLength < 128 / 8) return false;

                var expectedSubkey = new byte[subkeyLength];
                Buffer.BlockCopy(hashedPassword, 13 + salt.Length, expectedSubkey, 0, expectedSubkey.Length);

                // Hash the incoming password and verify it
                var actualSubkey = KeyDerivation.Pbkdf2(password, salt, prf, iterCount, subkeyLength);
                return ByteArraysEqual(actualSubkey, expectedSubkey);
            }
            catch
            {
                // This should never occur except in the case of a malformed payload, where
                // we might go off the end of the array. Regardless, a malformed payload
                // implies verification failed.
                return false;
            }
        }

        // Compares two byte arrays for equality. The method is specifically written so that the loop is not optimized.
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private static bool ByteArraysEqual(byte[] a, byte[] b)
        {
            if (a == null && b == null) return true;

            if (a == null || b == null || a.Length != b.Length) return false;

            var areSame = true;
            for (var i = 0; i < a.Length; i++) areSame &= a[i] == b[i];

            return areSame;
        }

        [SuppressMessage("ReSharper", "RedundantCast")]
        [SuppressMessage("ReSharper", "ArrangeRedundantParentheses")]
        private static uint ReadNetworkByteOrder(byte[] buffer, int offset)
        {
            return ((uint) (buffer[offset + 0]) << 24)
                   | ((uint) (buffer[offset + 1]) << 16)
                   | ((uint) (buffer[offset + 2]) << 8)
                   | ((uint) (buffer[offset + 3]));
        }


        private static void WriteNetworkByteOrder(byte[] buffer, int offset, uint value)
        {
            buffer[offset + 0] = (byte) (value >> 24);
            buffer[offset + 1] = (byte) (value >> 16);
            buffer[offset + 2] = (byte) (value >> 8);
            buffer[offset + 3] = (byte) (value >> 0);
        }
    }
}