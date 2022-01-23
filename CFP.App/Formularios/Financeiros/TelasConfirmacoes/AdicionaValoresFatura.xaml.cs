using CFP.Dominio.Dominio;
using CFP.Repositorio.Repositorio;
using Dominio.Dominio;
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
        CartaoCredito cartaoCredito;
        CartaoCreditoItens cartaoCreditoItens;

        #region Carrega combo
        private void CarregaCombo()
        {
            cmbGrupo.ItemsSource = new RepositorioSubGrupoGasto(Session)
           .ObterPorParametros(x => x.Situacao == Situacao.Ativo)
           .OrderBy(x => x.GrupoGasto.Nome).ThenBy(x => x.Nome)
           .ToList();
            cmbGrupo.SelectedIndex = 0;

            cmbRefPessoa.ItemsSource = new RepositorioPessoa(Session)
           .ObterPorParametros(x => x.Situacao == Situacao.Ativo)
           .OrderBy(x => x.Nome)
           .ToList();

        }
        #endregion

        #region Preenche Objeto
        private bool PreencheObjeto()
        {
            try
            {
                //tab Geral
                cartaoCreditoItens.SubGrupoGasto = (SubGrupoGasto)cmbGrupo.SelectedItem;
                cartaoCreditoItens.Nome = txtNome.Text;
                cartaoCreditoItens.Valor = Decimal.Parse(txtValor.Text);
                cartaoCreditoItens.NumeroParcelas = String.Format("{0}/1", txtQtd.Text);
                cartaoCreditoItens.DataCompra = txtData.SelectedDate;
                cartaoCreditoItens.Pessoa = (Pessoa)cmbRefPessoa.SelectedItem;
                cartaoCreditoItens.CartaoCredito = cartaoCredito;
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
                //return false;
            }
        }
        #endregion

        #region Repositorio
        private RepositorioCartaoCreditoItens _repositorio;
        public RepositorioCartaoCreditoItens Repositorio
        {
            get
            {
                if (_repositorio == null)
                    _repositorio = new RepositorioCartaoCreditoItens(Session);

                return _repositorio;
            }
            set { _repositorio = value; }
        }

        private RepositorioConta _repositorioConta;
        public RepositorioConta RepositorioConta
        {
            get
            {
                if (_repositorioConta == null)
                    _repositorioConta = new RepositorioConta(Session);

                return _repositorioConta;
            }
            set { _repositorioConta = value; }
        }

        private RepositorioContaPagamento _repositorioContaPagamento;
        public RepositorioContaPagamento RepositorioContaPagamento
        {
            get
            {
                if (_repositorioContaPagamento == null)
                    _repositorioContaPagamento = new RepositorioContaPagamento(Session);

                return _repositorioContaPagamento;
            }
            set { _repositorioContaPagamento = value; }
        }
        #endregion

        #region Preenche Campos
        private void PreencheCampos()
        {
            if (cartaoCreditoItens != null)
            {
                cmbGrupo.SelectedItem = cartaoCreditoItens.SubGrupoGasto;
                txtNome.Text = cartaoCreditoItens.Nome;
                txtValor.Text = cartaoCreditoItens.Valor.ToString("N2");
                //txtQtd.Text = cartaoCreditoItens.Qtd.ToString();
                txtData.SelectedDate = cartaoCreditoItens.DataCompra;
                cmbRefPessoa.SelectedItem = cartaoCreditoItens.Pessoa;
            }
        }
        #endregion

        #region Calculando novo valor da parcela referente ao cartao de credito
        public void CalculaItensAdicionadosFaturaParaConta()
        {
            var contaPagamento = new ContaPagamento();
            contaPagamento = RepositorioContaPagamento.ObterPorParametros(x => x.Conta.FaturaCartaoCredito == cartaoCredito).First();
            contaPagamento.ValorParcela = cartaoCredito.CartaoCreditos.Sum(x => x.Valor) + Decimal.Parse(txtValor.Text);
            contaPagamento.ValorReajustado = contaPagamento.ValorParcela;
            RepositorioContaPagamento.AlterarLote(contaPagamento);
            var conta = new Conta();
            conta = RepositorioConta.ObterPorId(contaPagamento.Conta.Id);
            conta.ValorTotal = contaPagamento.ValorParcela;
            RepositorioConta.AlterarLote(conta);
        }
        #endregion

        #region Verifica qtd de parcelas
        public void VerificaQtdParcelas()
        {
            if (txtQtd.Text.Equals("1"))
            {
                cmbTipoCalculo.IsEnabled = false;
                cmbTipoCalculo.SelectedIndex = -1;
            }
            else
            {
                cmbTipoCalculo.IsEnabled = true;
                cmbTipoCalculo.SelectedIndex = 0;
            }
        }
        #endregion

        public AdicionaValoresFatura(CartaoCreditoItens _cartaoCreditoItens, CartaoCredito _cartaoCredito, ISession _session)
        {
            InitializeComponent();
            Session = _session;
            cartaoCreditoItens = _cartaoCreditoItens;
            cartaoCredito = _cartaoCredito;
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
            if (cartaoCreditoItens.Id > 0)
                PreencheCampos();
            else
            {
                VerificaQtdParcelas();
            }
        }

        private void btCancelar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void txtQtd_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtQtd.Text) || txtQtd.Text.Equals("0"))
                txtQtd.Text = "1";
            VerificaQtdParcelas();
        }

        private void btConfirmar_Click(object sender, RoutedEventArgs e)
        {
            if (cmbGrupo.SelectedIndex == -1
                || string.IsNullOrEmpty(txtNome.Text)
                || string.IsNullOrEmpty(txtValor.Text)
                || txtData.SelectedDate == null)
            {
                MessageBox.Show("Os campos Grupo, Nome, Valor e Data da Compra são obrigatórios!", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using (var trans = Session.BeginTransaction())
            {
                try
                {
                    if (txtQtd.Text.Equals("1"))
                    {
                        if (PreencheObjeto())
                        {
                            if (cartaoCreditoItens.Id == 0)
                            {
                                cartaoCreditoItens.DataGeracao = DateTime.Now;
                                cartaoCreditoItens.UsuarioCriacao = MainWindow.UsuarioLogado;
                                Repositorio.SalvarLote(cartaoCreditoItens);
                                CalculaItensAdicionadosFaturaParaConta();
                            }
                            else
                            {
                                cartaoCreditoItens.DataAlteracao = DateTime.Now;
                                cartaoCreditoItens.UsuarioAlteracao = MainWindow.UsuarioLogado;
                                Repositorio.AlterarLote(cartaoCreditoItens);
                                CalculaItensAdicionadosFaturaParaConta();

                            }
                            trans.Commit();
                            DialogResult = true;
                        }
                    }
                    else
                    {
                        for (int i = 0; i < Int32.Parse(txtQtd.Text); i++)
                        {
                            if(cmbTipoCalculo.SelectedIndex == 0) //dividir o valor pela quantidade
                            {

                            }
                            else //replicar o valor pela quantidade
                            {

                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                    trans.Rollback();
                }
            }




        }
    }
}
