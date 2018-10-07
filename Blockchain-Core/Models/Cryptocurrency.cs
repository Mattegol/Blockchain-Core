using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RSA;

namespace Blockchain_Core.Models
{
    public class CryptoCurrency
    {
        private List<Transaction> _currentTransactions = new List<Transaction>();
        private List<Block> _chain = new List<Block>();
        private List<Node> _nodes = new List<Node>();

        private Block _lastBlock => _chain.Last();

        public string NodeId { get; set; }

        private static int blockCount;
        private static decimal reward = 50;

        private static string minerPrivateKey = "";
        private static Wallet _minersWallet = RSA.RSA.KeyGenerate();

        public CryptoCurrency()
        {
            minerPrivateKey = "L3aq7WPiSois3N7GxTr6ZSXMNdfbAZWNebiYvKK5hAUBCijk95rL";//_minersWallet.PrivateKey;
            NodeId = "18jp31DcT3n5vsYHGVhhQa2qsvEve4EUoQ";//_minersWallet.PublicKey;

            //string sign = RSA.RSA.Sign(minerPrivateKey, "h");
            //bool b = RSA.RSA.Verify(NodeId, "h", sign);

            //initial transaction
            var transaction = new Transaction { Sender = "0", Recipient = NodeId, Amount = 50, Fees = 0, Signature = "" };
            _currentTransactions.Add(transaction);

            CreateNewBlock(proof: 100, previousHash: "1"); //genesis block
        }

        private void RegisterNode(string address)
        {
            _nodes.Add(new Node { Address = new Uri(address) });
        }

        private Block CreateNewBlock(int proof, string previousHash = null)
        {
            var block = new Block
            {
                Index = _chain.Count,
                Timestamp = DateTime.UtcNow,
                Transactions = _currentTransactions.ToList(),
                Proof = proof,
                PreviousHash = previousHash ?? GetHash(_chain.Last())
            };

            _currentTransactions.Clear();
            _chain.Add(block);
            return block;
        }

        private string GetHash(Block block)
        {
            string blockText = JsonConvert.SerializeObject(block);
            return GetSha256(blockText);
        }

        private string GetSha256(string data)
        {
            var sha256 = new SHA256Managed();
            var hashBuilder = new StringBuilder();

            byte[] bytes = Encoding.Unicode.GetBytes(data);
            byte[] hash = sha256.ComputeHash(bytes);

            foreach (byte x in hash)
                hashBuilder.Append($"{x:x2}");

            return hashBuilder.ToString();
        }

        private int CreateProofOfWork(string previousHash)
        {
            int proof = 0;
            while (!IsValidProof(_currentTransactions, proof, previousHash))
                proof++;

            if (blockCount == 10)
            {
                blockCount = 0;
                reward = reward / 2;
            }

            var transaction = new Transaction { Sender = "0", Recipient = NodeId, Amount = reward, Fees = 0, Signature = "" };
            _currentTransactions.Add(transaction);
            //CreateTransaction(sender: "0", recipient: NodeId, amount: reward, signature: "");//reward
            blockCount++;
            return proof;
        }
        private bool IsValidProof(List<Transaction> transactions, int proof, string previousHash)
        {
            var signatures = transactions.Select(x => x.Signature).ToArray();
            string guess = $"{signatures}{proof}{previousHash}";
            string result = GetSha256(guess);
            return result.StartsWith("00");//difficulty
        }

        public bool Verify_transaction_signature(Transaction transaction, string signedMessage, string publicKey)
        {
            string originalMessage = transaction.ToString();
            bool verified = RSA.RSA.Verify(publicKey, originalMessage, signedMessage);

            return verified;
        }
        private List<Transaction> TransactionByAddress(string ownerAddress)
        {
            List<Transaction> trns = new List<Transaction>();
            foreach (var block in _chain.OrderByDescending(x => x.Index))
            {
                var ownerTransactions = block.Transactions.Where(x => x.Sender == ownerAddress || x.Recipient == ownerAddress).ToList();
                trns.AddRange(ownerTransactions);
            }
            return trns;
        }

        public bool HasBalance(Transaction transaction)
        {
            var transactions = TransactionByAddress(transaction.Sender);
            decimal balance = 0;
            foreach (var item in transactions)
            {
                if (item.Recipient == transaction.Sender)
                {
                    balance = balance + item.Amount;
                }
                else
                {
                    balance = balance - item.Amount;
                }
            }

            return balance >= (transaction.Amount + transaction.Fees);
        }
        private void AddTransaction(Transaction transaction)
        {
            _currentTransactions.Add(transaction);

            if (transaction.Sender != NodeId)
            {
                _currentTransactions.Add(new Transaction
                {
                    Sender = transaction.Sender,
                    Recipient = NodeId,
                    Amount = transaction.Fees,
                    Signature = "",
                    Fees = 0
                });
            }
        }

        private bool ResolveConflicts()
        {
            List<Block> newChain = null;
            int maxLength = _chain.Count;

            foreach (Node node in _nodes)
            {
                var url = new Uri(node.Address, "/chain");
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

                    if (data.chain.Count > _chain.Count && IsValidChain(data.chain))
                    {
                        maxLength = data.chain.Count;
                        newChain = data.chain;
                    }
                }
            }

            if (newChain != null)
            {
                _chain = newChain;
                return true;
            }

            return false;
        }

        private bool IsValidChain(List<Block> chain)
        {
            Block block = null;
            Block lastBlock = chain.First();
            int currentIndex = 1;
            while (currentIndex < chain.Count)
            {
                block = chain.ElementAt(currentIndex);

                //Check that the hash of the block is correct
                if (block.PreviousHash != GetHash(lastBlock))
                    return false;

                //Check that the Proof of Work is correct
                if (!IsValidProof(block.Transactions, block.Proof, lastBlock.PreviousHash))
                    return false;

                lastBlock = block;
                currentIndex++;
            }

            return true;
        }


        //web server calls
        internal Block Mine()
        {
            int proof = CreateProofOfWork(_lastBlock.PreviousHash);

            Block block = CreateNewBlock(proof /*, _lastBlock.PreviousHash*/);

            return block;
        }

        internal string GetFullChain()
        {
            var response = new
            {
                chain = _chain.ToArray(),
                length = _chain.Count
            };

            return JsonConvert.SerializeObject(response);
        }

        internal string RegisterNodes(string[] nodes)
        {
            var builder = new StringBuilder();
            foreach (string node in nodes)
            {
                string url = node;//$"http://{node}";
                RegisterNode(url);
                builder.Append($"{url}, ");
            }

            builder.Insert(0, $"{nodes.Count()} new nodes have been added: ");
            string result = builder.ToString();
            return result.Substring(0, result.Length - 2);
        }

        internal object Consensus()
        {
            bool replaced = ResolveConflicts();
            string message = replaced ? "was replaced" : "is authoritive";

            var response = new
            {
                Message = $"Our chain {message}",
                Chain = _chain
            };

            return response;//JsonConvert.SerializeObject(response);
        }

        internal object CreateTransaction(Transaction transaction)
        {
            var rsp = new object();
            //verify
            var verified = Verify_transaction_signature(transaction, transaction.Signature, transaction.Sender);
            if (verified == false || transaction.Sender == transaction.Recipient)
            {
                rsp = new { message = $"Invalid Transaction!" };
                return rsp;
            }
            if (HasBalance(transaction) == false)
            {
                rsp = new { message = $"InSufficient Balance" };
                return rsp;
            }

            AddTransaction(transaction);

            var blockIndex = _lastBlock != null ? _lastBlock.Index + 1 : 0;

            rsp = new { message = $"Transaction will be added to Block {blockIndex}" };
            return rsp;
        }

        internal List<Transaction> GetTransactions()
        {
            return _currentTransactions;
        }
        internal List<Block> GetBlocks()
        {
            return _chain;
        }
        internal List<Node> GetNodes()
        {
            return _nodes;
        }
        internal Wallet GetMinersWallet()
        {
            return _minersWallet;
        }
    }
}
