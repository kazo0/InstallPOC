using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstallPOC.Models;
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

		private void ImageButton_OnClicked(object sender, EventArgs e)
		{
			MainThread.BeginInvokeOnMainThread(async () =>
				{
					var button = (ImageButton) sender;
					var b = button.BindingContext;
					var answer = await DisplayAlert("Are you sure?", "Delete this photo?", "Yes", "No");
					if (answer)
					{
						((PhotoCollectionViewModel)BindingContext).DeletePhotoCommand.Execute((Photo)b);
					}
				});
		}
	}
}