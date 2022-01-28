using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Kemorave_LC_Generator
{
	public partial class MainPage : ContentPage
	{
		public MainPage()
		{
			InitializeComponent();
			Kemorave.LC.Manager.SetKey("0116720907", "Hema");
		}
		public class SecureCode : Kemorave.LC.Manager.IVal
		{
			public SecureString Secure { get; set; }
		}
		private void Button_Clicked(object sender, EventArgs e)
		{
			var code = new SecureCode();
			Kemorave.LC.Manager.ValidateCode(code, Host.Text);
			string password = new System.Net.NetworkCredential(string.Empty, code.Secure).Password;
			Navigation.PushAsync(new ActivationCodePage(password));
		}
	}
}
