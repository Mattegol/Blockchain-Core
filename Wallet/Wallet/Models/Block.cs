using System;
using System.Collections.Generic;
using System.Text;

namespace Wallet.Models
{
    public class Block
    {
        public int Index { get; set; }

        public DateTime Timestamp { get; set; }

        public List<Transaction> Transactions { get; set; }

        public int Proof { get; set; }

        public string PreviousHash { get; set; }

        public override string ToString()
        {
            return $"{Index} [{Timestamp:yyyy-MM-dd HH:mm:ss}] Proof: {Proof} | PrevHash: {PreviousHash} | Trx: {Transactions.Count}";
        }
    }
}
