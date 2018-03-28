using System;
using System.Diagnostics.CodeAnalysis;

namespace Backend.Users.Domain.Aggregates.UserAggregate
{
    [SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Local")]
    public class User
    {
        public User(string userName, string passwordHash) : this()
        {
            SetUserName(userName);
            SetPasswordHash(passwordHash);
        }

        private User()
        {
            CreationDate = DateTimeOffset.UtcNow;
        }

        public long Id { get; private set; }

        public string UserName { get; private set; }

        public string PasswordHash { get; private set; }

        public DateTimeOffset CreationDate { get; private set; }

        public DateTimeOffset? LastUpdateDate { get; private set; }


        private void SetUserName(string userName)
        {
            if (string.IsNullOrEmpty(userName))
                throw new Exception("Требуется указать имя пользователя");
            UserName = userName;
        }

        private void SetPasswordHash(string passwordHash)
        {
            if (string.IsNullOrEmpty(passwordHash))
                throw new Exception("Требуется указать пароль пользователя");
            PasswordHash = passwordHash;
        }

        public void ChangeUserName(string userName)
        {
            SetUserName(userName);
            LastUpdateDate = DateTimeOffset.UtcNow;
        }

        public void ChangePasswordHash(string passwordHash)
        {
            SetPasswordHash(passwordHash);
            LastUpdateDate = DateTimeOffset.UtcNow;
        }
    }
}