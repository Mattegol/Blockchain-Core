using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Wallet.Models;

namespace Wallet
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ItemsPage : ContentPage
	{
        public Transaction Transaction { get; set; }

        public ItemsPage()
        {
            InitializeComponent();
            Transaction = new Transaction
            {
                PrivateKey = "L3aq7WPiSois3N7GxTr6ZSXMNdfbAZWNebiYvKK5hAUBCijk95rL",
                Sender = "18jp31DcT3n5vsYHGVhhQa2qsvEve4EUoQ",
            };

            BindingContext = this;
        }

	    private async Task ButtonSave_Clicked(object sender, EventArgs e)
        {
            Credential.PublicKey = Transaction.Sender;
            Credential.PrivateKey = Transaction.PrivateKey;

            await DisplayAlert("Credential", "Keys Updated", "OK");
        }

	    private async void CreateSign_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new NavigationPage(new ViewTransaction()));
        }
    }
}