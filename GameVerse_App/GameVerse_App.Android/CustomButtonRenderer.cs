using Android.Content;
using GameVerse_App.Droid;
using GameVerse_App;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Android.Util;
using System.ComponentModel;

[assembly: ExportRenderer(typeof(CustomButton), typeof(CustomButtonRenderer))]
namespace GameVerse_App.Droid
{
    public class CustomButtonRenderer : ButtonRenderer
    {
        private CustomButton Layer;
        public CustomButtonRenderer(Context context) : base(context)
        {
        }
        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Button> e)
        {
            base.OnElementChanged(e);
            var view = (CustomButton)Element;
            if (view == null) return;
            Paint(view);
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (e.PropertyName == CustomButton.CustomBorderRadiusProperty.PropertyName ||
               e.PropertyName == CustomButton.CustomBorderColorProperty.PropertyName ||
               e.PropertyName == CustomButton.CustomBorderWidthProperty.PropertyName)
            {
                if (Element != null)
                {
                    Paint((CustomButton)Element);
                }
            }
        }
        public static float ToCGColor(Context context, float valueInDp)
        {
            DisplayMetrics metrics = context.Resources.DisplayMetrics;
            return TypedValue.ApplyDimension(ComplexUnitType.Dip, valueInDp, metrics);
        }

        private void Paint(CustomButton view)
        {
            if (Layer == null) return;
            this.Layer.CornerRadius = view.CustomBorderRadius;
            this.Layer.BorderColor = view.CustomBorderColor;
            this.Layer.BackgroundColor = view.CustomBackgroundColor;
            this.Layer.BorderWidth = view.CustomBorderWidth;
        }
    }
}