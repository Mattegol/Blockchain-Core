using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Blockchain_Core_Client.Models;
using Newtonsoft.Json;

namespace Blockchain_Core_Client.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult MakeTransaction()
        {
            return View();
        }

        public IActionResult ViewTransaction()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ViewTransaction(string nodeUrl)
        {
            var url = new Uri(nodeUrl + "/chain");
            ViewBag.Blocks = GetChain(url);

            return View();
        }

        public IActionResult WalletTransaction()
        {
            return View(new List<Transaction>());
        }

        [HttpPost]
        public IActionResult WalletTransaction(string publicKey)
        {
            var url = new Uri("http://localhost:61820" + "/chain");
            var blocks = GetChain(url);
            ViewBag.publickey = publicKey;

            return View(TransactionByAddress(publicKey, blocks));
        }

        private List<Transaction> TransactionByAddress(string ownerAddress, List<Block> chain)
        {
            List<Transaction> transactions = new List<Transaction>();
            foreach (var block in chain.OrderByDescending(x => x.Index))
            {
                var ownerTransactions = block.Transactions.Where(x => x.Sender == ownerAddress || x.Recipient == ownerAddress).ToList();
                transactions.AddRange(ownerTransactions);
            }
            return transactions;
        }

        private List<Block> GetChain(Uri url)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            var response = (HttpWebResponse)request.GetResponse();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var model = new
                {
                    chain = new List<Block>(),
                    length = 0
                };
                string json = new StreamReader(response.GetResponseStream()).ReadToEnd();
                var data = JsonConvert.DeserializeAnonymousType(json, model);

                return data.chain;
            }
            return null;
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
