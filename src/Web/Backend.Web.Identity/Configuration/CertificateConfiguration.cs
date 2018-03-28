using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Backend.Web.Identity.Configuration
{
    public class CertificateConfiguration
    {
        /// <summary>
        ///     Расположение сертификата.
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        ///     Пароль сертификата.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        ///     Конвертирует данный экземпляр <see cref="CertificateConfiguration" /> в <see cref="X509Certificate2" />.
        /// </summary>
        /// <returns></returns>
        public X509Certificate2 ToCertificate()
        {
            var certFileInfo = new FileInfo(Location);
            var cert = new X509Certificate2(
                certFileInfo.FullName,
                Password,
                X509KeyStorageFlags.Exportable | X509KeyStorageFlags.MachineKeySet);
            return cert;
        }

        /// <summary>
        ///     Проверяет корректность параметров сертификата.
        /// </summary>
        /// <returns></returns>
        public bool IsValid()
        {
            try
            {
                var cert = ToCertificate();
                return cert != null;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}