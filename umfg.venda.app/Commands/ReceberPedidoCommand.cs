using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using umfg.venda.app.Abstracts;
using umfg.venda.app.UserControls;
using umfg.venda.app.ViewModels;

namespace umfg.venda.app.Commands
{
    internal sealed class ReceberPedidoCommand : AbstractCommand
    {
        /// <summary>
        /// Habilitado quando há pelo menos um produto no pedido.
        /// O parâmetro aqui é o ListarProdutosViewModel (contexto da tela de listagem).
        /// </summary>
        public override bool CanExecute(object? parameter)
        {
            if (parameter is ListarProdutosViewModel listarVm)
                return listarVm.Pedido?.Produtos?.Any() == true;

            if (parameter is ReceberPedidoViewModel receberVm)
                return receberVm.Pedido?.Produtos?.Any() == true;

            return false;
        }

        public override void Execute(object? parameter)
        {
            // ── Chamado da tela de listagem ──────────────────────────────────────
            if (parameter is ListarProdutosViewModel listarVm)
            {
                ucReceberPedido.Exibir(listarVm.MainWindow, listarVm.Pedido);
                return;
            }

            // ── Chamado da tela de pagamento (botão "RECEBER" do formulário) ─────
            if (parameter is ReceberPedidoViewModel receberVm)
            {
                var erros = Validar(receberVm);

                if (erros.Count > 0)
                {
                    var sb = new StringBuilder();
                    sb.AppendLine("Por favor, corrija os seguintes erros antes de continuar:");
                    sb.AppendLine();
                    foreach (var erro in erros)
                        sb.AppendLine($"  • {erro}");

                    MessageBox.Show(sb.ToString(), "Dados Inválidos",
                                    MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Sucesso
                MessageBox.Show("Pagamento processado com sucesso! Obrigado pela sua compra.",
                                "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);

                receberVm.NumeroCartao = string.Empty;
                receberVm.CVV = string.Empty;
                receberVm.DataValidade = null;
                receberVm.NomeCartao = string.Empty;
                receberVm.TipoCartao = 0;

                receberVm.VoltarParaInicio();
                return;
            }

            MessageBox.Show("Parâmetro obrigatório não informado! Verifique.",
                            "Erro Interno", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        // ── Validações ────────────────────────────────────────────────────────────

        private static List<string> Validar(ReceberPedidoViewModel vm)
        {
            var erros = new List<string>();

            if (string.IsNullOrWhiteSpace(vm.NomeCartao))
                erros.Add("Nome no cartão é obrigatório.");

            if (vm.TipoCartao == 0)
                erros.Add("Selecione o tipo do cartão (Crédito ou Débito).");

            if (string.IsNullOrWhiteSpace(vm.NumeroCartao))
            {
                erros.Add("Número do cartão é obrigatório.");
            }
            else
            {
                var numeroLimpo = vm.NumeroCartao.Replace(" ", "").Replace("-", "");
                if (!long.TryParse(numeroLimpo, out _))
                    erros.Add("Número do cartão deve conter apenas dígitos.");
                else if (!LuhnValido(numeroLimpo))
                    erros.Add("Número do cartão inválido (falha na verificação Luhn).");
            }

            if (string.IsNullOrWhiteSpace(vm.CVV))
            {
                erros.Add("CVV é obrigatório.");
            }
            else if (vm.CVV.Length != 3 || !long.TryParse(vm.CVV, out _))
            {
                erros.Add("CVV deve conter exatamente 3 dígitos numéricos.");
            }

            if (vm.DataValidade is null)
            {
                erros.Add("Data de validade é obrigatória.");
            }
            else
            {
                var hoje = DateTime.Today;
                var validade = vm.DataValidade.Value;
                var ultimoDia = new DateTime(validade.Year, validade.Month,
                                            DateTime.DaysInMonth(validade.Year, validade.Month));
                if (ultimoDia < hoje)
                    erros.Add("Data de validade do cartão está vencida.");
            }

            return erros;
        }

        private static bool LuhnValido(string numero)
        {
            int soma = 0;
            bool dobrar = false;

            for (int i = numero.Length - 1; i >= 0; i--)
            {
                if (!char.IsDigit(numero[i])) return false;

                int digito = numero[i] - '0';

                if (dobrar)
                {
                    digito *= 2;
                    if (digito > 9) digito -= 9;
                }

                soma += digito;
                dobrar = !dobrar;
            }

            return soma % 10 == 0;
        }
    }
}