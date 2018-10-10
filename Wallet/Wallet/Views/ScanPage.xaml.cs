using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Newtonsoft.Json;
using Wallet.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ZXing.Net.Mobile.Forms;

namespace Wallet.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ScanPage : ContentPage
	{
        public Transaction Transaction { get; set; }
        public ScanPage()
        {
            InitializeComponent();
            Transaction = new Transaction
            {
                Fees = 2,
                Recipient = "",
                Amount = 0

            };

            BindingContext = this;
        }

        private async void ButtonScan_Clicked(object sender, EventArgs e)
        {
            try
            {
                //var options = new ZXing.Mobile.MobileBarcodeScanningOptions
                //{
                //    PossibleFormats = new List<ZXing.BarcodeFormat>()
                //    {
                //        ZXing.BarcodeFormat.QR_CODE
                //    }
                //};

                var scannerPage = new ZXingScannerPage();

                scannerPage.OnScanResult += (result) =>
                {
                    scannerPage.IsScanning = false;

                    Device.BeginInvokeOnMainThread(() =>
                    {
                        Navigation.PopAsync();
                        TxtBarcode.Text = result.Text;
                        Transaction.Signature = result.Text;

                        var str = result.Text.Split(new[] { "amount=" }, StringSplitOptions.None);

                        //Transaction.Recipient = str[0];
                        if (str.Length > 1)
                            TxtAmount.Text = str[1];
                        //Transaction.Amount = Convert.ToDecimal(str[1]);
                        //DisplayAlert("Código escaneado", result.Text, "OK");
                    });
                };

                await Navigation.PushAsync(scannerPage);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.InnerException);
                Console.WriteLine(exception.Message);
                throw;
            }
            
        }

        private async void ButtonPay_Clicked(object sender, EventArgs e)
        {
            var dictionary = ParseQueryString(new Uri(Transaction.Signature));

            Transaction.Sender = Credential.PublicKey;
            Transaction.PrivateKey = Credential.PrivateKey;
            Transaction.Recipient = dictionary.GetValueOrDefault("recipient");
            Transaction.Amount = Convert.ToDecimal(dictionary.GetValueOrDefault("amount"));

            var url = Transaction.Signature.Split('?');
            var signature = RSA.Sign(Transaction.PrivateKey, Transaction.ToString());

            dictionary.Add("signature", signature);
            dictionary.Add("sender", Transaction.Sender);
            var json = JsonConvert.SerializeObject(dictionary);
            var stringContent = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            try
            {
                string urll = url[0] + "?ip=" + dictionary.GetValueOrDefault("ip") + "&pid=" + dictionary.GetValueOrDefault("pid");
                var response = await client.PostAsync(urll, stringContent);

                var content = await response.Content.ReadAsStringAsync();
                var rsp = new { message = "" };
                var data = JsonConvert.DeserializeAnonymousType(content, rsp);

                if (data.message.Contains("Transaction will be added to Block"))
                {
                    //successfull
                    await DisplayAlert("Payment Done", "Transaction Completed", "OK");
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public static Dictionary<string, string> ParseQueryString(Uri uri)  //url?ab=1&new=2
        {
            var query = uri.Query.Substring(uri.Query.IndexOf('?') + 1); // +1 for skipping '?'
            var pairs = query.Split('&');
            return pairs
                .Select(o => o.Split('='))
                .Where(items => items.Count() == 2)
                .ToDictionary(pair => Uri.UnescapeDataString(pair[0]),
                    pair => Uri.UnescapeDataString(pair[1]));
        }
    }
}