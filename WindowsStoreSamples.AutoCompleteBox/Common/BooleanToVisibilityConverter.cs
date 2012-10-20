using System;
using System.Diagnostics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace WindowsStoreSamples.AutoCompleteBox.Common {
    /// <summary>
    /// Value converter that translates true to <see cref="Visibility.Visible"/> and false to
    /// <see cref="Visibility.Collapsed"/>.
    /// </summary>
    public sealed class BooleanToVisibilityConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, string language) {
            if (parameter == null) {
                return (value is bool && (bool)value) ? Visibility.Visible : Visibility.Collapsed;
            }

            return (value is bool && (bool)value) ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language) {
            if (parameter == null) {
                return value is Visibility && (Visibility)value == Visibility.Visible;
            }

            return value is Visibility && (Visibility)value == Visibility.Collapsed;
        }
    }
}