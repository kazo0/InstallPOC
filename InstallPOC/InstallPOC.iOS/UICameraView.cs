using System;
using System.Collections.Generic;
using System.Linq;
using AVFoundation;
using CoreAnimation;
using CoreGraphics;
using Foundation;
using UIKit;

namespace InstallPOC.iOS
{
	public class UICameraView : UIView
	{
		AVCaptureVideoPreviewLayer previewLayer;

		public AVCaptureSession CaptureSession { get; private set; }

		public AVCaptureDevice CaptureDevice { get; private set; }

		private UIButton _toggleFlashButton;
		private UIButton _takePhotoButton;
		AVCaptureStillImageOutput stillImageOutput;
		private CALayer _controlsLayer;
		private UIView _controlsContainer;

		private List<NSLayoutConstraint> _sharedConstraints;
		private List<NSLayoutConstraint> _landscapeRightConstraints;
		private List<NSLayoutConstraint> _landscapeLeftConstraints;
		private List<NSLayoutConstraint> _portraitConstraints;
		private List<NSLayoutConstraint> _portraitUpsideDownConstraints;

		public UICameraView()
		{
			Initialize();
		}


		public override void LayoutSubviews()
		{
			base.LayoutSubviews();
			if (previewLayer != null)
			{
				previewLayer.Frame = Bounds;
			}
		}

		public override void TouchesEnded(NSSet touches, UIEvent evt)
		{
			base.TouchesBegan(touches, evt);

			if (touches.FirstOrDefault() is UITouch touch)
			{
				var location = touch.LocationInView(this);
				if (CaptureDevice != null)
				{
					if (!CaptureDevice.FocusPointOfInterestSupported)
					{
						return;
					}

					var point = previewLayer.CaptureDevicePointOfInterestForPoint(location);
					if (CaptureDevice.LockForConfiguration(out var error))
					{
						
							CaptureDevice.FocusPointOfInterest = point;
							CaptureDevice.FocusMode = AVCaptureFocusMode.ContinuousAutoFocus;
						
					}

					CaptureDevice.UnlockForConfiguration();
				}
			}
		}

		public void ToggleFlash()
		{
			if (CaptureDevice.HasFlash)
			{
				if (CaptureDevice.FlashMode == AVCaptureFlashMode.On)
				{
					CaptureDevice.LockForConfiguration(out var error);
					CaptureDevice.FlashMode = AVCaptureFlashMode.Off;
					CaptureDevice.UnlockForConfiguration();

					_toggleFlashButton.SetBackgroundImage(UIImage.FromFile("NoFlashButton.png"), UIControlState.Normal);
				}
				else
				{
					CaptureDevice.LockForConfiguration(out var error);
					CaptureDevice.FlashMode = AVCaptureFlashMode.On;
					CaptureDevice.UnlockForConfiguration();

					_toggleFlashButton.SetBackgroundImage(UIImage.FromFile("FlashButton.png"), UIControlState.Normal);
				}
			}
		}

		public async void CapturePhoto()
		{
			var videoConnection = stillImageOutput.ConnectionFromMediaType(AVMediaType.Video);
			var sampleBuffer = await stillImageOutput.CaptureStillImageTaskAsync(videoConnection);

			// var jpegImageAsBytes = AVCaptureStillImageOutput.JpegStillToNSData (sampleBuffer).ToArray ();
			var jpegImageAsNsData = AVCaptureStillImageOutput.JpegStillToNSData(sampleBuffer);
			// var image = new UIImage (jpegImageAsNsData);
			// var image2 = new UIImage (image.CGImage, image.CurrentScale, UIImageOrientation.UpMirrored);
			// var data = image2.AsJPEG ().ToArray ();

			// SendPhoto (data);
			//SendPhoto(jpegImageAsNsData.ToArray());
		}

		public void ConfigureCameraForDevice()
		{
			var error = new NSError();
			if (CaptureDevice.IsFocusModeSupported(AVCaptureFocusMode.ContinuousAutoFocus))
			{
				CaptureDevice.LockForConfiguration(out error);
				CaptureDevice.FocusMode = AVCaptureFocusMode.ContinuousAutoFocus;
				CaptureDevice.UnlockForConfiguration();
			}
			else if (CaptureDevice.IsExposureModeSupported(AVCaptureExposureMode.ContinuousAutoExposure))
			{
				CaptureDevice.LockForConfiguration(out error);
				CaptureDevice.ExposureMode = AVCaptureExposureMode.ContinuousAutoExposure;
				CaptureDevice.UnlockForConfiguration();
			}
			else if (CaptureDevice.IsWhiteBalanceModeSupported(AVCaptureWhiteBalanceMode.ContinuousAutoWhiteBalance))
			{
				CaptureDevice.LockForConfiguration(out error);
				CaptureDevice.WhiteBalanceMode = AVCaptureWhiteBalanceMode.ContinuousAutoWhiteBalance;
				CaptureDevice.UnlockForConfiguration();
			}
		}

		void Initialize()
		{
			CaptureSession = new AVCaptureSession();

			SetupUserInterface();
			SetupConstraints();
			AuthorizeCameraUse();
			SetupCameraStream();

			NSLayoutConstraint.ActivateConstraints(_sharedConstraints.ToArray());

			OrientationChanged(null);
		}

		private void SetupCameraStream()
		{
			var videoDevices = AVCaptureDevice.DevicesWithMediaType(AVMediaType.Video);
			var cameraPosition = AVCaptureDevicePosition.Back;

			CaptureDevice = videoDevices.FirstOrDefault(d => d.Position == cameraPosition);
			if (CaptureDevice == null)
			{
				return;
			}

			ConfigureCameraForDevice();

			var input = new AVCaptureDeviceInput(CaptureDevice, out var error);
			var dictionary = new NSMutableDictionary();
			dictionary[AVVideo.CodecKey] = new NSNumber((int) AVVideoCodec.JPEG);
			stillImageOutput = new AVCaptureStillImageOutput
			{
				OutputSettings = new NSDictionary()
			};

			CaptureSession.AddInput(input);
			CaptureSession.AddOutput(stillImageOutput);


			CaptureSession.StartRunning();
		}

		public async void AuthorizeCameraUse()
		{
			var authorizationStatus = AVCaptureDevice.GetAuthorizationStatus(AVMediaType.Video);

			if (authorizationStatus != AVAuthorizationStatus.Authorized)
			{
				await AVCaptureDevice.RequestAccessForMediaTypeAsync(AVMediaType.Video);
			}
		}

		private void SetupUserInterface()
		{
			previewLayer = new AVCaptureVideoPreviewLayer(CaptureSession)
			{
				VideoGravity = AVLayerVideoGravity.ResizeAspectFill
			};
			_controlsContainer = new UIView()
			{
				TranslatesAutoresizingMaskIntoConstraints = false,
			};
			_toggleFlashButton = new UIButton()
			{
				TranslatesAutoresizingMaskIntoConstraints = false,
			};
			_toggleFlashButton.SetBackgroundImage(UIImage.FromFile("NoFlashButton.png"), UIControlState.Normal);
			_toggleFlashButton.TouchUpInside += (sender, args) => { ToggleFlash(); };

			_takePhotoButton = new UIButton()
			{
				TranslatesAutoresizingMaskIntoConstraints = false,
			};
			_takePhotoButton.SetBackgroundImage(UIImage.FromFile("TakePhotoButton.png"), UIControlState.Normal);

			_takePhotoButton.TouchUpInside += (sender, e) => {
				CapturePhoto();
			};


			Layer.AddSublayer(previewLayer);
			AddSubview(_controlsContainer);
			_controlsContainer.AddSubview(_takePhotoButton);

			NSNotificationCenter.DefaultCenter.AddObserver(
				UIDevice.OrientationDidChangeNotification,
				OrientationChanged);
		}

		private void SetupConstraints()
		{
			_sharedConstraints = new List<NSLayoutConstraint>(new []
			{
				_controlsContainer.LeadingAnchor.ConstraintEqualTo(this.LeadingAnchor),
				_controlsContainer.TrailingAnchor.ConstraintEqualTo(this.TrailingAnchor),
				_controlsContainer.TopAnchor.ConstraintEqualTo(this.TopAnchor),
				_controlsContainer.BottomAnchor.ConstraintEqualTo(this.BottomAnchor),

				NSLayoutConstraint.Create(_takePhotoButton, NSLayoutAttribute.Height, NSLayoutRelation.Equal, 1, 70), 
				NSLayoutConstraint.Create(_takePhotoButton, NSLayoutAttribute.Width, NSLayoutRelation.Equal, 1, 70), 
			});

			_portraitConstraints = new List<NSLayoutConstraint>(new []
			{
				_takePhotoButton.TrailingAnchor.ConstraintEqualTo(_controlsContainer.TrailingAnchor, -25f),
				_takePhotoButton.CenterYAnchor.ConstraintEqualTo(_controlsContainer.CenterYAnchor)
			});

			_portraitUpsideDownConstraints= new List<NSLayoutConstraint>(new[]
			{
				_takePhotoButton.LeadingAnchor.ConstraintEqualTo(_controlsContainer.LeadingAnchor, 25f),
				_takePhotoButton.CenterYAnchor.ConstraintEqualTo(_controlsContainer.CenterYAnchor)
			});

			_landscapeRightConstraints = new List<NSLayoutConstraint>(new[]
			{
				_takePhotoButton.TopAnchor.ConstraintEqualTo(_controlsContainer.TopAnchor, 25f),
				_takePhotoButton.CenterXAnchor.ConstraintEqualTo(_controlsContainer.CenterXAnchor)
			});

			_landscapeLeftConstraints = new List<NSLayoutConstraint>(new[]
			{
				_takePhotoButton.BottomAnchor.ConstraintEqualTo(_controlsContainer.BottomAnchor, -25f),
				_takePhotoButton.CenterXAnchor.ConstraintEqualTo(_controlsContainer.CenterXAnchor)
			});
		}

		private void OnLayout()
		{
			if (_controlsContainer != null)
			{
				_controlsContainer.Frame = Bounds;
			}

			var centerButtonX = Bounds.GetMidX() - 35f;
			var topLeftX = Bounds.X + 25;
			var topRightX = Bounds.Right - 65;
			var bottomButtonY = Bounds.Bottom - 85;
			var topButtonY = Bounds.Top + 15;
			var buttonWidth = 70;
			var buttonHeight = 70;

			if (previewLayer != null)
			{
				previewLayer.Frame = Bounds;
			}

			if (_toggleFlashButton != null)
			{
				_toggleFlashButton.Frame = new CGRect(25, 25, 40, 40);
			}

			if (_takePhotoButton != null)
			{
				_takePhotoButton.Frame = new CGRect(centerButtonX, bottomButtonY, buttonWidth, buttonHeight);
			}

			
		}

		private void OrientationChanged(NSNotification notification)
		{
			UIView.Animate(0.3d, () =>
			{
				if (!_sharedConstraints.Any(x => x.Active))
				{
					NSLayoutConstraint.ActivateConstraints(_sharedConstraints.ToArray());
				}

				switch (UIDevice.CurrentDevice.Orientation)
				{
					case UIDeviceOrientation.LandscapeRight:
						NSLayoutConstraint.DeactivateConstraints(_portraitConstraints.ToArray());
						NSLayoutConstraint.DeactivateConstraints(_portraitUpsideDownConstraints.ToArray());
						NSLayoutConstraint.DeactivateConstraints(_landscapeLeftConstraints.ToArray());

						NSLayoutConstraint.ActivateConstraints(_landscapeRightConstraints.ToArray());
						break;
					case UIDeviceOrientation.LandscapeLeft:
						NSLayoutConstraint.DeactivateConstraints(_portraitConstraints.ToArray());
						NSLayoutConstraint.DeactivateConstraints(_portraitUpsideDownConstraints.ToArray());
						NSLayoutConstraint.DeactivateConstraints(_landscapeRightConstraints.ToArray());

						NSLayoutConstraint.ActivateConstraints(_landscapeLeftConstraints.ToArray());
						break;

					case UIDeviceOrientation.Portrait:
						NSLayoutConstraint.DeactivateConstraints(_landscapeRightConstraints.ToArray());
						NSLayoutConstraint.DeactivateConstraints(_portraitUpsideDownConstraints.ToArray());
						NSLayoutConstraint.DeactivateConstraints(_landscapeLeftConstraints.ToArray());

						NSLayoutConstraint.ActivateConstraints(_portraitConstraints.ToArray());
						break;
					case UIDeviceOrientation.PortraitUpsideDown:
						NSLayoutConstraint.DeactivateConstraints(_portraitConstraints.ToArray());
						NSLayoutConstraint.DeactivateConstraints(_landscapeRightConstraints.ToArray());
						NSLayoutConstraint.DeactivateConstraints(_landscapeLeftConstraints.ToArray());

						NSLayoutConstraint.ActivateConstraints(_portraitUpsideDownConstraints.ToArray());
						break;
					default:
						NSLayoutConstraint.DeactivateConstraints(_landscapeRightConstraints.ToArray());
						NSLayoutConstraint.DeactivateConstraints(_portraitUpsideDownConstraints.ToArray());
						NSLayoutConstraint.DeactivateConstraints(_landscapeLeftConstraints.ToArray());

						NSLayoutConstraint.ActivateConstraints(_portraitConstraints.ToArray());
						break;
				}

				LayoutIfNeeded();
			});
		}
	}
}