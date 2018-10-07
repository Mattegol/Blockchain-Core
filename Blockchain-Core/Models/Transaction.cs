using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blockchain_Core.Models
{
    public class Transaction
    {
        public decimal Amount { get; set; }

        public string Recipient { get; set; }

        public string Sender { get; set; }

        public string Signature { get; set; }

        public decimal Fees { get; set; }

        public override string ToString()
        {
            return Amount.ToString("0.00000000") + Recipient + Sender;
        }
    }
}
