// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;

namespace JBCroppableViewAppMono
{
	[Register ("JBCroppableViewApp_MonoViewController")]
	partial class JBCroppableViewApp_MonoViewController
	{
		[Outlet]
		MonoTouch.UIKit.UIButton undoButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton cropButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIImageView image { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (undoButton != null) {
				undoButton.Dispose ();
				undoButton = null;
			}

			if (cropButton != null) {
				cropButton.Dispose ();
				cropButton = null;
			}

			if (image != null) {
				image.Dispose ();
				image = null;
			}
		}
	}
}
