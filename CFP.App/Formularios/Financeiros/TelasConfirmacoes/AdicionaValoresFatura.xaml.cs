using CFP.Repositorio.Repositorio;
using Dominio.ObjetoValor;
using NHibernate;
using Repositorio.Repositorios;
using System;
using System.Collections.Generic;
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
    /// Lógica interna para AdicionaValoresFatura.xaml
    /// </summary>
    public partial class AdicionaValoresFatura : Window
    {
        ISession Session;

        #region Carrega combo
        private void CarregaCombo()
        {
            cmbGrupo.ItemsSource = new RepositorioSubGrupoGasto(Session)
           .ObterPorParametros(x => x.Situacao == Situacao.Ativo)
           .OrderBy(x => x.GrupoGasto.Nome).ThenBy(x => x.Nome)
           .ToList();
            cmbGrupo.SelectedIndex = 0;

            cmbCartao.ItemsSource = new RepositorioFormaPagamento(Session)
               .ObterPorParametros(x => x.Situacao == Situacao.Ativo && x.UsadoParaCompras == Dominio.ObjetoValor.SimNao.Sim)
               .OrderBy(x => x.Nome)
               .ToList();
            cmbCartao.SelectedIndex = 0;

           cmbRefPessoa.ItemsSource = new RepositorioPessoa(Session)
          .ObterPorParametros(x => x.Situacao == Situacao.Ativo)
          .OrderBy(x => x.Nome)
          .ToList();
        }
        #endregion

        public AdicionaValoresFatura(ISession _session)
        {
            InitializeComponent();
            Session = _session;
        }

        private void txtValor_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }

        private void txtValor_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = Regex.IsMatch(e.Text, @"[^0-9,]+");
        }

        private void txtValor_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!String.IsNullOrEmpty(txtValor.Text))
            {
                Decimal valido;
                var valor = decimal.TryParse(txtValor.Text, out valido);
                if (!valor)
                {
                    MessageBox.Show("Texto colado é inválido! Por favor verifique.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtValor.Clear();
                    txtValor.Focus();
                }
            }
        }

        private void txtQtd_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }

        private void txtQtd_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = Regex.IsMatch(e.Text, @"[^0-9]+");
        }

        private void txtQtd_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!String.IsNullOrEmpty(txtQtd.Text))
            {
                Int32 valido;
                var qtd = Int32.TryParse(txtQtd.Text, out valido);
                if (!qtd)
                {
                    MessageBox.Show("Texto colado é inválido! Por favor verifique.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtQtd.Clear();
                    txtQtd.Focus();
                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CarregaCombo();
            txtData.SelectedDate = DateTime.Now;
        }

        private void btCancelar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void txtQtd_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtQtd.Text) || txtQtd.Text.Equals("0"))
                txtQtd.Text = "1";
        }
    }
}
