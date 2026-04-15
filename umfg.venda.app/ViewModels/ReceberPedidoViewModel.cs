using System;
using System.Windows.Controls;
using umfg.venda.app.Abstracts;
using umfg.venda.app.Commands;
using umfg.venda.app.Interfaces;
using umfg.venda.app.Models;

namespace umfg.venda.app.ViewModels
{
    internal sealed class ReceberPedidoViewModel : AbstractViewModel
    {
        // ── Campos de pagamento ───────────────────────────────────────────────────

        private PedidoModel _pedido = new();
        private string _numeroCartao = string.Empty;
        private string _cvv = string.Empty;
        private DateTime? _dataValidade = null;
        private string _nomeCartao = string.Empty;
        private int _tipoCartao = 0;   // 0 = não selecionado, 1 = Crédito, 2 = Débito

        public string NumeroCartao
        {
            get => _numeroCartao;
            set
            {
                SetField(ref _numeroCartao, value);
                ReceberPedido.RaiseCanExecuteChanged();
            }
        }

        public string CVV
        {
            get => _cvv;
            set => SetField(ref _cvv, value);
        }

        /// <summary>
        /// Armazena apenas mês e ano selecionados pelo usuário.
        /// O dia será sempre 1 — a expiração real é o último dia desse mês.
        /// </summary>
        public DateTime? DataValidade
        {
            get => _dataValidade;
            set => SetField(ref _dataValidade, value);
        }

        public string NomeCartao
        {
            get => _nomeCartao;
            set => SetField(ref _nomeCartao, value);
        }

        /// <summary>0 = não selecionado | 1 = Crédito | 2 = Débito</summary>
        public int TipoCartao
        {
            get => _tipoCartao;
            set => SetField(ref _tipoCartao, value);
        }

        public PedidoModel Pedido
        {
            get => _pedido;
            set
            {
                SetField(ref _pedido, value);
                ReceberPedido.RaiseCanExecuteChanged();
            }
        }

        // ── Comando ───────────────────────────────────────────────────────────────

        public ReceberPedidoCommand ReceberPedido { get; } = new();

        // ── Construtor ────────────────────────────────────────────────────────────

        public ReceberPedidoViewModel(UserControl userControl, IObserver observer, PedidoModel pedido)
            : base("Receber Pedido")
        {
            UserControl = userControl ?? throw new ArgumentNullException(nameof(userControl));
            MainWindow = observer ?? throw new ArgumentNullException(nameof(observer));
            Pedido = pedido ?? throw new ArgumentNullException(nameof(pedido));

            Add(observer);
        }

        // ── Navegação ─────────────────────────────────────────────────────────────

        /// <summary>
        /// Chamado pelo Command após pagamento bem-sucedido para retornar à tela inicial.
        /// Dispara o Observer para que o MainWindowViewModel troque o UserControl.
        /// </summary>
        public void VoltarParaInicio()
        {
            // Limpa o pedido para reiniciar o fluxo
            Pedido = new PedidoModel();
            Notify();   // AbstractViewModel.Notify() → MainWindowViewModel.Update()
        }
    }
}