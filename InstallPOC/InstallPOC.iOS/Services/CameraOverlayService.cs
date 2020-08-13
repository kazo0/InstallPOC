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
				Text = "This is a test of the camera overlay feature for iOS only. This text should show at the top of the camera frame regardless of orientation",
				TextColor = UIColor.Red,
				TextAlignment = UITextAlignment.Center,
				Lines = 0,
				LineBreakMode = UILineBreakMode.WordWrap,
				AutoresizingMask = UIViewAutoresizing.FlexibleLeftMargin | UIViewAutoresizing.FlexibleRightMargin,
				Frame = new CGRect(0, 0, UIScreen.MainScreen.Bounds.Width, 0),
			};
			label.SizeToFit();
			
			var labelFrame = label.Frame;

			label.Frame = new CGRect(labelFrame.Location, new CGSize(UIScreen.MainScreen.Bounds.Width, labelFrame.Height));

			return label;
		}
	}
}