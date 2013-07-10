using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Media;

namespace Projectors {
  public class ConnectionStatusColorConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
      return value is bool && (bool) value
                     ? new SolidColorBrush(Color.FromArgb(255, 100, 255, 100))
                     : new SolidColorBrush(Color.FromArgb(255, 255, 100, 100));
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
      return null;
    }
  }

  public class EnumerableRangeValue : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
      var range = parameter as string;
      var list = (value as IEnumerable).Cast<int>().ToList();
      if (!list.Any() || range == null) return null;

      switch (range.ToLower()) {
        case "lower":
          return list.Min();
        case "upper":
          return list.Max();
      }
      return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
      return null;
    }
  }

  public class InverseBoolean : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
      if (value is bool)
        return !(bool) value;
      return false;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
      return null;
    }
  }
}
