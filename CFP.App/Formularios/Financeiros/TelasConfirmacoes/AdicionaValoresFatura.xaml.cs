using CFP.Dominio.Dominio;
using CFP.Dominio.ObjetoValor;
using CFP.Repositorio.Repositorio;
using CFP.Servicos.Financeiro;
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
                txtData.SelectedDate = cartaoCreditoItens.DataCompra;
                cmbRefPessoa.SelectedItem = cartaoCreditoItens.Pessoa;
            }
        }
        #endregion

        #region Calculando novo valor da parcela referente ao cartao de credito
        public void CalculaItensAdicionadosFaturaParaConta(CartaoCreditoItens obj)
        {
            var contaPagamento = new ContaPagamento();
            contaPagamento = RepositorioContaPagamento.ObterPorParametros(x => x.Conta.FaturaCartaoCredito == obj.CartaoCredito).First();
            if (cartaoCreditoItens.Id > 0)
                contaPagamento.ValorParcela = obj.CartaoCredito.CartaoCreditos.Sum(x => x.Valor);
            else
                contaPagamento.ValorParcela += obj.Valor;
            contaPagamento.ValorReajustado = contaPagamento.ValorParcela;
            RepositorioContaPagamento.AlterarLote(contaPagamento);
            var conta = new Conta();
            conta = RepositorioConta.ObterPorId(contaPagamento.Conta.Id);
            //conta.Observacao += obj.Pessoa != null ? String.Format("{0},",obj.Pessoa.Nome): null;
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

        #region Salvar
        private void Salvar()
        {
            Decimal valorTotal = 0, valorParcela = 0, valorDiferenca = 0;
            Int32 qtdParcelas = 0;
            int mes = cartaoCredito.MesReferencia, ano = cartaoCredito.AnoReferencia;
            var descricao = string.Empty;

            var listaFaturas = new RepositorioCartaoCredito(Session).ObterTodos().Where(x => x.Cartao.Id == cartaoCredito.Cartao.Id &&  x.SituacaoFatura == SituacaoFatura.Aberta).ToList();

            if (cmbTipoCalculo.SelectedIndex == 0)
            {
                valorTotal = Decimal.Parse(txtValor.Text);
                qtdParcelas = Int32.Parse(txtQtd.Text);
                valorParcela = Math.Round(valorTotal / qtdParcelas, 2);
                valorDiferenca = Math.Round(valorTotal - valorParcela * qtdParcelas, 2);
            }
            else
            {
                qtdParcelas = Int32.Parse(txtQtd.Text);
                valorParcela = Decimal.Parse(txtValor.Text);
            }

            for (int i = 1; i <= qtdParcelas; i++)
            {
                if (i != 1)
                {
                    mes = mes == 12 ? 1 : mes + 1;
                    ano = mes == 1 ? ano + 1 : ano;
                    descricao = string.Format("{0} - {1:00}/{2}", cartaoCredito.Cartao, mes, ano);
                }
                else
                    descricao = cartaoCredito.DescricaoCompleta;
                   
                if (listaFaturas.Count != 0)
                {
                    foreach (var fatura in listaFaturas.OrderBy(x => x.DescricaoCompleta))
                    {
                        if (descricao != fatura.DescricaoCompleta)
                        {
                            CartaoCredito novaFaturaCartaoCredito = new CartaoCredito()
                            {
                                MesReferencia = mes,
                                AnoReferencia = ano,
                                Cartao = cartaoCredito.Cartao,
                                ValorFatura = 0,
                                SituacaoFatura = SituacaoFatura.Aberta,
                                DataGeracao = DateTime.Now,
                                UsuarioCriacao = MainWindow.UsuarioLogado
                            };
                            new RepositorioCartaoCredito(Session).SalvarLote(novaFaturaCartaoCredito);
                            ContaServicos.NovaContaRefCartaoCredito(MainWindow.UsuarioLogado, novaFaturaCartaoCredito, Session);

                            var cartaoCreditoItens = new CartaoCreditoItens()
                            {
                                Valor = !(i == qtdParcelas) ? valorParcela : valorParcela + valorDiferenca,
                                Nome = txtNome.Text,
                                NumeroParcelas = String.Format("{0}/{1}", i, qtdParcelas),
                                DataCompra = txtData.SelectedDate,
                                SubGrupoGasto = (SubGrupoGasto)cmbGrupo.SelectedItem,
                                Pessoa = (Pessoa)cmbRefPessoa.SelectedItem,
                                CartaoCredito = novaFaturaCartaoCredito,
                                UsuarioCriacao = MainWindow.UsuarioLogado,
                                DataGeracao = DateTime.Now
                            };
                            Repositorio.SalvarLote(cartaoCreditoItens);
                            CalculaItensAdicionadosFaturaParaConta(cartaoCreditoItens);
                        }
                        else
                        {
                            var cartaoCreditoItens = new CartaoCreditoItens()
                            {
                                Valor = valorParcela,
                                Nome = txtNome.Text,
                                NumeroParcelas = String.Format("{0}/{1}", i, qtdParcelas),
                                DataCompra = txtData.SelectedDate,
                                SubGrupoGasto = (SubGrupoGasto)cmbGrupo.SelectedItem,
                                Pessoa = (Pessoa)cmbRefPessoa.SelectedItem,
                                CartaoCredito = fatura,
                                UsuarioCriacao = MainWindow.UsuarioLogado,
                                DataGeracao = DateTime.Now
                            };
                            Repositorio.SalvarLote(cartaoCreditoItens);
                            CalculaItensAdicionadosFaturaParaConta(cartaoCreditoItens);
                        }
                        listaFaturas.Remove(fatura);
                        break;
                    }
                }
                else
                {
                    CartaoCredito novaFaturaCartaoCredito = new CartaoCredito()
                    {
                        MesReferencia = mes,
                        AnoReferencia = ano,
                        Cartao = cartaoCredito.Cartao,
                        ValorFatura = 0,
                        SituacaoFatura = SituacaoFatura.Aberta,
                        DataGeracao = DateTime.Now,
                        UsuarioCriacao = MainWindow.UsuarioLogado
                    };
                    new RepositorioCartaoCredito(Session).SalvarLote(novaFaturaCartaoCredito);
                    ContaServicos.NovaContaRefCartaoCredito(MainWindow.UsuarioLogado, novaFaturaCartaoCredito, Session);

                    var cartaoCreditoItens = new CartaoCreditoItens()
                    {
                        Valor = !(i == qtdParcelas) ? valorParcela : valorParcela + valorDiferenca,
                        Nome = txtNome.Text,
                        NumeroParcelas = String.Format("{0}/{1}", i, qtdParcelas),
                        DataCompra = txtData.SelectedDate,
                        SubGrupoGasto = (SubGrupoGasto)cmbGrupo.SelectedItem,
                        Pessoa = (Pessoa)cmbRefPessoa.SelectedItem,
                        CartaoCredito = novaFaturaCartaoCredito,
                        UsuarioCriacao = MainWindow.UsuarioLogado,
                        DataGeracao = DateTime.Now
                    };
                    Repositorio.SalvarLote(cartaoCreditoItens);
                    CalculaItensAdicionadosFaturaParaConta(cartaoCreditoItens);
                }

            }
        }
        #endregion

        #region Alterar
        private void Alterar()
        {
            if (PreencheObjeto())
            {
                cartaoCreditoItens.UsuarioAlteracao = MainWindow.UsuarioLogado;
                cartaoCreditoItens.DataAlteracao = DateTime.Now;
                Repositorio.AlterarLote(cartaoCreditoItens);
                CalculaItensAdicionadosFaturaParaConta(cartaoCreditoItens);
            }
        }
        #endregion

        #region Bloqueando campos para Edicao
        private void BloqueandoCamposParaEdicao()
        {
            txtQtd.IsEnabled = !txtQtd.IsEnabled;
            cmbTipoCalculo.IsEnabled = !cmbTipoCalculo.IsEnabled;
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
            e.Handled = Regex.IsMatch(e.Text, @"-[^0-9,]+");
        }

        private void txtValor_TextChanged(object sender, TextChangedEventArgs e)
        {
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
            {
                PreencheCampos();
                BloqueandoCamposParaEdicao();
            }
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
                    if (cartaoCreditoItens.Id == 0)
                        Salvar();
                    else
                        Alterar();
                    trans.Commit();
                    DialogResult = true;

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                    trans.Rollback();
                }
            }
        }

        private void cmbRefPessoa_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape || e.Key == Key.Delete)
                cmbRefPessoa.SelectedIndex = -1;
        }

        private void txtValor_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(txtValor.Text))
            {
                var valor = decimal.TryParse(txtValor.Text, out decimal valido);
                if (!valor)
                {
                    MessageBox.Show("Valor inválido! Por favor verifique.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtValor.Clear();
                    txtValor.SelectAll();
                }
            }
        }
    }
}
