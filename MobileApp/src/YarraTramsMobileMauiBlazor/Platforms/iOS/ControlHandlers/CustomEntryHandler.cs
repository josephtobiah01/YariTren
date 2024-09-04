using Microsoft.Maui.Platform;
using Microsoft.Maui.Handlers;
using YarraTramsMobileMauiBlazor.CustomHandlers;
using UIKit;
using System.Drawing;

namespace YarraTramsMobileMauiBlazor.Platforms.iOS.ControlHandlers
{
    public class CustomEntryHandler : EntryHandler
    {
        private CustomEntry element;

        protected override MauiTextField CreatePlatformView()
        {
            var textField = new MauiTextField();
            element = VirtualView as CustomEntry;

            if (!string.IsNullOrEmpty(element?.LeftImage))
            {
                textField.LeftViewMode = UITextFieldViewMode.Always;
                textField.LeftView = GetImageView(element.LeftImage, element.LeftImageHeight, element.LeftImageWidth);
                
            }
            if (!string.IsNullOrEmpty(element.RightImage))
            {
                textField.LeftViewMode = UITextFieldViewMode.Always;
                textField.LeftView = GetImageView(element.RightImage, element.RightImageHeight, element.RightImageWidth);

            }

            textField.BorderStyle = UIKit.UITextBorderStyle.Line;
            textField.Layer.MasksToBounds = true;
            return textField;
        }

        private UIView GetImageView(string imagePath, int height, int width)
        {
            var uiImageView = new UIImageView(UIImage.FromFile(imagePath))
            {
                Frame = new RectangleF(7, 0, width, height)
            };
            UIView objLeftView = new UIView(new System.Drawing.Rectangle(0, 0, width + 10, height));
            objLeftView.AddSubview(uiImageView);

            return objLeftView;
        }
    }
}
