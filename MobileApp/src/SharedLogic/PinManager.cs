using Core.Analytics;
using Core.Helpers;
using Core.Models;
using Core.Security;
using Core.Interfaces;
using System;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Data.SharePoint.Authentication;

namespace SharedLogic
{
    public class PinManager : IPinManager
    {
        private IDatabaseService _databaseService;
        private IAnalyticsService _analyticsService;
        public PinManager(
            IDatabaseService databaseService, 
            IAnalyticsService analyticsService)
        {
            _databaseService = databaseService;
            _analyticsService = analyticsService;
        }

        private string Key {
            get {
                return AsyncHelper.RunSync(_databaseService.GetKey);
            }
        }
  
        public async Task SavePin(AuthenticatedUser user, SecureString pin)
        {
            if (user == null)
            {
                user = new AuthenticatedUser();
            }
            var encryptor = Encryptor.Get(Key);
            var hashedPin = HashPin(pin);
            var encryptedPin = encryptor.Encrypt(hashedPin);
            user.EncryptedPinHash = encryptedPin;
            await _databaseService.InsertUpdate(user);
            encryptor.Dispose();

        }
        public bool VerifyPin(AuthenticatedUser user, SecureString pin)
        {
            var encryptor = Encryptor.Get(Key);
            var hashedPin = HashPin(pin);
            var decryptedHash = encryptor.Decrypt(user.EncryptedPinHash);
            encryptor.Dispose();
            return hashedPin == decryptedHash;
        }

        internal static string HashPin(SecureString pin)
        {
            return Convert.ToBase64String(GetHash(pin.ToInsecureString()));
        }

        internal static byte[] GetHash(string inputString)
        {
            HashAlgorithm algorithm = SHA256.Create();
            return algorithm.ComputeHash(Encoding.Unicode.GetBytes(inputString));
        }
    }
}
