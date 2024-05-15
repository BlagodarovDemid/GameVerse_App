using Android.Content;
using GameVerse_App.Droid;
using GameVerse_App;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Android.Graphics.Drawables;
using Android.Util;

[assembly: ExportRenderer(typeof(CustomPicker), typeof(CustomPickerRenderer))]
namespace GameVerse_App.Droid
{
    public class CustomPickerRenderer : PickerRenderer
    {
        public CustomPickerRenderer(Context context) : base(context)
        {
        }
        public static void Init() { }
        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Picker> e)
        {
            base.OnElementChanged(e);
            if (e.OldElement == null)
            {
                Control.Background = null;

                var view = (CustomPicker)Element;

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