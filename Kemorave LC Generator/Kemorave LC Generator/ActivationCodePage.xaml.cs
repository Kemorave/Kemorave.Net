using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kemorave_LC_Generator
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ActivationCodePage : ContentPage
	{
		public ActivationCodePage(string code)
		{
			Code = code;
			InitializeComponent();
			BindingContext = this;
		}

		public string Code { get; }
	}
}