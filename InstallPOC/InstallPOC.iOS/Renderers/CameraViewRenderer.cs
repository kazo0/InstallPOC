using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AVFoundation;
using Foundation;
using InstallPOC.Controls;
using InstallPOC.iOS.Renderers;
using UIKit;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(CameraView), typeof(CameraViewRenderer))]
namespace InstallPOC.iOS.Renderers
{
	
	public class CameraViewRenderer : ViewRenderer<CameraView, UICameraView>
	{
		UICameraView _uiCameraView;


		protected override void OnElementChanged(ElementChangedEventArgs<CameraView> e)
		{
			base.OnElementChanged(e);

			if (e.NewElement != null)
			{
				if (Control == null)
				{
					_uiCameraView = new UICameraView();
					SetNativeControl(_uiCameraView);
				}
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				Control.CaptureSession?.Dispose();
				Control.CaptureDevice?.Dispose();
				Control.Dispose();
			}
			base.Dispose(disposing);
		}
	}
}