using System;
using System.Globalization;
using System.Windows.Data;

namespace umfg.venda.app.Converters
{
    /// <summary>
    /// Formata um DateTime? para "MM/yyyy".
    /// Usado no TextBox interno do DatePicker de validade do cartão.
    /// </summary>
    [ValueConversion(typeof(DateTime?), typeof(string))]
    internal sealed class MonthYearConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime dt)
                return dt.ToString("MM/yyyy", culture);

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // O TextBox é IsReadOnly=True; ConvertBack não é usado
            return Binding.DoNothing;
        }
    }
}