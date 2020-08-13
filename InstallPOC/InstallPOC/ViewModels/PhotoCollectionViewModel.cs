using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using InstallPOC.Models;
using InstallPOC.Services;
using MvvmHelpers;
using MvvmHelpers.Commands;
using Plugin.Media;
using Xamarin.Forms;

namespace InstallPOC.ViewModels
{
	public class PhotoCollectionViewModel : ViewModelBase
	{
		public string Title { get; set; }

		public ObservableRangeCollection<Photo> Photos { get; set; }

		public ICommand TakePhotoCommand => new AsyncCommand(TakePhoto);
		public ICommand DeletePhotoCommand => new AsyncCommand<Photo>(DeletePhoto);

		public PhotoCollectionViewModel()
		{
			Photos = new ObservableRangeCollection<Photo>();
		}

		private async Task TakePhoto()
		{
			await CrossMedia.Current.Initialize();

			var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
			{
				Directory = "Sample",
				Name = "test.jpg",
				OverlayViewProvider = () => DependencyService.Get<ICameraOverlayService>().GetOverlayView(),
			});

			if (file == null)
				return;

			Photos.Add(new Photo() { PhotoPath = file.Path });
        }

		private async Task DeletePhoto(Photo photo)
		{
			Photos.Remove(photo);
		}
	}
}
