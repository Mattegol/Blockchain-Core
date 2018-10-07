using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Blockchain_Core.Controllers.Api;
using Microsoft.AspNetCore.Mvc;
using Blockchain_Core.Models;

namespace Blockchain_Core.Controllers
{
    public class HomeController : Controller
    {
        private static readonly CryptoCurrency _blockchain = BlockchainController.Blockchain;

        public IActionResult Index()
        {
            List<Transaction> transactions = _blockchain.GetTransactions();
            ViewBag.Transactions = transactions;

            List<Block> blocks = _blockchain.GetBlocks();
            ViewBag.Blocks = blocks;

            return View();
        }

        public IActionResult Mine()
        {
            _blockchain.Mine();
            return RedirectToAction("Index");
        }

        public IActionResult Configure()
        {
            return View(_blockchain.GetNodes());
        }

        [HttpPost]
        public IActionResult RegisterNodes(string nodes)
        {
            string[] node = nodes.Split(',');
            _blockchain.RegisterNodes(node);

            return RedirectToAction("Configure");
        }

        public IActionResult CoinBase()
        {
            List<Block> blocks = _blockchain.GetBlocks();
            ViewBag.Blocks = blocks;

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
