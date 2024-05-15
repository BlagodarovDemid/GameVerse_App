using Xamarin.Forms;

namespace GameVerse_App
{
    public class CustomPicker : Picker
    {
        public static readonly BindableProperty BorderColorProperty =
           BindableProperty.Create(
           nameof(BorderColor),
           typeof(Color),
           typeof(CustomPicker),
           Color.Gray);

        public Color BorderColor
        {
            get { return (Color)GetValue(BorderColorProperty); }
            set { SetValue(BorderColorProperty, value); }
        }

        public static readonly BindableProperty BorderWidthProperty =
            BindableProperty.Create(
            nameof(BorderWidth),
            typeof(int),
            typeof(CustomPicker),
            Device.OnPlatform<int>(1, 2, 2));

        public int BorderWidth
        {
            get { return (int)GetValue(BorderWidthProperty); }
            set { SetValue(BorderWidthProperty, value); }
        }

        public static readonly BindableProperty CornerRadiusProperty =
            BindableProperty.Create(
            nameof(CornerRadius),
            typeof(double),
            typeof(CustomPicker),
            Device.OnPlatform<double>(6, 7, 7));

        public double CornerRadius
        {
            get { return (double)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        public static readonly BindableProperty IsCurvedCornersEnabledProperty =
            BindableProperty.Create(
            nameof(IsCurvedCornersEnabled),
            typeof(bool),
            typeof(CustomPicker),
            true);

        public bool IsCurvedCornersEnabled
        {
            get { return (bool)GetValue(IsCurvedCornersEnabledProperty); }
            set { SetValue(IsCurvedCornersEnabledProperty, value); }
        }
    }
}
