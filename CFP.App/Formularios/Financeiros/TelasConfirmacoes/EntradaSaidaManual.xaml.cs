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
    /// Lógica interna para EntradaSaidaManual.xaml
    /// </summary>
    public partial class EntradaSaidaManual : Window
    {

        ISession Session;
        Boolean Entrada;
        Caixa caixa;

        #region Carrega combo
        private void CarregaCombo()
        {
            cmbFormaPagamento.ItemsSource = new RepositorioFormaPagamento(Session)
               .ObterPorParametros(x => x.Situacao == Situacao.Ativo)
               .OrderBy(x => x.Nome)
               .ToList();
        }
        #endregion
        public EntradaSaidaManual(Boolean _entrada, Caixa _caixa, ISession _session)
        {
            InitializeComponent();
            Session = _session;
            Entrada = _entrada;
            caixa = _caixa;
        }

        private void btConfirmar_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(txtValorInicial.Text) || String.IsNullOrEmpty(txtNome.Text) || cmbFormaPagamento.SelectedItem == null)
            {
                MessageBox.Show("Todos os campos são Obrigatórios. Por favor verifique!", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            FluxoCaixa fluxoCaixa = new FluxoCaixa();
            if (Entrada)
            {
                fluxoCaixa.TipoFluxo = EntradaSaida.Entrada;
                fluxoCaixa.Nome = fluxoCaixa.Nome = String.Format("Entrada manual no caixa. Descrição: {0}", txtNome.Text);
                fluxoCaixa.Valor = Decimal.Parse(txtValorInicial.Text);
            }
            else
            {
                fluxoCaixa.TipoFluxo = EntradaSaida.Saída;
                fluxoCaixa.Nome = fluxoCaixa.Nome = String.Format("Saída manual no caixa. Descrição: {0}", txtNome.Text);
                fluxoCaixa.Valor = Decimal.Parse(txtValorInicial.Text) * -1;
            }
            fluxoCaixa.DataGeracao = DateTime.Now;
            fluxoCaixa.Conta = null;
            fluxoCaixa.UsuarioCriacao = MainWindow.UsuarioLogado;
            
            fluxoCaixa.Caixa = caixa;
            fluxoCaixa.FormaPagamento = (FormaPagamento)cmbFormaPagamento.SelectedItem;
            new RepositorioFluxoCaixa(Session).Salvar(fluxoCaixa);
            Close();
        }

        private void txtValorInicial_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = Regex.IsMatch(e.Text, @"[^0-9]+");
        }

        private void txtValorInicial_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }

        private void btCancelar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CarregaCombo();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void txtValorInicial_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!String.IsNullOrEmpty(txtValorInicial.Text))
            {
                Decimal valido;
                var valor = decimal.TryParse(txtValorInicial.Text, out valido);
                if (!valor)
                {
                    MessageBox.Show("Texto colado é inválido! Por favor verifique.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtValorInicial.Clear();
                    txtValorInicial.Focus();
                }
            }
        }
    }
}
