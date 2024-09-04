using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using AndroidX.AppCompat.Widget;
using AndroidX.Core.Content;
using Microsoft.Maui.Handlers;
using YarraTramsMobileMauiBlazor.CustomHandlers;
using BlendMode = Android.Graphics.BlendMode;
using Color = Android.Graphics.Color;

namespace YarraTramsMobileMauiBlazor.Platforms.Android.ControlHandlers
{
    public class CustomEntryHandler : EntryHandler
    {
        private CustomEntry? element;
        //private Context? _context;

        //public CustomEntryHandler() : base(new PropertyMapper<CustomEntry>())
        //{

        //}

        protected override AppCompatEditText CreatePlatformView()
        {
            var _context = MauiContext?.Context ?? throw new InvalidOperationException("MauiContext is null.");

            var editText = new AppCompatEditText(_context);
            element = VirtualView as CustomEntry;

            if (!string.IsNullOrEmpty(element?.LeftImage))
            {
                var leftDrawable = GetDrawable(element.LeftImage, element.LeftImageHeight, element.LeftImageWidth);
                var rightDrawable = GetDrawable(element.RightImage, element.RightImageWidth, element.RightImageHeight);
                editText.SetCompoundDrawablesWithIntrinsicBounds(leftDrawable, null, rightDrawable, null);
            }

            editText.CompoundDrawablePadding = 25;
            //editText.Background?.SetColorFilter(new BlendModeColorFilter(Color.White, BlendMode.SrcAtop));
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Q)
            {
                editText.Background?.SetColorFilter(new BlendModeColorFilter(Color.White, BlendMode.SrcAtop));
            }
            else
            {
                editText.Background?.SetColorFilter(Color.White, PorterDuff.Mode.SrcAtop);
            }

            return editText;
        }

        private BitmapDrawable GetDrawable(string imageName, int height, int width)
        {
            var context = MauiContext?.Context ?? throw new InvalidOperationException("MauiContext is null.");

            int resID = context.Resources.GetIdentifier(imageName, "drawable", context.PackageName);
            if (resID == 0)
            {
                return null; // Return null if the resource is not found
            }
            var drawable = ContextCompat.GetDrawable(context, resID);
            var bitmap = drawableToBitmap(drawable);

            var scaledBitmap = Bitmap.CreateScaledBitmap(bitmap, width, height, true);

            return new BitmapDrawable(context?.Resources, scaledBitmap);

            //return new BitmapDrawable(Bitmap.CreateScaledBitmap(bitmap, default, element.CustomHeight * 2, true));
        }

        public Bitmap drawableToBitmap(Drawable drawable)
        {
            if (drawable is BitmapDrawable)
            {
                return ((BitmapDrawable)drawable).Bitmap;
            }

            int width = drawable.IntrinsicWidth;
            width = width > 0 ? width : 1;
            int height = drawable.IntrinsicHeight;
            height = height > 0 ? height : 1;

            Bitmap bitmap = Bitmap.CreateBitmap(width, height, Bitmap.Config.Argb8888);
            Canvas canvas = new Canvas(bitmap);
            drawable.SetBounds(0, 0, canvas.Width, canvas.Height);
            drawable.Draw(canvas);

            return bitmap;
        }
    }
}
