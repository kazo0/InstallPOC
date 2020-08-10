using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CoreGraphics;
using Foundation;
using InstallPOC.iOS.Services;
using InstallPOC.Services;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(CameraOverlayService))]
namespace InstallPOC.iOS.Services
{
	public class CameraOverlayService : ICameraOverlayService
	{
		public object GetOverlayView()
		{
			var label = new UILabel()
			{
				Text = "TESTING",
				TextColor = UIColor.Red,
				TextAlignment = UITextAlignment.Center,
			};
			label.Lines = 0;
			label.SizeToFit();
			var labelFrame = label.Frame;
			label.AutoresizingMask = UIViewAutoresizing.FlexibleLeftMargin | UIViewAutoresizing.FlexibleRightMargin;

			label.Frame = new CGRect(labelFrame.Location, new CGSize(UIScreen.MainScreen.Bounds.Width, labelFrame.Height));

			return label;
		}
	}
}