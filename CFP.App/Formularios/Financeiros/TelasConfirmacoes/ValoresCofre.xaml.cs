using CFP.Dominio.Dominio;
using CFP.Dominio.ObjetoValor;
using CFP.Repositorio.Repositorio;
using Dominio.Dominio;
using Dominio.ObejtoValor;
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
    /// Lógica interna para ValoresCofre.xaml
    /// </summary>
    public partial class ValoresCofre : Window
    {

        ISession Session;
        Cofre cofre;

        #region Repositorio
        private RepositorioCofre _repositorio;
        public RepositorioCofre Repositorio
        {
            get
            {
                if (_repositorio == null)
                    _repositorio = new RepositorioCofre(Session);

                return _repositorio;
            }
            set { _repositorio = value; }
        }
        #endregion

        #region Carrega combo
        private void CarregaCombo()
        {
            cmbSituacaoCofre.ItemsSource = Enum.GetValues(typeof(SituacaoCofre));
            cmbSituacaoCofre.SelectedIndex = 0;

            cmbBanco.ItemsSource = new RepositorioBanco(Session)
               .ObterPorParametros(x => x.Situacao == Situacao.Ativo)
               .OrderBy(x => x.Nome)
               .ToList();
        }
        #endregion

        #region Preenche Objeto para Salvar
        private bool PreencheObjeto()
        {
            try
            {
                cofre = new Cofre();
                cofre.Codigo = Repositorio.RetornaUltimoCodigo() + 1;
                cofre.Nome = txtNome.Text;
                cofre.Valor = Decimal.Parse(txtValor.Text);
                cofre.Situacao = (SituacaoCofre)cmbSituacaoCofre.SelectedItem;
                cofre.Banco = (Banco)cmbBanco.SelectedItem;
                return true;
            }
            catch
            {
                return false;
            }

        }
        #endregion
        public ValoresCofre(ISession _session)
        {
            InitializeComponent();
            Session = _session;
        }

        private void btConfirmar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtValor.Text) || string.IsNullOrEmpty(txtNome.Text) || cmbBanco.SelectedItem == null || cmbSituacaoCofre.SelectedIndex == -1)
            {
                MessageBox.Show("Todos os campos são Obrigatórios. Por favor verifique!", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (PreencheObjeto())
            {
                if (cofre.Id == 0)
                {
                    cofre.DataGeracao = DateTime.Now;
                    cofre.Caixa = null;
                    cofre.UsuarioCriacao = MainWindow.UsuarioLogado;
                    cofre.Situacao = SituacaoCofre.Depositado;
                    Repositorio.Salvar(cofre);
                }
                else
                {
                    cofre.DataAlteracao = DateTime.Now;
                    cofre.UsuarioAlteracao = MainWindow.UsuarioLogado;
                    Repositorio.Alterar(cofre);
                }
                Close();
            }

        }

        private void txtValorInicial_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = Regex.IsMatch(e.Text, @"[^0-9,-]+");
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
    }
}
