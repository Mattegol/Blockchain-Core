using System;
using System.Collections.Generic;
using System.Text;

namespace Wallet.Models
{
    public class Transaction
    {
        public decimal Amount { get; set; }

        public string Recipient { get; set; }

        public string Sender { get; set; }//publicKey

        public string Signature { get; set; }

        public decimal Fees { get; set; }

        public string PrivateKey { get; set; }

        public override string ToString()
        {
            return Amount.ToString("0.00000000") + Recipient + Sender;
        }
    }
}
