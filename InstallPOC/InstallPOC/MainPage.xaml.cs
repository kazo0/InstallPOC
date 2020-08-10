using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstallPOC.Models;
using InstallPOC.ViewModels;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace InstallPOC
{
	public partial class MainPage : ContentPage
	{
		public MainPage()
		{
			InitializeComponent();
			BindingContext = new MainViewModel();
		}


		private void TapGestureRecognizer_OnTapped(object sender, EventArgs e)
		{
			MainThread.BeginInvokeOnMainThread(async () => await Navigation.PushAsync(new PhotosPage()));
		}
	}
}
