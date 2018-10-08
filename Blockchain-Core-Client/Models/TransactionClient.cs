using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blockchain_Core_Client.Models
{
    public class TransactionClient
    {
        public decimal Amount { get; set; }

        public string RecipientAddress { get; set; }

        public string SenderAddress { get; set; }

        public string SenderPrivateKey { get; set; }

        public decimal Fees { get; set; }

        public override string ToString()
        {
            return Amount.ToString("0.00000000") + RecipientAddress + SenderAddress;
        }
    }
}
