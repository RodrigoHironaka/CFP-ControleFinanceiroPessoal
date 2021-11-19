using CFP.App.Formularios.Financeiros.TelasConfirmacoes;
using CFP.App.Formularios.Pesquisas;
using CFP.Dominio.Dominio;
using CFP.Dominio.ObjetoValor;
using CFP.Repositorio.Repositorio;
using Dominio.Dominio;
using Dominio.ObejtoValor;
using Dominio.ObjetoValor;
using LinqKit;
using Microsoft.Win32;
using NHibernate;
using Repositorio.Repositorios;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CFP.App.Formularios.Financeiros
{
    /// <summary>
    /// Interação lógica para UserControlContas.xam
    /// </summary>
    public partial class UserControlContas : UserControl
    {
        #region EXEMPLOS DE CODIGOS NAO APAGAR   
        //ContaPagamento linhaContaPagamento = (ContaPagamento)DataGridContaPagamento.SelectedItem;
        //if (linhaContaPagamento.SituacaoParcelas == SituacaoParcela.Pendente || linhaContaPagamento.SituacaoParcelas == SituacaoParcela.Parcial)
        //{
        //    ConfirmacaoPagamentoParcela janela = new ConfirmacaoPagamentoParcela(linhaContaPagamento, Session);
        //    janela.ShowDialog();
        //    DataGridContaPagamento.Items.Refresh();
        //}
        //else
        //    MessageBox.Show("Situacao da parcela está " + linhaContaPagamento.SituacaoParcelas + "!", "Mensagem", MessageBoxButton.OK, MessageBoxImage.Information);
        #endregion

        //--------------------------------------------------------------------------------------------------------------------------------

        ISession Session;
        Conta conta;
        FluxoCaixa fluxoCaixa;
        Caixa caixa;

        #region Carrega Combos
        private void CarregaCombos()
        {
            lblSituacao.Text = SituacaoConta.Aberto.ToString();
            cmbTipoConta.ItemsSource = Enum.GetValues(typeof(TipoConta));
            cmbTipoConta.SelectedIndex = 0;
            cmbTipoPeriodo.ItemsSource = Enum.GetValues(typeof(TipoPeriodo));
            cmbTipoPeriodo.SelectedIndex = 0;

            cmbTipoSubGasto.ItemsSource = new RepositorioSubGrupoGasto(Session)
           .ObterPorParametros(x => x.Situacao == Situacao.Ativo)
           .OrderBy(x => x.Nome)
           .ToList();
            cmbTipoSubGasto.SelectedIndex = 0;

            cmbFormaCompra.ItemsSource = new RepositorioFormaPagamento(Session)
               .ObterPorParametros(x => x.Situacao == Situacao.Ativo && x.QtdParcelas > 0)
               .OrderBy(x => x.Nome)
               .ToList<FormaPagamento>();
            cmbFormaCompra.SelectedIndex = 0;

            DatagridCmbFormaPagamento.ItemsSource = new RepositorioFormaPagamento(Session)
               .ObterPorParametros(x => x.Situacao == Situacao.Ativo)
               .OrderBy(x => x.Nome)
               .ToList<FormaPagamento>();

            cmbReferenciaPessoa.ItemsSource = new RepositorioPessoa(Session)
               .ObterPorParametros(x => x.Situacao == Situacao.Ativo)
               .OrderBy(x => x.Nome)
               .ToList<Pessoa>();
        }
        #endregion

        #region Repositorio
        private RepositorioConta _repositorio;
        public RepositorioConta Repositorio
        {
            get
            {
                if (_repositorio == null)
                    _repositorio = new RepositorioConta(Session);

                return _repositorio;
            }
            set { _repositorio = value; }
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

        private RepositorioContaArquivo _repositorioContaArquivo;
        public RepositorioContaArquivo RepositorioContaArquivo
        {
            get
            {
                if (_repositorioContaArquivo == null)
                    _repositorioContaArquivo = new RepositorioContaArquivo(Session);

                return _repositorioContaArquivo;
            }
            set { _repositorioContaArquivo = value; }
        }

        private RepositorioFluxoCaixa _repositorioFluxoCaixa;
        public RepositorioFluxoCaixa RepositorioFluxoCaixa
        {
            get
            {
                if (_repositorioFluxoCaixa == null)
                    _repositorioFluxoCaixa = new RepositorioFluxoCaixa(Session);

                return _repositorioFluxoCaixa;
            }
            set { _repositorioFluxoCaixa = value; }
        }

        private RepositorioCaixa _repositorioCaixa;
        public RepositorioCaixa RepositorioCaixa
        {
            get
            {
                if (_repositorioCaixa == null)
                    _repositorioCaixa = new RepositorioCaixa(Session);

                return _repositorioCaixa;
            }
            set { _repositorioCaixa = value; }
        }
        #endregion

        #region Controle de acessos Inicial e Cadastro
        private void ControleAcessoInicial()
        {
            //Bloqueando
            GridCampos.IsEnabled = !GridCampos.IsEnabled;
            btSalvar.IsEnabled = !btSalvar.IsEnabled;
            btExcluir.IsEnabled = !btExcluir.IsEnabled;
            btFinalizarConta.IsEnabled = !btFinalizarConta.IsEnabled;

            //Desbloqueando
            btPesquisar.IsEnabled = true;
            txtCodigo.IsEnabled = true;

            txtCodigo.Focus();
            txtCodigo.SelectAll();
        }

        private void ControleAcessoCadastro()
        {
            //Bloqueando
            btPesquisar.IsEnabled = !btPesquisar.IsEnabled;
            txtCodigo.IsEnabled = !txtCodigo.IsEnabled;

            //Desbloqueando
            GridCampos.IsEnabled = true;
            btSalvar.IsEnabled = true;
            btExcluir.IsEnabled = true;
            btFinalizarConta.IsEnabled = true;

            //define o foco no primeiro campo
            txtNome.Focus();
            txtNome.Select(txtNome.Text.Length, 0);

        }
        #endregion

        #region Selecão e Foco no campo Codigo 
        private void FocoNoCampoCodigo()
        {
            txtCodigo.SelectAll();
            txtCodigo.Focus();
        }
        #endregion

        #region Limpa os campos do Cadastro
        public void LimpaCampos()
        {
            lblSituacao.Text = SituacaoConta.Aberto.ToString();
            btSalvar.Visibility = Visibility.Visible;
            btFinalizarConta.Visibility = Visibility.Visible;
            btExcluir.Visibility = Visibility.Visible;
            lblTotalPagos.Content = "R$ 0,00";
            lblTotalPendentes.Content = "R$ 0,00";
            lblTotalParceiais.Content = "R$ 0,00";
            lblTotalCancelados.Content = "R$ 0,00";

            foreach (var item in GridControls.Children)
            {
                if (item is TabControl)
                {
                    tabItemGeral.IsSelected = true;
                    foreach (var gridControls2 in GridControls2.Children)
                    {
                        if (gridControls2 is TextBox)
                            (gridControls2 as TextBox).Text = string.Empty;
                        if (gridControls2 is ComboBox)
                            (gridControls2 as ComboBox).SelectedIndex = 0;
                        if (gridControls2 is CheckBox)
                            (gridControls2 as CheckBox).IsChecked = false;
                        if (gridControls2 is RadioButton)
                            (gridControls2 as RadioButton).IsChecked = false;
                        if (gridControls2 is DatePicker)
                            (gridControls2 as DatePicker).Text = string.Empty;

                        if (gridControls2 is Grid)
                        {
                            foreach (var gridRow1 in GridRow1.Children)
                            {
                                if (gridRow1 is TextBox)
                                    (gridRow1 as TextBox).Text = string.Empty;
                                if (gridRow1 is ComboBox)
                                    (gridRow1 as ComboBox).SelectedIndex = 0;

                            }
                            foreach (var gridRow2 in GridRow2.Children)
                            {
                                if (gridRow2 is TextBox)
                                    (gridRow2 as TextBox).Text = string.Empty;
                                if (gridRow2 is ComboBox)
                                    (gridRow2 as ComboBox).SelectedIndex = 0;
                            }
                            foreach (var gridRow3 in GridRow3.Children)
                            {
                                if (gridRow3 is TextBox)
                                    (gridRow3 as TextBox).Text = string.Empty;
                                if (gridRow3 is ComboBox)
                                    (gridRow3 as ComboBox).SelectedIndex = 0;
                                if (gridRow3 is DatePicker)
                                    (gridRow3 as DatePicker).Text = string.Empty;
                            }
                            foreach (var gridRow4 in GridRow4.Children)
                            {
                                if (gridRow4 is TextBox)
                                    (gridRow4 as TextBox).Text = string.Empty;
                                if (gridRow4 is ComboBox)
                                    (gridRow4 as ComboBox).SelectedIndex = 0;

                            }
                            foreach (var gridRow5 in GridRow5.Children)
                            {
                                if (gridRow5 is TextBox)
                                    (gridRow5 as TextBox).Text = string.Empty;
                                if (gridRow5 is ComboBox)
                                    (gridRow5 as ComboBox).SelectedIndex = 0;
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #region Preenche Objeto para Salvar
        private bool PreencheObjeto()
        {
            try
            {
                //tab Geral
                conta.UsuarioCriacao = MainWindow.UsuarioLogado;
                conta.Nome = txtNome.Text;
                conta.TipoConta = (TipoConta)cmbTipoConta.SelectedIndex;
                conta.SubGrupoGasto = cmbTipoSubGasto.SelectedItem != null ? (SubGrupoGasto)cmbTipoSubGasto.SelectedItem : null;
                conta.TipoPeriodo = (TipoPeriodo)cmbTipoPeriodo.SelectedIndex;
                conta.Situacao = ((SituacaoConta)Enum.Parse(typeof(SituacaoConta), lblSituacao.Text));
                conta.DataEmissao = (DateTime)(txtEmissao.Text != string.Empty ? txtEmissao.SelectedDate : conta.DataEmissao.GetValueOrDefault());
                conta.Pessoa = (Pessoa)(cmbReferenciaPessoa.SelectedItem ?? null);
                conta.NumeroDocumento = txtNumDocumento.Text != string.Empty ? Int64.Parse(txtNumDocumento.Text) : 0;
                conta.FormaCompra = (FormaPagamento)cmbFormaCompra.SelectedItem;
                conta.Observacao = txtObservacao.Text;

                //tab Pagamento
                conta.ContaPagamentos = contaPagamento; //(IList<ContaPagamento>)DataGridContaPagamento.ItemsSource;

                //tab Arquivos
                conta.ContaArquivos = contaArquivos;
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
                //return false;
            }
        }

        private void PreencheObjetoComListaDadaGrid()
        {
            try
            {
                if (conta.ContaPagamentos.Count != 0)
                {
                    foreach (var item in conta.ContaPagamentos)
                    {
                        ContaPagamento contaPagamento = new ContaPagamento();
                        contaPagamento.Numero = item.Numero;
                        contaPagamento.ValorParcela = item.ValorParcela;
                        contaPagamento.DataVencimento = item.DataVencimento;
                        contaPagamento.DataPagamento = item.DataPagamento;
                        contaPagamento.JurosPorcentual = item.JurosPorcentual;
                        contaPagamento.JurosValor = item.JurosValor;
                        contaPagamento.DescontoPorcentual = item.DescontoPorcentual;
                        contaPagamento.DescontoValor = item.DescontoValor;
                        contaPagamento.ValorReajustado = item.ValorReajustado;
                        contaPagamento.ValorPago = item.ValorPago;
                        contaPagamento.SituacaoParcelas = item.SituacaoParcelas;
                        contaPagamento.FormaPagamento = item.FormaPagamento;
                        contaPagamento.Conta = item.Conta;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }

        }

        private void PreencheObjetoComListaArquivos()
        {
            try
            {
                if (conta.ContaArquivos.Count != 0)
                {
                    foreach (var item in conta.ContaArquivos)
                    {
                        ContaArquivo contaArquivo = new ContaArquivo();
                        contaArquivo.Caminho = item.Caminho;
                        contaArquivo.Conta = item.Conta;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }

        }
        #endregion

        #region Preenche campos no user control
        private void PreencheCampos()
        {
            if (conta != null)
            {
                txtCodigo.Text = conta.Codigo.ToString();
                txtNome.Text = conta.Nome.ToString();
                cmbTipoConta.SelectedIndex = conta.TipoConta.GetHashCode();
                cmbTipoSubGasto.SelectedItem = conta.SubGrupoGasto;
                cmbTipoPeriodo.SelectedIndex = conta.TipoPeriodo.GetHashCode();
                lblSituacao.Text = conta.Situacao.ToString();
                txtEmissao.Text = conta.DataEmissao != DateTime.MinValue ? conta.DataEmissao.ToString() : string.Empty;

                cmbReferenciaPessoa.SelectedValue = conta.Pessoa;
                txtNumDocumento.Text = conta.NumeroDocumento > 0 ? conta.NumeroDocumento.ToString() : string.Empty;
                cmbFormaCompra.SelectedItem = conta.FormaCompra;
                txtObservacao.Text = conta.Observacao;

                #region Lista Pagamentos
                var listaContaPagamentos = conta.ContaPagamentos;
                foreach (var item in listaContaPagamentos)
                {
                    contaPagamento.Add(item);
                }
                #endregion
                #region Lista Arquivos
                var listaContaArquivos = conta.ContaArquivos;
                foreach (var arquivo in listaContaArquivos)
                {
                    contaArquivos.Add(arquivo);
                }
                #endregion
            }
        }
        #endregion

        #region Definindo Cor Padrão do botão Pesquisar #FF1F3D68 
        public void CorPadrãoBotaoPesquisar()
        {
            var converter = new System.Windows.Media.BrushConverter();
            var HexaToBrush = (Brush)converter.ConvertFromString("#FF1F3D68");
            btPesquisar.Background = HexaToBrush;
        }
        #endregion

        #region Verifcando situações das parcelas
        private bool VerificaSituacaoParcelas()
        {
            if (conta.ContaPagamentos != null)
            {
                foreach (var item in conta.ContaPagamentos)
                {
                    if (item.SituacaoParcelas != SituacaoParcela.Pendente)
                        return false;
                }
            }
            return true;
        }

        private bool VerificaParcelasPendentes()
        {
            if (conta.ContaPagamentos != null)
            {
                foreach (var item in conta.ContaPagamentos)
                {
                    if (item.SituacaoParcelas == SituacaoParcela.Pendente || item.SituacaoParcelas == SituacaoParcela.Parcial)
                        return true;
                }
            }
            return false;
        }
        #endregion

        #region Metodo para salvar
        private bool Salvar()
        {
            if (String.IsNullOrEmpty(txtNome.Text) || String.IsNullOrEmpty(txtEmissao.Text))
            {
                MessageBox.Show("Os campos nome e data de emissão são obrigatórios, por favor verifique!", "Atenção", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return false;
            }

            if (PreencheObjeto())
            {
                if ((conta.Id == 0) && (String.IsNullOrEmpty(txtCodigo.Text)))
                {
                    conta.DataGeracao = DateTime.Now;
                    conta.Codigo = Repositorio.RetornaUltimoCodigo() + 1;
                    conta.ContaPagamentos.ToList().ForEach(x => x.Conta = conta);
                    conta.ContaArquivos.ToList().ForEach(x => x.Conta = conta);
                    Repositorio.Salvar(conta);
                    PreencheObjetoComListaDadaGrid();
                    PreencheObjetoComListaArquivos();
                    txtCodigo.Text = conta.Codigo.ToString();
                    return true;
                }
                else
                {
                    #region Conta Pagamento
                    List<ContaPagamento> novoContaPagamentos = new List<ContaPagamento>(conta.ContaPagamentos.Count);
                    foreach (ContaPagamento item in contaPagamento)
                        novoContaPagamentos.Add(item);

                    conta.ContaPagamentos.Clear();

                    foreach (var item in novoContaPagamentos)
                    {
                        if (item.Conta == null)
                            item.Conta = this.conta;
                        conta.ContaPagamentos.Add(item);
                    }
                    #endregion

                    #region Conta Arquivos
                    List<ContaArquivo> novoContaArquivos = new List<ContaArquivo>(conta.ContaArquivos.Count);
                    foreach (var item in contaArquivos)
                        novoContaArquivos.Add(item);

                    conta.ContaArquivos.Clear();

                    foreach (var item in novoContaArquivos)
                    {
                        if (item.Conta == null)
                            item.Conta = this.conta;
                        conta.ContaArquivos.Add(item);
                    }
                    #endregion

                    conta.DataAlteracao = DateTime.Now;
                    Repositorio.Alterar(conta);
                    DataGridContaPagamento.Items.Refresh();
                    lstArquivos.Items.Refresh();
                    return true;
                }
            }

            return false;
        }

        private void SalvarFluxo(List<ContaPagamento> dados)
        {
            if (VerificaCaixa())
            {
                foreach (var item in dados.OrderBy(x => x.Numero))
                {
                    if (item.SituacaoParcelas == SituacaoParcela.Pago)
                    {
                        fluxoCaixa = new FluxoCaixa();
                        if (item.Conta.TipoConta == TipoConta.Pagar)
                        {
                            fluxoCaixa.TipoFluxo = EntradaSaida.Saída;
                            fluxoCaixa.Nome = String.Format("Pagamento parcela número {0} - Conta: {1}", item.Numero, conta.Codigo);
                            fluxoCaixa.Valor = item.ValorPago * -1;
                        }
                        else
                        {
                            fluxoCaixa.TipoFluxo = EntradaSaida.Entrada;
                            fluxoCaixa.Nome = String.Format("Recebimento parcela número {0} - Conta: {1}", item.Numero, conta.Codigo);
                            fluxoCaixa.Valor = item.ValorPago;
                        }
                        fluxoCaixa.DataGeracao = DateTime.Now;
                        fluxoCaixa.Conta = item.Conta;
                        fluxoCaixa.UsuarioLogado = MainWindow.UsuarioLogado;
                        fluxoCaixa.Caixa = caixa;
                        fluxoCaixa.FormaPagamento = item.FormaPagamento;
                        RepositorioFluxoCaixa.Salvar(fluxoCaixa);
                    }
                }
            }
        }
        #endregion

        #region Remove todos os itens da lista Conta Pagamento e Conta Arquivo e do Banco
        private void RemoveTodosOsItensDaListaEBanco()
        {
            if (conta.ContaPagamentos != null)
            {
                var lista = new List<ContaPagamento>(contaPagamento);
                foreach (var item in lista)
                {
                    contaPagamento.Remove(item);
                    conta.ContaPagamentos.Remove(item);
                    RepositorioContaPagamento.Excluir(item);
                }
            }
        }

        private void RemoveTodosOsItensDaListaEBancoArquivos()
        {
            if (conta.ContaArquivos != null)
            {
                var lista = new List<ContaArquivo>(contaArquivos);
                foreach (var item in lista)
                {
                    contaArquivos.Remove(item);
                    conta.ContaArquivos.Remove(item);
                    RepositorioContaArquivo.Excluir(item);
                    //preciso remover da pasta
                }
            }
        }
        #endregion

        //#region Gerar Parcelas
        //private void GerarParcelas(string vTotal, string qtd, DateTime primeiroVencimento)
        //{
        //    if (VerificaSituacaoParcelas())
        //    {
        //        RemoveTodosOsItensDaListaEBanco();
        //        #region Gerando as Parcelas
        //        Decimal valorTotal = txtValorTotal.Text != string.Empty ? Decimal.Parse(vTotal) : 0;
        //        Int32 qtdParcelas = txtQtdParcelas.Text != string.Empty ? Int32.Parse(qtd) : 1;
        //        DateTime dataPrimeiroVencimento = primeiroVencimento;
        //        if (!valorTotal.Equals(0) || !qtdParcelas.Equals(0))
        //        {

        //            Decimal valorParcela = Math.Round(valorTotal / qtdParcelas, 2);
        //            Decimal valorDiferenca = valorTotal - valorParcela * qtdParcelas;

        //            PreencheDataGrid();
        //            for (int i = 0; i < qtdParcelas; i++)
        //            {
        //                String numeroParcela = (i + 1).ToString().PadLeft(2, '0');
        //                String valorParcelaCorrigido = !(i + 1 == qtdParcelas) ? valorParcela.ToString() : (valorParcela + valorDiferenca).ToString();
        //                String dataVencimento = dataPrimeiroVencimento.AddMonths(i).ToShortDateString();

        //                contaPagamento.Add(new ContaPagamento()
        //                {
        //                    SituacaoParcelas = SituacaoParcela.Pendente,
        //                    Numero = Int32.Parse(numeroParcela),
        //                    ValorParcela = Decimal.Parse(valorParcelaCorrigido),
        //                    DataVencimento = Convert.ToDateTime(dataVencimento)
        //                });
        //                DataGridContaPagamento.Items.Refresh();
        //            }
        //            DataGridContaPagamento.Items.Refresh();
        //        }
        //        #endregion 
        //    }
        //    else
        //    {
        //        MessageBoxResult d = MessageBox.Show("Existem parcelas modificadas! Deseja REFAZER mesmo assim? ", " Atenção ", MessageBoxButton.YesNo, MessageBoxImage.Question);
        //        if (d == MessageBoxResult.Yes)
        //        {
        //            RemoveTodosOsItensDaListaEBanco();
        //            #region Gerando as Parcelas
        //            Decimal valorTotal = txtValorTotal.Text != string.Empty ? Decimal.Parse(vTotal) : 0;
        //            Int32 qtdParcelas = txtQtdParcelas.Text != string.Empty ? Int32.Parse(qtd) : 1;
        //            DateTime dataPrimeiroVencimento = primeiroVencimento;
        //            if (!valorTotal.Equals(0) || !qtdParcelas.Equals(0))
        //            {

        //                Decimal valorParcela = Math.Round(valorTotal / qtdParcelas, 2);
        //                Decimal valorDiferenca = valorTotal - valorParcela * qtdParcelas;

        //                PreencheDataGrid();
        //                for (int i = 0; i < qtdParcelas; i++)
        //                {
        //                    String numeroParcela = (i + 1).ToString().PadLeft(2, '0');
        //                    String valorParcelaCorrigido = !(i + 1 == qtdParcelas) ? valorParcela.ToString() : (valorParcela + valorDiferenca).ToString();
        //                    String dataVencimento = dataPrimeiroVencimento.AddMonths(i).ToShortDateString();

        //                    contaPagamento.Add(new ContaPagamento()
        //                    {
        //                        SituacaoParcelas = SituacaoParcela.Pendente,
        //                        Numero = Int32.Parse(numeroParcela),
        //                        ValorParcela = Decimal.Parse(valorParcelaCorrigido),
        //                        DataVencimento = Convert.ToDateTime(dataVencimento)
        //                    });
        //                    DataGridContaPagamento.Items.Refresh();
        //                }
        //            }
        //            #endregion
        //        }
        //    }
        //}
        //#endregion

        #region PreencheDataGrid
        private ObservableCollection<ContaPagamento> contaPagamento;
        public void PreencheDataGrid()
        {
            contaPagamento = new ObservableCollection<ContaPagamento>();
            DataGridContaPagamento.ItemsSource = contaPagamento;
        }
        #endregion

        #region PreencheListView
        private ObservableCollection<ContaArquivo> contaArquivos;
        public void PreencheListView()
        {
            contaArquivos = new ObservableCollection<ContaArquivo>();
            lstArquivos.ItemsSource = contaArquivos;
        }
        #endregion

        #region Iniciando o Form Cancelado
        private void VerificandoSituacaoConta()
        {
            switch (lblSituacao.Text)
            {
                case "Aberto":
                    GridControls2.IsEnabled = true;
                    GridItemPagamento.IsEnabled = true;
                    GridArquivos.IsEnabled = true;
                    btSalvar.Visibility = Visibility.Visible;
                    btFinalizarConta.Visibility = Visibility.Visible;
                    break;
                case "Cancelado":
                    GridControls2.IsEnabled = false;
                    GridItemPagamento.IsEnabled = false;
                    GridArquivos.IsEnabled = false;
                    btSalvar.Visibility = Visibility.Hidden;
                    btFinalizarConta.Visibility = Visibility.Hidden;
                    btExcluir.Visibility = Visibility.Hidden;
                    break;
                case "Finalizado":
                    GridControls2.IsEnabled = false;
                    GridItemPagamento.IsEnabled = false;
                    GridArquivos.IsEnabled = false;
                    btSalvar.Visibility = Visibility.Hidden;
                    btFinalizarConta.Visibility = Visibility.Hidden;
                    break;
            }
        }
        #endregion

        #region Filtro Situação Parcelas
        private void FiltroSituacaoParcelas()
        {
            IList<SituacaoParcela> situacaoParcela = new List<SituacaoParcela>();

            var predicado = RepositorioContaPagamento.CriarPredicado();
            predicado = predicado.And(x => x.Conta.Id == conta.Id);

            if ((bool)chkPagos.IsChecked)
                situacaoParcela.Add(SituacaoParcela.Pago);
            else
                situacaoParcela.Remove(SituacaoParcela.Pago);
            if ((bool)chkPendentes.IsChecked)
                situacaoParcela.Add(SituacaoParcela.Pendente);
            else
                situacaoParcela.Remove(SituacaoParcela.Pendente);
            if ((bool)chkParciais.IsChecked)
                situacaoParcela.Add(SituacaoParcela.Parcial);
            else
                situacaoParcela.Remove(SituacaoParcela.Parcial);
            if ((bool)chkCancelados.IsChecked)
                situacaoParcela.Add(SituacaoParcela.Cancelado);
            else
                situacaoParcela.Remove(SituacaoParcela.Cancelado);

            predicado = predicado.And(x => situacaoParcela.Contains(x.SituacaoParcelas));
            DataGridContaPagamento.ItemsSource = RepositorioContaPagamento.ObterPorParametros(predicado);
        }
        #endregion

        #region Calculo total Por situacao das parcelas
        public void CalculoTotalPorSituacaoParcela()
        {
            lblTotalPagos.Content = String.Format("R$ {0}", conta.ContaPagamentos.Where(x => x.SituacaoParcelas == SituacaoParcela.Pago).Sum(x => x.ValorPago).ToString("N2"));
            lblTotalPendentes.Content = String.Format("R$ {0}", conta.ContaPagamentos.Where(x => x.SituacaoParcelas == SituacaoParcela.Pendente).Sum(x => x.ValorParcela).ToString("N2"));
            lblTotalParceiais.Content = String.Format("R$ {0}", conta.ContaPagamentos.Where(x => x.SituacaoParcelas == SituacaoParcela.Parcial).Sum(x => x.ValorParcela).ToString("N2"));
            lblTotalCancelados.Content = String.Format("R$ {0}", conta.ContaPagamentos.Where(x => x.SituacaoParcelas == SituacaoParcela.Cancelado).Sum(x => x.ValorParcela).ToString("N2"));
        }
        #endregion

        #region Gerando novo caminho para arquivos de acordo com o Id da conta
        public string NovoCaminho()
        {
            Configuracao configuracao = new RepositorioConfiguracao(Session).ObterTodos().FirstOrDefault();
            if (configuracao == null || configuracao.CaminhoArquivos == string.Empty)
            {
                return string.Empty;
            }
            else
            {
                if (conta.Id == 0)
                {
                    var novoCodigo = Repositorio.RetornaUltimoCodigo() + 1;
                    var novoDiretorio = String.Format("{0}\\Conta_{1}", configuracao.CaminhoArquivos, novoCodigo);
                    if (!Directory.Exists(novoDiretorio))
                        Directory.CreateDirectory(novoDiretorio);
                    return novoDiretorio;
                }
                else
                {
                    var diretorio = String.Format("{0}\\Conta_{1}", configuracao.CaminhoArquivos, conta.Codigo);
                    if (!Directory.Exists(diretorio))
                        Directory.CreateDirectory(diretorio);
                    return diretorio;
                }

            }
        }

        #endregion

        #region Verificando se caixa esta aberto
        private bool VerificaCaixa()
        {
            caixa = (Caixa)RepositorioCaixa.ObterPorParametros(x => x.Situacao == SituacaoCaixa.Aberto && x.UsuarioAbertura == MainWindow.UsuarioLogado).FirstOrDefault();
            if (caixa == null || caixa.Situacao == SituacaoCaixa.Fechado)
            {
                MessageBoxResult avisoCaixa = MessageBox.Show("Caixa esta fechado! Deseja abrir?", "Pergunta", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (avisoCaixa == MessageBoxResult.Yes)
                {
                    caixa = new Caixa();
                    MessageBoxResult colocarValor = MessageBox.Show("Deseja digitar um valor inicial?", "Pergunta", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (colocarValor == MessageBoxResult.Yes)
                    {
                        ConfirmaValorInicialCaixa janela = new ConfirmaValorInicialCaixa();
                        bool? res = janela.ShowDialog();
                        if ((bool)res)
                            caixa.ValorInicial = Decimal.Parse(janela.valorDigitado);
                        else
                            caixa.ValorInicial = 0;
                    }
                    else
                        caixa.ValorInicial = 0;

                    caixa.Codigo = Repositorio.RetornaUltimoCodigo() + 1;
                    caixa.DataAbertura = DateTime.Now;
                    caixa.UsuarioAbertura = MainWindow.UsuarioLogado;
                    RepositorioCaixa.Salvar(caixa);
                    return true;
                }
                return false;
            }
            return true;
        }
        #endregion

        public UserControlContas(Conta _conta, ISession _session)
        {
            InitializeComponent();
            Session = _session;
            conta = _conta;
        }

        private void btCancelar_Click(object sender, RoutedEventArgs e)
        {
            if (GridCampos.IsEnabled)
            {
                ControleAcessoInicial();
                FocoNoCampoCodigo();
                LimpaCampos();
                cmbReferenciaPessoa.SelectedIndex = -1;
            }
            else
            {
                (this.Parent as StackPanel).Children.Remove(this);
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            ControleAcessoInicial();
            CarregaCombos();
            PreencheDataGrid();
        }

        private void txtCodigo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                try
                {
                    // verifico se campo codigo esta preenchido para incluir novos dados ou carregar um existente
                    if (String.IsNullOrEmpty(txtCodigo.Text))
                    {
                        #region Nova Conta
                        conta = new Conta();
                        LimpaCampos();
                        PreencheDataGrid();
                        PreencheListView();
                        cmbReferenciaPessoa.SelectedIndex = -1;
                        ControleAcessoCadastro();
                        CorPadrãoBotaoPesquisar();
                        VerificandoSituacaoConta();
                        #endregion
                    }
                    else
                    {
                        #region Conta existente
                        conta = Repositorio.ObterPorCodigo(Int64.Parse(txtCodigo.Text));
                        if (conta != null)
                        {
                            PreencheDataGrid();
                            PreencheListView();
                            PreencheCampos();
                            ControleAcessoCadastro();
                            CorPadrãoBotaoPesquisar();
                            VerificandoSituacaoConta();
                            FiltroSituacaoParcelas();
                            CalculoTotalPorSituacaoParcela();
                        }
                        else
                        {
                            txtCodigo.SelectAll();
                            txtCodigo.Focus();
                            btPesquisar.Background = Brushes.DarkRed;
                        }
                        #endregion
                    }
                }
                catch (Exception)
                {
                    conta = null;
                }
            }
        }

        private void txtCodigo_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = Regex.IsMatch(e.Text, "[^0-9]+");
        }

        private void txtCodigo_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }

        private void btPesquisar_Click(object sender, RoutedEventArgs e)
        {
            PesquisaContas p = new PesquisaContas();
            p.ShowDialog();
            if (p.objeto != null)
            {
                conta = Repositorio.ObterPorCodigo(p.objeto.Codigo);
                PreencheDataGrid();
                PreencheListView();
                PreencheCampos();
                ControleAcessoCadastro();
                CorPadrãoBotaoPesquisar();
                VerificandoSituacaoConta();
                FiltroSituacaoParcelas();
                CalculoTotalPorSituacaoParcela();
            }
        }

        private void btSalvar_Click(object sender, RoutedEventArgs e)
        {
            //if ((TipoPeriodo)cmbTipoPeriodo.SelectedIndex == TipoPeriodo.Unica && DataGridContaPagamento.ItemsSource == null)
            //{
            //    GerarParcelas(txtValorTotal.Text, "1", txtPrimeiroVencimento.SelectedDate.Value);
            //}
            if (Salvar())
            {
                tabItemGeral.IsSelected = true;
                ControleAcessoInicial();
                FocoNoCampoCodigo();
                CalculoTotalPorSituacaoParcela();
            }
        }

        private void btExcluir_Click(object sender, RoutedEventArgs e)
        {
            if (VerificaSituacaoParcelas())
            {
                if (conta != null)
                {
                    MessageBoxResult d = MessageBox.Show(" Deseja realmente excluir o registro: " + conta.Nome + " ? ", " Atenção ", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (d == MessageBoxResult.Yes)
                    {
                        Repositorio.Excluir(conta);
                        LimpaCampos();
                        ControleAcessoInicial();
                    }
                }
            }
            else
            {
                if (conta != null)
                {
                    MessageBoxResult d = MessageBox.Show("Essa Conta não pode ser excluída!!! Deseja cancelar?!", " Atenção ", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (d == MessageBoxResult.Yes)
                    {
                        conta.Situacao = SituacaoConta.Cancelado;
                        Repositorio.Alterar(conta);
                        LimpaCampos();
                        ControleAcessoInicial();
                    }
                }
            }
        }

        //private void txtQtdParcelas_PreviewTextInput(object sender, TextCompositionEventArgs e)
        //{
        //    e.Handled = Regex.IsMatch(e.Text, "[^0-9]+");
        //}

        //private void txtQtdParcelas_PreviewKeyDown(object sender, KeyEventArgs e)
        //{
        //    if (e.Key == Key.Space)
        //        e.Handled = true;
        //}

        //private void txtValorTotal_PreviewTextInput(object sender, TextCompositionEventArgs e)
        //{
        //    e.Handled = Regex.IsMatch(e.Text, @"[^0-9,-]+");
        //}

        private void txtValorParcela_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = Regex.IsMatch(e.Text, @"[^0-9,-]+");
        }

        private void txtValorParcela_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }

        //private void txtValorTotal_PreviewKeyDown(object sender, KeyEventArgs e)
        //{
        //    if (e.Key == Key.Space)
        //        e.Handled = true;
        //}

        //private void txtPrimeiroVencimento_LostFocus(object sender, RoutedEventArgs e)
        //{
        //    if (txtPrimeiroVencimento.SelectedDate < txtEmissao.SelectedDate)
        //    {
        //        MessageBox.Show("Data de vencimento não pode ser menor que data de emissão!");
        //        return;
        //    }
        //}

        //private void btGerarParcelas_Click(object sender, RoutedEventArgs e)
        //{
        //    if (ValidaCampos())
        //    {
        //        GerarParcelas(txtValorTotal.Text, txtQtdParcelas.Text, txtPrimeiroVencimento.SelectedDate.Value);
        //        Salvar();
        //    }
        //}



        private void btEditar_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridContaPagamento.SelectedItem != null)
            {
                if (lblSituacao.Text != SituacaoConta.Cancelado.ToString() || lblSituacao.Text != SituacaoConta.Finalizado.ToString())
                {
                    IList<ContaPagamento> linhasContaPagamento = new List<ContaPagamento>();
                    foreach (var selecao in DataGridContaPagamento.SelectedItems)
                    {
                        linhasContaPagamento.Add((ContaPagamento)selecao);
                    }
                    foreach (var linha in linhasContaPagamento)
                    {
                        if (linha.SituacaoParcelas != SituacaoParcela.Pendente && linha.SituacaoParcelas != SituacaoParcela.Parcial)
                        {
                            MessageBox.Show("Situacao da parcela numero " + linha.Numero + " - " + linha.SituacaoParcelas + "!", "Mensagem", MessageBoxButton.OK, MessageBoxImage.Information);
                            return;
                        }
                    }

                    ConfirmacaoPagamentoParcela janela = new ConfirmacaoPagamentoParcela(linhasContaPagamento, Session);
                    bool? res = janela.ShowDialog();
                    if ((bool)res)
                    {
                        foreach (ContaPagamento parcelaAtualizada in linhasContaPagamento)
                        {
                            var ParcelaAntiga = contaPagamento.FirstOrDefault(x => x.ID == parcelaAtualizada.ID);
                            if (ParcelaAntiga != null)
                            {
                                contaPagamento.Remove(ParcelaAntiga);
                                contaPagamento.Add(parcelaAtualizada);
                            }
                            else
                            {
                                parcelaAtualizada.Numero++;
                                contaPagamento.Add(parcelaAtualizada);
                            }

                        }

                        if (Salvar())
                            SalvarFluxo(linhasContaPagamento.ToList());
                    }
                    DataGridContaPagamento.Items.Refresh();
                    CalculoTotalPorSituacaoParcela();
                    FiltroSituacaoParcelas();
                }
            }
            else
                MessageBox.Show("Selecione uma ou mais Parcelas!", "Mensagem", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void btCancelarParcela_Click(object sender, RoutedEventArgs e)
        {
            ContaPagamento selecao = (ContaPagamento)DataGridContaPagamento.SelectedItem;
            if ((selecao != null) && (selecao.SituacaoParcelas == SituacaoParcela.Pendente || selecao.SituacaoParcelas == SituacaoParcela.Parcial))
            {
                MessageBoxResult d = MessageBox.Show("Deseja cancelar esta Parcela?", " Atenção ", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (d == MessageBoxResult.Yes)
                {
                    foreach (var item in conta.ContaPagamentos)
                    {
                        if (item == selecao)
                        {
                            item.SituacaoParcelas = SituacaoParcela.Cancelado;
                            RepositorioContaPagamento.Alterar(item);
                            break;
                        }
                    }
                }
                DataGridContaPagamento.Items.Refresh();
                FiltroSituacaoParcelas();
                CalculoTotalPorSituacaoParcela();
            }
            else
                MessageBox.Show("Voce não pode cancelar esta Parcela!", " Informacão ", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void DataGridContaPagamento_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            //Neste exemplo pego o valor de uma coluna
            //if (DataGridContaPagamento.SelectedItem != null)
            //{
            //    ContaPagamento id = (ContaPagamento)DataGridContaPagamento.SelectedItem;
            //    MessageBox.Show(id.ValorParcela.ToString());

            //}

        }

        private void DataGridContaPagamento_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            btEditar_Click(sender, e);
        }

        private void chkPagos_Click(object sender, RoutedEventArgs e)
        {
            FiltroSituacaoParcelas();
        }

        private void chkPendentes_Click(object sender, RoutedEventArgs e)
        {
            FiltroSituacaoParcelas();
        }

        private void chkParciais_Click(object sender, RoutedEventArgs e)
        {
            FiltroSituacaoParcelas();
        }

        private void chkCancelados_Click(object sender, RoutedEventArgs e)
        {
            FiltroSituacaoParcelas();
        }

        #region Busca de Imagens
        String novoCaminho;
        private void btBuscarArquivos_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog() { Multiselect = true };
            bool? response = openFileDialog.ShowDialog();
            if (response == true)
            {
                IList<string> arquivos = new List<string>(openFileDialog.FileNames);
                novoCaminho = NovoCaminho();
                if (string.IsNullOrEmpty(novoCaminho))
                {
                    MessageBox.Show("Defina o caminho padrão para salvar os arquivos em configuracões!", "Informacão", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
                if (contaArquivos.Count == 0)
                {
                    if (String.IsNullOrEmpty(txtNome.Text) || String.IsNullOrEmpty(txtEmissao.Text))
                    {
                        MessageBox.Show("Os campos nome e data de emissão são obrigatórios, por favor verifique!", "Atenção", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        return;
                    }
                    #region Adiciona arquivo quando a conta ainda nao foi salva, Id neste caso é 0
                    PreencheListView();
                    foreach (var arquivo in arquivos)
                    {
                        var nomeArquivo = System.IO.Path.GetFileName(arquivo);
                        var caminhoCompleto = string.Format("{0}\\{1}", novoCaminho, nomeArquivo);

                        if (!File.Exists(caminhoCompleto))
                        {
                            File.Copy(arquivo, System.IO.Path.Combine(novoCaminho, new FileInfo(arquivo).Name));
                            contaArquivos.Add(new ContaArquivo() { Caminho = novoCaminho, Nome = nomeArquivo, DataGeracao = DateTime.Now });
                            lstArquivos.Items.Refresh();
                        }
                    }
                    #endregion
                }
                else
                {
                    #region Adiciona arquivo quando a conta ja existe, Id diferente de 0
                    foreach (var arquivo in arquivos)
                    {
                        var nomeArquivo = System.IO.Path.GetFileName(arquivo);
                        var caminhoCompleto = string.Format("{0}\\{1}", novoCaminho, nomeArquivo);
                        if (!File.Exists(caminhoCompleto))
                        {
                            File.Copy(arquivo, System.IO.Path.Combine(novoCaminho, new FileInfo(arquivo).Name));
                            contaArquivos.Add(new ContaArquivo() { Caminho = novoCaminho, Nome = nomeArquivo, DataAlteracao = DateTime.Now });
                            lstArquivos.Items.Refresh();
                        }
                    }
                    #endregion
                }
                if (!Salvar())
                    MessageBox.Show("Houve algum erro ao salvar a conta!", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void lstArquivos_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Configuracao configuracao = new RepositorioConfiguracao(Session).ObterTodos().FirstOrDefault();

            string ArquivoSelecionado = lstArquivos.SelectedItem.ToString();
            var caminhoCompleto = string.Format("{0}\\Conta_{1}\\{2}", configuracao.CaminhoArquivos, conta.Codigo, ArquivoSelecionado);

            if (File.Exists(caminhoCompleto))
                Process.Start(caminhoCompleto);
            else
                MessageBox.Show("Arquivo não existe!", "Informação", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void MenuItemRemover_Click(object sender, RoutedEventArgs e)
        {
            ContaArquivo selecao = (ContaArquivo)lstArquivos.SelectedItem;
            var caminhoPastaSistema = string.Format("{0}\\{1}", selecao.Caminho, selecao.Nome);
            if (File.Exists(caminhoPastaSistema))
                File.Delete(caminhoPastaSistema);
            if (conta.Id != 0)
            {
                conta.ContaArquivos.Remove((ContaArquivo)selecao);
                RepositorioContaArquivo.Excluir((ContaArquivo)selecao);
            }
            contaArquivos.Remove((ContaArquivo)selecao);

        }

        private void RectArrastarSoltar_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                IList<string> arquivos = new List<string>((string[])e.Data.GetData(DataFormats.FileDrop));
                novoCaminho = NovoCaminho();
                if (string.IsNullOrEmpty(novoCaminho))
                {
                    MessageBox.Show("Defina o caminho padrão para salvar os arquivos em configuracões!", "Informacão", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
                if (contaArquivos.Count == 0)
                {
                    if (String.IsNullOrEmpty(txtNome.Text) || String.IsNullOrEmpty(txtEmissao.Text))
                    {
                        MessageBox.Show("Os campos nome e data de emissão são obrigatórios, por favor verifique!", "Atenção", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        return;
                    }
                    #region Adiciona arquivo quando a conta ainda nao foi salva, Id neste caso é 0

                    PreencheListView();

                    foreach (var arquivo in arquivos)
                    {
                        var nomeArquivo = System.IO.Path.GetFileName(arquivo);
                        var caminhoCompleto = string.Format("{0}\\{1}", novoCaminho, nomeArquivo);

                        if (!File.Exists(caminhoCompleto))
                        {
                            File.Copy(arquivo, System.IO.Path.Combine(novoCaminho, new FileInfo(arquivo).Name));
                            contaArquivos.Add(new ContaArquivo() { Caminho = novoCaminho, Nome = nomeArquivo, DataGeracao = DateTime.Now });
                            lstArquivos.Items.Refresh();
                        }
                    }
                    #endregion
                }
                else
                {
                    #region Adiciona arquivo quando a conta ja existe, Id diferente de 0
                    foreach (var arquivo in arquivos)
                    {
                        var nomeArquivo = System.IO.Path.GetFileName(arquivo);
                        var caminhoCompleto = string.Format("{0}\\{1}", novoCaminho, nomeArquivo);
                        if (!File.Exists(caminhoCompleto))
                        {
                            File.Copy(arquivo, System.IO.Path.Combine(novoCaminho, new FileInfo(arquivo).Name));
                            contaArquivos.Add(new ContaArquivo() { Caminho = novoCaminho, Nome = nomeArquivo, DataAlteracao = DateTime.Now });
                            lstArquivos.Items.Refresh();
                        }
                    }
                    #endregion
                }
                if (!Salvar())
                    MessageBox.Show("Houve algum erro ao salvar a conta!", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        #endregion

        private void btFinalizarConta_Click(object sender, RoutedEventArgs e)
        {
            if (conta.Id != 0)
            {
                if (!VerificaParcelasPendentes())
                {
                    MessageBoxResult d = MessageBox.Show("Deseja finalizar essa conta?!", " Atenção ", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (d == MessageBoxResult.Yes)
                    {
                        conta.Situacao = SituacaoConta.Finalizado;
                        Repositorio.Alterar(conta);
                        LimpaCampos();
                        ControleAcessoInicial();
                    }
                }
                else
                {
                    MessageBoxResult d = MessageBox.Show("Existem parcelas pendentes, deseja finalizar assim mesmo?", " Atenção ", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (d == MessageBoxResult.Yes)
                    {
                        conta.Situacao = SituacaoConta.Finalizado;
                        Repositorio.Alterar(conta);
                        LimpaCampos();
                        ControleAcessoInicial();
                    }
                }
            }
        }

        private void txtNumDocumento_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = Regex.IsMatch(e.Text, "[^0-9]+");
        }

        private void btNovasParcelas_Click(object sender, RoutedEventArgs e)
        {
            NovasParcelas janela = new NovasParcelas(conta, Session);
            bool? res = janela.ShowDialog();
            if ((bool)res)
            {
                //conta.QtdParcelas = conta.QtdParcelas += janela.conta.QtdParcelas;
                //conta.ValorTotal = janela.conta.ValorTotal;
                foreach (var item in janela.contaPagamento)
                {
                    contaPagamento.Add(item);
                }
                Salvar();
                DataGridContaPagamento.ItemsSource = contaPagamento;
                DataGridContaPagamento.Items.Refresh();
                FiltroSituacaoParcelas();
            }
        }
    }
}

