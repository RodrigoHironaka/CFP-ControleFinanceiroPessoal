using CFP.Dominio.Dominio;
using CFP.Dominio.ObjetoValor;
using CFP.Repositorio.Repositorio;
using Dominio.Dominio;
using Dominio.ObjetoValor;
using NHibernate;
using Repositorio.Repositorios;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CFP.App.Formularios.Financeiros.TelasConfirmacoes
{
    /// <summary>
    /// Lógica interna para ConfirmaEntradaSaidaRefPessoa.xaml
    /// </summary>
    public partial class ConfirmaEntradaSaidaRefPessoa : Window
    {
        #region Carrega Combos
        private void CarregaCombos()
        {
            cmbFormaPagamento.ItemsSource = new RepositorioFormaPagamento(Session)
               .ObterPorParametros(x => x.Situacao == Situacao.Ativo)
               .OrderBy(x => x.Nome)
               .ToList();
        }
        #endregion

        ISession Session;
        Caixa caixa;
        List<ContaPagamento> valoresRefPessoas = new List<ContaPagamento>();

        public ConfirmaEntradaSaidaRefPessoa(List<ContaPagamento> _valoresRefPessoas, Caixa _caixa, ISession _session)
        {
            InitializeComponent();
            Session = _session;
            valoresRefPessoas = _valoresRefPessoas;
            caixa = _caixa;
            txtData.SelectedDate = DateTime.Now;
            CarregaCombos();
        }

        public ConfirmaEntradaSaidaRefPessoa(ContaPagamento _selecionado, Caixa _caixa, ISession _session)
        {
            InitializeComponent();
            Session = _session;
            valoresRefPessoas.Add(_selecionado);
            caixa = _caixa;
            txtData.SelectedDate = DateTime.Now;
            CarregaCombos();
        }

        private void btConfirmar_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(txtData.Text) || cmbFormaPagamento.SelectedItem == null)
            {
                MessageBox.Show("Todos os campos são obrigatórios. Por favor verifique!", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            foreach (var item in valoresRefPessoas)
            {
                FluxoCaixa fluxoCaixa = new FluxoCaixa();
                if (item.Conta.TipoConta == TipoConta.Receber)
                {
                    fluxoCaixa.TipoFluxo = EntradaSaida.Saída;
                    fluxoCaixa.Nome = fluxoCaixa.Nome = String.Format("Saída no caixa ref. {0}", item.Conta.Pessoa.Nome);
                    fluxoCaixa.Valor = item.ValorParcela * -1;
                }
                else
                {
                    fluxoCaixa.TipoFluxo = EntradaSaida.Entrada;
                    fluxoCaixa.Nome = fluxoCaixa.Nome = String.Format("Entrada no caixa ref. {0}.", item.Conta.Pessoa.Nome); 
                    fluxoCaixa.Valor = item.ValorParcela ;
                }
                fluxoCaixa.DataGeracao = DateTime.Now;
                fluxoCaixa.Conta = item.Conta;
                fluxoCaixa.UsuarioCriacao = MainWindow.UsuarioLogado;

                fluxoCaixa.Caixa = caixa;
                fluxoCaixa.FormaPagamento = (FormaPagamento)cmbFormaPagamento.SelectedItem;
                new RepositorioFluxoCaixa(Session).Salvar(fluxoCaixa);
            }
            DialogResult = true;
        }

        private void btCancelar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
