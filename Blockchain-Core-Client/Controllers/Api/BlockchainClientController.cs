using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blockchain_Core_Client.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Blockchain_Core_Client.Controllers.Api
{
    [Produces("application/json")]
    [Route("")]
    public class BlockchainClientController : Controller
    {
        [HttpGet("wallet/new")]
        public IActionResult NewWallet()
        {
            var wallet = RSA.RSA.KeyGenerate();
            var response = new
            {
                private_key = wallet.PrivateKey,
                public_key = wallet.PublicKey
            };

            return Ok(response);
        }

        [HttpPost("generate/transaction")]
        public IActionResult NewTransaction(TransactionClient transaction)
        {
            var sign = RSA.RSA.Sign(transaction.SenderPrivateKey, transaction.ToString());
            var response = new { transaction, signature = sign };

            return Ok(response);
        }

    }
}