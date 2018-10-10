using NBitcoin;

namespace Wallet.Models
{
    public static class RSA
    {
        public static string Sign(string privateKey, string messageToSign)
        {
            var secret = Network.Main.CreateBitcoinSecret(privateKey);
            var signature = secret.PrivateKey.SignMessage(messageToSign);

            var v = secret.PubKey.VerifyMessage(messageToSign, signature);

            return signature;
        }
    }
}
