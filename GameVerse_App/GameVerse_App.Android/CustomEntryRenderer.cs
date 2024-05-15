using Android.Content;
using GameVerse_App.Droid;
using GameVerse_App;
using System;
using Xamarin.Forms;
using Android.Graphics.Drawables;
using Android.Util;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(CustomEntry), typeof(CustomEntryRenderer))]
namespace GameVerse_App.Droid
{
    public class CustomEntryRenderer : EntryRenderer
    {
        public CustomEntryRenderer(Context context) : base(context)
        {
        }
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null)
            {
                var view = (CustomEntry)Element;

                if (view.IsCurvedCornersEnabled)
                {
                    var _gradientBackground = new GradientDrawable();
                    _gradientBackground.SetShape(ShapeType.Rectangle);
                    _gradientBackground.SetColor(view.BackgroundColor.ToAndroid());

                    _gradientBackground.SetStroke(view.BorderWidth, view.BorderColor.ToAndroid());

                    _gradientBackground.SetCornerRadius(
                        DpToPixels(this.Context,
                            Convert.ToSingle(view.CornerRadius)));

                    Control.SetBackground(_gradientBackground);
                }

                Control.SetPadding(
                    (int)DpToPixels(this.Context, Convert.ToSingle(12)),
                    Control.PaddingTop,
                    (int)DpToPixels(this.Context, Convert.ToSingle(12)),
                    Control.PaddingBottom);
            }
        }
        public static float DpToPixels(Context context, float valueInDp)
        {
            DisplayMetrics metrics = context.Resources.DisplayMetrics;
            return TypedValue.ApplyDimension(ComplexUnitType.Dip, valueInDp, metrics);
        }
    }
}