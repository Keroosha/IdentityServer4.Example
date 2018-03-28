using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;

namespace Backend.Users.Services.Security
{
    [SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global")]
    public class PasswordHasherOptions
    {
        private static readonly RandomNumberGenerator _defaultRng = RandomNumberGenerator.Create();

        public int IterationCount { get; set; } = 10000;

        internal RandomNumberGenerator Rng { get; set; } = _defaultRng;
    }
}