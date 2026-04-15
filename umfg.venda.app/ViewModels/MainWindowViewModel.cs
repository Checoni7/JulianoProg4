using System.Windows;
using System.Windows.Controls;
using umfg.venda.app.Abstracts;
using umfg.venda.app.Commands;
using umfg.venda.app.Interfaces;

namespace umfg.venda.app.ViewModels
{
    internal sealed class MainWindowViewModel : AbstractViewModel, IObserver
    {
        private UserControl _userControl;

        public UserControl UserControl
        {
            get => _userControl;
            set => SetField(ref _userControl, value);
        }

        public ListarProdutosCommand ListarProdutos { get; private set; } = new();

        public MainWindowViewModel() : base("UMFG - Tela Principal") { }

        public void Update(ISubject subject)
        {
            if (subject is ListarProdutosViewModel listarVm)
            {
                UserControl = listarVm.UserControl;
                return;
            }

            if (subject is ReceberPedidoViewModel receberVm)
            {
                // ── Navegação condicional ────────────────────────────────────────
                // Só avança para a tela de pagamento se houver itens no pedido.
                // Se o pedido estiver vazio (após sucesso, VoltarParaInicio reseta
                // o Pedido), volta para a tela inicial em vez de exibir o pagamento.
                if (receberVm.Pedido?.Produtos?.Count > 0)
                {
                    UserControl = receberVm.UserControl;
                }
                else
                {
                    // Pedido vazio → retorna para a tela de listagem/início
                    // O ListarProdutosCommand sabe como exibir a tela inicial
                    ListarProdutos.Execute(this);
                }
            }
        }
    }
}