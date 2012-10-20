using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace WindowsStoreSamples.AutoCompleteBox {
    public class UpdateSourceHelper : FrameworkElement {
        public static string GetUpdateSourceText(DependencyObject obj) {
            return (string)obj.GetValue(UpdateSourceTextProperty);
        }

        public static void SetUpdateSourceText(DependencyObject obj, string value) {
            obj.SetValue(UpdateSourceTextProperty, value);
        }

        // Using a DependencyProperty as the backing store for UpdateSourceText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UpdateSourceTextProperty =
            DependencyProperty.RegisterAttached("UpdateSourceText", typeof(string), typeof(UpdateSourceHelper), new PropertyMetadata(""));

        public static bool GetIsEnabled(DependencyObject obj) {
            return (bool)obj.GetValue(IsEnabledProperty);
        }

        public static void SetIsEnabled(DependencyObject obj, bool value) {
            obj.SetValue(IsEnabledProperty, value);
        }

        // Using a DependencyProperty as the backing store for IsEnabled.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsEnabledProperty =
            DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(UpdateSourceHelper),
                new PropertyMetadata(false,
            // property changed
                (obj, args) => {
                    if (obj is TextBox) {
                        TextBox tb = (TextBox)obj;
                        if ((bool)args.NewValue) {
                            tb.TextChanged += AttachedTextBoxTextChanged;
                        } else {
                            tb.TextChanged -= AttachedTextBoxTextChanged;
                        }
                    }
                }
            ));

        static void AttachedTextBoxTextChanged(object sender, TextChangedEventArgs e) {
            if (sender is TextBox) {
                var tb = (TextBox)sender;
                tb.SetValue(UpdateSourceHelper.UpdateSourceTextProperty, tb.Text);
            }
        }
    }
}