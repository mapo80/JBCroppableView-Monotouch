using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using JBCroppableViewLibrary;

namespace JBCroppableViewAppMono
{
	public partial class JBCroppableViewApp_MonoViewController : UIViewController
	{
		JBCroppableView pointsView;

		static bool UserInterfaceIdiomIsPhone {
			get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone; }
		}

		public JBCroppableViewApp_MonoViewController ()
			: base (UserInterfaceIdiomIsPhone ? "JBCroppableViewApp_MonoViewController_iPhone" : "JBCroppableViewApp_MonoViewController_iPad", null)
		{


		}
		
		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			// Perform any additional setup after loading the view, typically from a nib.

			this.image.Frame = JBCroppableView.scaleRespectAspectFromRect1(new RectangleF(0, 0, this.image.Image.Size.Width, this.image.Image.Size.Height) ,this.image.Frame);
			this.pointsView = new JBCroppableView(this.image);
			
			//    [self.pointsView addPointsAt:@[[NSValue valueWithCGPoint:CGPointMake(10, 10)],
			//                                    [NSValue valueWithCGPoint:CGPointMake(50, 10)],
			//                                    [NSValue valueWithCGPoint:CGPointMake(50, 50)],
			//                                    [NSValue valueWithCGPoint:CGPointMake(10, 50)]]];
			
			
			this.pointsView.addPoints(4);
			
			this.View.AddSubview(this.pointsView);
			this.View.BringSubviewToFront(this.cropButton);
			this.View.BringSubviewToFront(this.undoButton);

			cropButton.TouchUpInside += HandleTouchUpInside;
			undoButton.TouchUpInside += HandleTouchUpInside1;
		}

		void HandleTouchUpInside1 (object sender, EventArgs e)
		{
			this.image.Image = UIImage.FromFile("IMG_0152.JPG");
		}

		void HandleTouchUpInside (object sender, EventArgs e)
		{
			this.image.Image = this.pointsView.deleteBackgroundOfImage(this.image);
		}
		
		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			// Return true for supported orientations
			if (UserInterfaceIdiomIsPhone) {
				return (toInterfaceOrientation != UIInterfaceOrientation.PortraitUpsideDown);
			} else {
				return true;
			}
		}
	}
}

