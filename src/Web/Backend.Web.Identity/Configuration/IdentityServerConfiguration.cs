using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;

namespace Backend.Web.Identity.Configuration
{
    /// <summary>
    ///     Конфигурация IdentityServer.
    /// </summary>
    public class IdentityServerConfiguration
    {
        /// <summary>
        ///     ApiResource-ы, доступные IdentityServer.
        /// </summary>
        public ApiResource[] ApiResources { get; set; } = { };

        /// <summary>
        ///     IdentityResource-ы, доступные IdentityServer.
        /// </summary>
        public IdentityResource[] IdentityResources { get; set; } = { };

        /// <summary>
        ///     Клиентские приложения IdentityServer-а.
        /// </summary>
        public Client[] Clients { get; set; } = { };

        /// <summary>
        ///     Сертификат для подписи токенов IdentityServer-ом.
        /// </summary>
        public CertificateConfiguration SigningCertificate { get; set; }

        /// <summary>
        ///     Проверяет корректность конфигурации.
        /// </summary>
        /// <returns></returns>
        public bool IsValid()
        {
            return ApiResources?.LongLength > 0
                   && IdentityResources?.LongLength > 0
                   && Clients?.LongLength > 0
                   && SigningCertificate?.IsValid() == true;
        }
    }
}