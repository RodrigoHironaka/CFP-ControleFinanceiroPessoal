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
        public Cofre cofre = new Cofre();
        Caixa caixa;

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
            cmbSituacao.ItemsSource = Enum.GetValues(typeof(EntradaSaida));
            

            cmbTransacaoBancaria.ItemsSource = new RepositorioFormaPagamento(Session)
              .ObterPorParametros(x => x.Situacao == Situacao.Ativo && x.TransacoesBancarias == SimNao.Sim)
              .OrderBy(x => x.Nome)
              .ToList();
          

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
                cofre.Nome = txtNome.Text;
                cofre.Banco = (Banco)cmbBanco.SelectedItem;
                cofre.TransacoesBancarias = (FormaPagamento)cmbTransacaoBancaria.SelectedItem;
                cofre.Situacao = (EntradaSaida)cmbSituacao.SelectedIndex;
                if (cofre.Situacao == EntradaSaida.Entrada)
                    cofre.Valor = Decimal.Parse(txtValor.Text) < 0 ? Decimal.Parse(txtValor.Text) * -1: Decimal.Parse(txtValor.Text);
                else
                    cofre.Valor = Decimal.Parse(txtValor.Text) < 0 ? Decimal.Parse(txtValor.Text) : Decimal.Parse(txtValor.Text) * -1;

                #region Verificando se valor é maior que o saldo final do Caixa quando for transferencia do caixa para o cofre
                if (caixa != null && !String.IsNullOrEmpty(txtValor.Text))
                {
                    if (cofre.Situacao == EntradaSaida.Entrada)
                    {
                        if (caixa.BalancoFinal >= Decimal.Parse(txtValor.Text))
                            cofre.Caixa = caixa;
                        else
                        {
                            MessageBox.Show("Valor digitado é maior que o saldo do caixa!", "Atencao", MessageBoxButton.OK, MessageBoxImage.Warning);
                            txtValor.Clear();
                            return false;
                        }
                    }
                    if (cofre.Situacao == EntradaSaida.Saída)
                    {
                        DateTime data = DateTime.Today;
                        var dataInicio = new DateTime(data.Year, data.Month, 1);
                        var dataFinal = new DateTime(data.Year, data.Month, DateTime.DaysInMonth(data.Year, data.Month));
                        var valor = Repositorio.ObterPorParametros(x => x.Banco == cmbBanco.SelectedItem && x.DataGeracao >= dataInicio && x.DataGeracao <= dataFinal).Sum(x => x.Valor);
                        if (valor >= Decimal.Parse(txtValor.Text))
                            cofre.Caixa = caixa;
                        else
                        {
                            MessageBox.Show("Valor digitado é maior que o saldo do cofre!", "Atencao", MessageBoxButton.OK, MessageBoxImage.Warning);
                            txtValor.Clear();
                            return false;
                        }
                    }
                }
                #endregion
                return true;
            }
            catch
            {
                return false;
            }

        }
        #endregion

        #region Preeenche Campos
        private void PreencheCampos()
        {
            txtValor.Text = cofre.Valor.ToString();
            txtNome.Text = cofre.Nome;
            cmbBanco.SelectedValue = cofre.Banco;
            cmbTransacaoBancaria.SelectedValue = cofre.TransacoesBancarias;
            cmbSituacao.SelectedValue = cofre.Situacao;
        }
        #endregion

        public ValoresCofre(ISession _session)
        {
            InitializeComponent();
            Session = _session;
            cmbBanco.SelectedIndex = 0;
            cmbTransacaoBancaria.SelectedIndex = 0;
            cmbSituacao.SelectedIndex = 0;
        }

        public ValoresCofre(Cofre _cofre, ISession _session)
        {
            InitializeComponent();
            Session = _session;
            cofre = _cofre;
            PreencheCampos();
        }

        public ValoresCofre(Caixa _caixa, ISession _session)
        {
            InitializeComponent();
            Session = _session;
            caixa = _caixa;
        }

        private void btConfirmar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtValor.Text) || string.IsNullOrEmpty(txtNome.Text) || cmbTransacaoBancaria.SelectedItem == null || cmbTransacaoBancaria.SelectedItem == null || cmbSituacao.SelectedIndex == -1)
            {
                MessageBox.Show("Todos os campos são Obrigatórios. Por favor verifique!", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (PreencheObjeto())
            {
                if (cofre.Id == 0)
                {
                    cofre.Codigo = Repositorio.RetornaUltimoCodigo() + 1;
                    cofre.DataGeracao = DateTime.Now;
                    cofre.UsuarioCriacao = MainWindow.UsuarioLogado;
                    Repositorio.Salvar(cofre);
                }
                else
                {
                    cofre.DataAlteracao = DateTime.Now;
                    cofre.UsuarioAlteracao = MainWindow.UsuarioLogado;
                    Repositorio.Alterar(cofre);
                }
                this.DialogResult = true;
            }
        }

        private void btCancelar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CarregaCombo();
        }

        private void txtValor_LostFocus(object sender, RoutedEventArgs e)
        {
            //Decimal valido ;
            //var valor = decimal.TryParse(txtValor.Text, out valido);
            //if (!valor)
            //{
            //    MessageBox.Show("Valor colado é inválido! Por favor verifique.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
            //    txtValor.Focus();
            //    txtValor.SelectAll();
            //}
                
        }

        private void txtValor_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = Regex.IsMatch(e.Text, @"[^0-9,]+");
        }

        private void txtValor_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void txtValor_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!String.IsNullOrEmpty(txtValor.Text))
            {
                Decimal valido;
                var valor = decimal.TryParse(txtValor.Text, out valido);
                if (!valor)
                {
                    MessageBox.Show("Valor colado é inválido! Por favor verifique.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtValor.Clear();
                    txtValor.Focus();
                }
            }
           
        }
    }
}
