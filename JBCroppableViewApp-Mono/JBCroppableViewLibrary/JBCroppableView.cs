using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using MonoTouch;
using MonoTouch.UIKit;
using Microsoft;
using MonoTouch.CoreGraphics;

namespace JBCroppableViewLibrary
{
	public class JBCroppableView : UIView
	{
		static RectangleF CGRectZero = new RectangleF (new PointF (0.0f, 0.0f), new SizeF (0.0f, 0.0f));
		static PointF CGPointZero = new PointF(0.0f, 0.0f);

		const float K_POINT_WIDTH = 30f;

		public UIColor pointColor {get;set;}
		public UIColor lineColor {get;set;}

		public IList<UIView> points {get;set;}
		public UIView activePoint {get;set;}

		PointF lastPoint;
		UIBezierPath LastBezierPath;
		bool isContainView;
		public JBCroppableView (UIImageView image) : base (image.Frame)
		{
			this.BackgroundColor = UIColor.Clear;
			this.pointColor      = UIColor.Blue;
			this.lineColor       = UIColor.Yellow;
			this.ClipsToBounds = true;
		}
		public void addPointsAt(IList<PointF> points){
			this.points = new List<UIView>();
			int i=0;
			foreach(PointF point in points){
				UIView pointToAdd = getPointView(i, point);
				this.points.Add(pointToAdd);
				this.AddSubview(pointToAdd);
				i++;
			}
		}
		private UIView getPointView(int num, PointF pointAt){
			UIView point1 = new UIView(new RectangleF(pointAt.X - K_POINT_WIDTH/2, pointAt.Y - K_POINT_WIDTH/2, K_POINT_WIDTH, K_POINT_WIDTH));
			point1.Alpha = 0.8f;
			point1.BackgroundColor    = this.pointColor;
			point1.Layer.BorderColor  = this.lineColor.CGColor;
			point1.Layer.BorderWidth  = 4f;
			point1.Layer.CornerRadius = K_POINT_WIDTH/2;
			
			UILabel number = new UILabel(new RectangleF(0,0,K_POINT_WIDTH, K_POINT_WIDTH));
			number.Text = String.Format("{0}", num);
			number.TextColor = UIColor.White;
			number.BackgroundColor = UIColor.Clear;
			number.Font = UIFont.SystemFontOfSize(14);
			number.TextAlignment = UITextAlignment.Center;
			
			point1.AddSubview(number);
			
			return point1;
		}
		public void addPoints(int num){
		
			if (num <= 0) return;
			IList<UIView> tmp = new List<UIView>();

			//NSMutableArray *tmp = [NSMutableArray array];
			int pointsAdded     = 0;
			int pointsToAdd     = num -1;
			float pointsPerSide = 0.0f;
			
			if (num > 4)
				pointsPerSide = (num-4) /4.0f;
			
			// Corner 1
			UIView point = getPointView(pointsAdded, new PointF(20,20));
			tmp.Add(point);
			this.AddSubview(point);
			pointsAdded ++;
			pointsToAdd --;

			// Upper side
			if (pointsPerSide - (int)pointsPerSide >= 0.25)
				pointsPerSide ++;
			
			for (uint i=0; i<(int)pointsPerSide; i++)
			{
				float x = ((this.Frame.Size.Width -40) / ((int)pointsPerSide +1)) * (i+1);
				
				point = getPointView(pointsAdded, new PointF(x+20,20));
				tmp.Add(point);
				this.AddSubview(point);
				pointsAdded ++;
				pointsToAdd --;
			}
			
			if (pointsPerSide - (int)pointsPerSide >= 0.25)
				pointsPerSide --;

			// Corner 2
			point = getPointView(pointsAdded, new PointF(this.Frame.Size.Width - 20, 20));
			tmp.Add(point);
			this.AddSubview(point);
			pointsAdded ++;
			pointsToAdd --;
			
			// Right side
			if (pointsPerSide - (int)pointsPerSide >= 0.5)
				pointsPerSide ++;
			
			for (uint i=0; i<(int)pointsPerSide; i++)
			{
				float y = (this.Frame.Size.Height -40) / ((int)pointsPerSide +1)  * (i+1);
				
				point = getPointView(pointsAdded, new PointF(this.Frame.Size.Width -20, 20+y));
				tmp.Add(point);
				this.AddSubview(point);
				pointsAdded ++;
				pointsToAdd --;
			}
			
			if (pointsPerSide - (int)pointsPerSide >= 0.5)
				pointsPerSide --;

			// Corner 3
			point = getPointView(pointsAdded, new PointF(this.Frame.Size.Width -20, this.Frame.Size.Height -20));
			tmp.Add(point);
			this.AddSubview(point);
			pointsAdded ++;
			pointsToAdd --;
			
			// Bottom side
			if (pointsPerSide - (int)pointsPerSide >= 0.75)
				pointsPerSide ++;
			
			for (int i=(int)pointsPerSide; i > 0; i--)
			{
				float x = (this.Frame.Size.Width -40) / ((int)pointsPerSide +1) * i;
				
				point = getPointView(pointsAdded, new PointF(x +20, this.Frame.Size.Height -20));
				tmp.Add(point);
				this.AddSubview(point);
				pointsAdded ++;
				pointsToAdd --;
			}
			
			if (pointsPerSide - (int)pointsPerSide >= 0.75)
				pointsPerSide --;

			// Corner 4
			point = getPointView(pointsAdded, new PointF(20, this.Frame.Size.Height -20));
			tmp.Add(point);
			this.AddSubview(point);
			pointsAdded ++;
			pointsToAdd --;
			
			// Left side
			for (int i=(int)pointsPerSide; i>0; i--)
			{
				float y = (this.Frame.Size.Height -40) / (pointsPerSide +1) * i;
				
				point = getPointView(pointsAdded, new PointF(20, 20+y));
				tmp.Add(point);
				this.AddSubview(point);
				pointsAdded ++;
				pointsToAdd --;
			}
			
			this.points = tmp;
		}
		public override void Draw (RectangleF rect)
		{
			base.Draw (rect);

			if (this.points.Count <= 0) return;
			
			// get the current context
			CGContext context = UIGraphics.GetCurrentContext ();
			context.ClearRect(this.Frame);
			
			float[] components = this.lineColor.CGColor.Components; //CGColor;// CGColorGetComponents(self.lineColor.CGColor);
			
			float red;
			float green;
			float blue;
			float alpha;
			
			if(components.Length == 2)
			{
				red   = 1;
				green = 1;
				blue  = 1;
				alpha = 1;
			}
			else
			{
				red   = components[0];
				green = components[1];
				blue  = components[2];
				alpha = components[3];
				if (alpha <= 0) alpha = 1;
			}
			
			
			// set the stroke color and width
			context.SetRGBStrokeColor(red, green, blue, alpha);
			context.SetLineWidth(2.0f);
			
			UIView point1 = this.points[0];
			context.MoveTo(point1.Frame.Location.X +K_POINT_WIDTH/2, point1.Frame.Location.Y +K_POINT_WIDTH/2);
			
			for (int i=1; i<this.points.Count; i++)
			{
				UIView point = this.points[i];
				context.AddLineToPoint(point.Frame.Location.X +K_POINT_WIDTH/2, point.Frame.Location.Y +K_POINT_WIDTH/2);
			}
			context.AddLineToPoint(point1.Frame.Location.X +K_POINT_WIDTH/2, point1.Frame.Location.Y +K_POINT_WIDTH/2);
			
			// tell the context to draw the stroked line
			context.StrokePath();
		}
		public UIImage deleteBackgroundOfImage(UIImageView image){
			if (this.points.Count <= 0) return null;
			
			var points = this.getPoints();

			RectangleF rect = CGRectZero;
			rect.Size = image.Image.Size;

			UIGraphics.BeginImageContextWithOptions(rect.Size, true, 0.0f);
			UIColor.Black.SetFill();
			UIGraphics.RectFill(rect);
			UIColor.White.SetFill();

			UIBezierPath aPath = new UIBezierPath();
			
			// Set the starting point of the shape.
			PointF p1 = JBCroppableView.convertCGPoint(points[0],image.Frame.Size,image.Image.Size);
			aPath.MoveTo(new PointF(p1.X, p1.Y));
			
			for (int i=1; i<points.Count; i++)
			{
				PointF p = JBCroppableView.convertCGPoint(points[i], image.Frame.Size, image.Image.Size);
				aPath.AddLineTo(new PointF(p.X, p.Y));
			}
			aPath.ClosePath();
			aPath.Fill();

			UIImage mask = UIGraphics.GetImageFromCurrentImageContext();
			UIGraphics.EndImageContext();
			
			UIGraphics.BeginImageContextWithOptions(rect.Size, false, 0.0f);
			
			{
				UIGraphics.GetCurrentContext().ClipToMask(rect, mask.CGImage);
				image.Image.Draw(CGPointZero);
			}
			
			UIImage maskedImage = UIGraphics.GetImageFromCurrentImageContext();
			UIGraphics.EndImageContext();

			return maskedImage;
		}
		private IList<PointF> getPoints(){
			IList<PointF> p = new List<PointF>();
			for (int i=0; i<this.points.Count; i++)
			{
				UIView v = this.points[i];
				PointF point = new PointF(v.Frame.Location.X +K_POINT_WIDTH/2, v.Frame.Location.Y +K_POINT_WIDTH/2);
				p.Add(point);
			}
			return p;
		}
		private bool pointInside(PointF point, UIEvent ev){
			if (this.points.Count <= 0) return false;
			
			PointF locationPoint = point;
			
			foreach (UIView pointView in this.points)
			{
				PointF viewPoint = pointView.ConvertPointFromView(locationPoint, this); //[pointView convertPoint:locationPoint fromView:self];
				
				if (pointView.PointInside(viewPoint, ev))
				{
					return  true;
				}
			}
			
			lastPoint = locationPoint;
			
			LastBezierPath.RemoveAllPoints();
			
			IList<PointF> points = this.getPoints();
			
			SizeF rectSize = this.Frame.Size;
			
			// Set the starting point of the shape.
			PointF p1 = JBCroppableView.convertCGPoint(points[0], rectSize, rectSize);//[[points objectAtIndex:0] CGPointValue] fromRect1:rectSize toRect2:rectSize];
			LastBezierPath.MoveTo(new PointF(p1.X, rectSize.Height - p1.Y));
			
			for (int i=1; i<points.Count; i++)
			{
				PointF p = JBCroppableView.convertCGPoint(points[i],rectSize,rectSize);
				LastBezierPath.AddLineTo(new PointF(p.X, rectSize.Height - p.Y));
			}
			
			LastBezierPath.ClosePath();
			
			isContainView = LastBezierPath.ContainsPoint(point);
			
			return isContainView;
		}
		public override void TouchesBegan (MonoTouch.Foundation.NSSet touches, UIEvent evt)
		{
			base.TouchesBegan (touches, evt);

			UITouch touch = touches.AnyObject as UITouch;
			
			if(touch != null)
			{
				PointF locationPoint = touch.LocationInView(this);
				
				foreach (UIView point in this.points)
				{
					PointF viewPoint = point.ConvertPointFromView(locationPoint,this);
					
					if (point.PointInside(viewPoint, evt))
					{
						this.activePoint = point;
						this.activePoint.BackgroundColor = UIColor.Red;
						
						break;
					}
				}
				
				lastPoint = locationPoint;
			} 
		}
		public override void TouchesMoved (MonoTouch.Foundation.NSSet touches, UIEvent evt)
		{
			base.TouchesMoved (touches, evt);
			UITouch touch = touches.AnyObject as UITouch;
			
			if(touch != null)
			{
				PointF locationPoint = touch.LocationInView(this);
				if (this.activePoint == null)
				{
					//if in BezierPath,move the view,else don't move
					if (isContainView) {
						foreach(UIView point in this.points)
						{
							point.Frame.Offset(new PointF(locationPoint.X - lastPoint.X, locationPoint.Y -lastPoint.Y));
						}
						this.SetNeedsDisplay();
					}
				}
				else
				{
					this.activePoint.Frame = new RectangleF(locationPoint.X -K_POINT_WIDTH/2, locationPoint.Y -K_POINT_WIDTH/2, K_POINT_WIDTH, K_POINT_WIDTH);
					this.SetNeedsDisplay();
				}
				lastPoint = locationPoint;
			}
		}
		public override void TouchesEnded (MonoTouch.Foundation.NSSet touches, UIEvent evt)
		{
			base.TouchesEnded (touches, evt);
			if(this.activePoint != null){
				this.activePoint.BackgroundColor = this.pointColor;
				this.activePoint = null;
			}
		}
		public override void TouchesCancelled (MonoTouch.Foundation.NSSet touches, UIEvent evt)
		{
			base.TouchesCancelled (touches, evt);

			if(this.activePoint != null){
				this.activePoint.BackgroundColor = this.pointColor;
				this.activePoint = null;
			}
		}
		#region Static methods
		public static RectangleF scaleRespectAspectFromRect1(RectangleF rect1, RectangleF rect2){
			SizeF scaledSize = rect2.Size;

			float scaleFactor = 1.0f;
			
			float widthFactor  = rect2.Size.Width / rect1.Size.Width;
			float heightFactor = rect2.Size.Height / rect1.Size.Width;

			if (widthFactor < heightFactor)
				scaleFactor = widthFactor;
			else
				scaleFactor = heightFactor;

			scaledSize.Height = rect1.Size.Height *scaleFactor;
			scaledSize.Width  = rect1.Size.Width  *scaleFactor;

			float y = (rect2.Size.Height - scaledSize.Height)/2;
			
			return new RectangleF(0, y, scaledSize.Width, scaledSize.Height);

		}
		public static PointF convertCGPoint(PointF point1, SizeF rect1, SizeF rect2){
			point1.Y = rect1.Height - point1.Y;
			return new PointF((point1.X*rect2.Width)/rect1.Width, (point1.Y*rect2.Height)/rect1.Height);
		}
		public static PointF convertPoint(PointF point1, SizeF rect1, SizeF rect2){
			return new PointF((point1.X*rect2.Width)/rect1.Width, (point1.Y*rect2.Height)/rect1.Height);;
		}		
		#endregion
	}
}

