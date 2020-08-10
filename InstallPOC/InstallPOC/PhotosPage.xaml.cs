using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstallPOC.ViewModels;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace InstallPOC
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PhotosPage : ContentPage
	{
		public PhotosPage()
		{
			InitializeComponent();
			BindingContext = new PhotoCollectionViewModel();
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();
		}
	}
}