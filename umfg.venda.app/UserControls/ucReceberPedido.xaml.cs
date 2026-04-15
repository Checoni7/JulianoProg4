using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using umfg.venda.app.Interfaces;
using umfg.venda.app.Models;
using umfg.venda.app.ViewModels;

namespace umfg.venda.app.UserControls
{
    public partial class ucReceberPedido : UserControl
    {
        private ucReceberPedido(IObserver observer, PedidoModel pedido)
        {
            InitializeComponent();
            DataContext = new ReceberPedidoViewModel(this, observer, pedido);
        }

        internal static void Exibir(IObserver mainWindow, PedidoModel pedido)
        {
            (new ucReceberPedido(mainWindow, pedido).DataContext as ReceberPedidoViewModel).Notify();
        }

        private void dpValidade_CalendarOpened(object sender, RoutedEventArgs e)
        {
            if (sender is DatePicker dp)
            {
                var popup = dp.Template?.FindName("PART_Popup", dp) as Popup;
                if (popup?.Child is Calendar cal)
                {
                    cal.DisplayMode = CalendarMode.Year;
                    cal.DisplayModeChanged -= Cal_DisplayModeChanged;
                    cal.DisplayModeChanged += Cal_DisplayModeChanged;
                }
            }
        }

        private void Cal_DisplayModeChanged(object sender, CalendarModeChangedEventArgs e)
        {
            if (sender is Calendar cal && cal.DisplayMode == CalendarMode.Month)
                cal.DisplayMode = CalendarMode.Year;
        }

        private void dpValidade_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is DatePicker dp && dp.SelectedDate.HasValue)
            {
                var d = dp.SelectedDate.Value;
                dp.SelectedDate = new DateTime(d.Year, d.Month, 1);
            }
        }
    }
}