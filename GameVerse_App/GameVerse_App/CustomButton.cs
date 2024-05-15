using Xamarin.Forms;

namespace GameVerse_App
{
    public class CustomButton : Button
    {
        public static readonly BindableProperty CustomBorderColorProperty =
           BindableProperty.Create(
               nameof(BorderColor),
               typeof(Color),
               typeof(CustomButton),
               Color.Default);

        public Color CustomBorderColor
        {
            get { return (Color)GetValue(CustomBorderColorProperty); }
            set { SetValue(BorderColorProperty, value); }
        }

        public static readonly BindableProperty CustomBorderRadiusProperty =
            BindableProperty.Create(
                nameof(CustomBorderRadius),
                typeof(int),
                typeof(CustomButton),
                4);

        public int CustomBorderRadius
        {
            get { return (int)GetValue(CustomBorderRadiusProperty); }
            set { SetValue(CustomBorderRadiusProperty, value); }
        }

        public static readonly BindableProperty CustomBorderWidthProperty =
            BindableProperty.Create(
                nameof(CustomBorderWidth),
                typeof(double),
                typeof(CustomButton),
                4.0);

        public double CustomBorderWidth
        {
            get { return (double)GetValue(CustomBorderWidthProperty); }
            set { SetValue(CustomBorderRadiusProperty, value); }
        }

        public static readonly BindableProperty CustomBackgroundColorProperty =
           BindableProperty.Create(
               nameof(CustomBorderColor),
               typeof(Color),
               typeof(CustomButton),
               Color.Default);

        public Color CustomBackgroundColor 
        {
            get { return (Color)GetValue(CustomBackgroundColorProperty); }
            set { SetValue(CustomBackgroundColorProperty, value); }
        }
    }
}
