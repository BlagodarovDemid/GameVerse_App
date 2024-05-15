using Android.Content;
using GameVerse_App.Droid;
using GameVerse_App;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Android.Graphics.Drawables;
using Android.Util;
using Android.Graphics;

[assembly: ExportRenderer(typeof(CustomPickerRegion), typeof(CustomPickerRegionRenderer))]
namespace GameVerse_App.Droid
{
    public class CustomPickerRegionRenderer : PickerRenderer
    {
        public CustomPickerRegionRenderer(Context context) : base(context)
        {
        }
        public static void Init() { }
        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Picker> e)
        {
            base.OnElementChanged(e);
            if (e.OldElement == null)
            {

                var view = (CustomPickerRegion)Element;

                if (view.IsCurvedCornersEnabled)
                {
                    var _gradientBackground = new GradientDrawable();
                    _gradientBackground.SetShape(ShapeType.Rectangle);
                    _gradientBackground.SetColor(view.BackgroundColor.ToAndroid());
                    
                    _gradientBackground.SetStroke(view.BorderWidth, view.BorderColor.ToAndroid());

                    _gradientBackground.SetCornerRadius(
                        DpToPixels(this.Context,
                            Convert.ToSingle(view.CornerRadius)));

                    Drawable backgroundImage = Context.GetDrawable(Resource.Drawable.RegionWorld_icon);

                    BitmapDrawable bitmapDrawable = (BitmapDrawable)backgroundImage;

                    Bitmap bitmap = bitmapDrawable.Bitmap;

                    Bitmap scaledBitmap = Bitmap.CreateScaledBitmap(bitmap, 100, 200, false);

                    Drawable scaledDrawable = new BitmapDrawable(Resources, scaledBitmap);

                    LayerDrawable layerDrawable = new LayerDrawable(new Drawable[] { scaledDrawable, _gradientBackground });

                    Control.SetBackground(layerDrawable);
                }


                var layoutParams = new MarginLayoutParams(Control.LayoutParameters);
                layoutParams.SetMargins(0, 0, 0, 0);
                LayoutParameters = layoutParams;
                Control.LayoutParameters = layoutParams;
                Control.SetPadding(25, 0, 0, 0);
                SetPadding(0, 0, 0, 0);
            }
        }
        public static float DpToPixels(Context context, float valueInDp)
        {
            DisplayMetrics metrics = context.Resources.DisplayMetrics;
            return TypedValue.ApplyDimension(ComplexUnitType.Dip, valueInDp, metrics);
        }
    }
}