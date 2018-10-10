using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using Newtonsoft.Json;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Wallet.Models;

namespace Wallet
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ViewTransaction : ContentPage
    {

        public ViewTransaction()
        {
            InitializeComponent();

            var blocks = GetChain();
            var transactions = TransactionByAddress(Credential.PublicKey, blocks);
            decimal balance = 0;
            decimal receives = 0;
            decimal deduct = 0;
            var listStrings = new List<string>();

            foreach (var item in transactions)
            {
                if (item.Recipient == Credential.PublicKey)
                {
                    balance = balance + item.Amount;
                    receives = receives + item.Amount;
                }
                else
                {
                    balance = balance - item.Amount;
                    deduct = deduct + item.Amount;
                }
                listStrings.Add(item.Sender + " sent " + item.Amount + " to " + item.Recipient);
            }

            TxtReceives.Text = receives.ToString(CultureInfo.InvariantCulture);
            TxtDeduct.Text = deduct.ToString(CultureInfo.InvariantCulture);
            TxtBalance.Text = balance.ToString(CultureInfo.InvariantCulture);

            List.ItemsSource = listStrings;
        }

        private List<Block> GetChain()
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            try
            {
                var url = new Uri("http://localhost:61821/" + "/api/chain");
                var response = client.GetAsync(url).Result;

                var content = response.Content.ReadAsStringAsync().Result;
                var model = new
                {
                    chain = new List<Block>(),
                    length = 0
                };
                var data = JsonConvert.DeserializeAnonymousType(content, model);

                return data.chain;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException);
                throw ex;
            }
        }

        private List<Transaction> TransactionByAddress(string ownerAddress, List<Block> chain)
        {
            var transactions = new List<Transaction>();
            foreach (var block in chain.OrderByDescending(x => x.Index))
            {
                var ownerTransactions = block.Transactions
                                            .Where(x => 
                                            x.Sender == ownerAddress || 
                                            x.Recipient == ownerAddress)
                                            .ToList();

                transactions.AddRange(ownerTransactions);
            }

            return transactions;
        }

    }
}