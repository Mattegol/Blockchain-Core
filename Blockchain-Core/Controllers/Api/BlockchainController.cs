using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blockchain_Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Blockchain_Core.Controllers.Api
{
    [Produces("application/json")]
    [Route("")]
    public class BlockchainController : Controller
    {
        public static CryptoCurrency Blockchain = new CryptoCurrency();

        [HttpPost("transactions/new")]
        public IActionResult NewTransaction([FromBody]Transaction transaction)
        {
            var response = Blockchain.CreateTransaction(transaction);

            return Ok(response);
        }

        [HttpGet("transactions/get")]
        public IActionResult GetTransactions()
        {
            var response = new { transactions = Blockchain.GetTransactions() };

            return Ok(response);
        }

        [HttpGet("chain")]
        public IActionResult FullChain()
        {
            var blocks = Blockchain.GetBlocks();
            var response = new { chain = blocks, length = blocks.Count };

            return Ok(response);
        }

        [HttpGet("mine")]
        public IActionResult Mine()
        {
            var block = Blockchain.Mine();
            var response = new
            {
                message = "New Block Forged",
                block_number = block.Index,
                transactions = block.Transactions.ToArray(),
                nonce = block.Proof,
                previous_hash = block.PreviousHash
            };

            return Ok(response);
        }

        [HttpPost("nodes/register")]
        public IActionResult RegisterNodes(string[] nodes)//{ "Urls": ["localhost:54321", "localhost:54345", "localhost:12321"] }
        {
            Blockchain.RegisterNodes(nodes);
            var response = new
            {
                message = "New nodes have been added",
                total_nodes = nodes.Count()//'total_nodes': [node for node in blockchain.nodes],
            };

            return Created("", response);
        }

        [HttpGet("nodes/resolve")]
        public IActionResult Consensus()
        {
            return Ok(Blockchain.Consensus());
        }

        [HttpGet("nodes/get")]
        public IActionResult GetNodes()
        {
            return Ok(new { nodes = Blockchain.GetNodes() });
        }

        //////miner/////
        [HttpGet("wallet/miner")]
        public IActionResult GetMinersWallet()
        {
            return Ok(Blockchain.GetMinersWallet());
        }
    }
}