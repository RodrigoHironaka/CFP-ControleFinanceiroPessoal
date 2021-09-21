using CFP.App.Formularios.Pesquisas;
using Dominio.Dominio;
using Dominio.ObjetoValor;
using LinqKit;
using NHibernate;
using Repositorio.Repositorios;
using SGE.Repositorio.Configuracao;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace CFP.App.Formularios.Cadastros
{
    /// <summary>
    /// Interação lógica para UserControlPessoa.xam
    /// </summary>
    public partial class UserControlPessoa : UserControl
    {
        ISession Session;
        Pessoa pessoa;

        #region Carrega Combos
        private void CarregaCombos()
        {
            //carrega combo Situacao e define Ativo 
            cmbSituacao.ItemsSource = Enum.GetValues(typeof(Situacao));
            cmbSituacao.SelectedIndex = 0;

            cmbRenda.ItemsSource = new RepositorioTipoRenda(Session)
                .ObterPorParametros(x => x.Situacao == Situacao.Ativo)
                .OrderBy(x => x.Nome)
                .ToList<TipoRenda>();

        }
        #endregion

        #region Repositorios
        private RepositorioPessoa _repositorio;
        public RepositorioPessoa Repositorio
        {
            get
            {
                if (_repositorio == null)
                    _repositorio = new RepositorioPessoa(Session);

                return _repositorio;
            }
            set { _repositorio = value; }
        }

        private RepositorioPessoaTipoRendas _repositorioTipoRendas;
        public RepositorioPessoaTipoRendas RepositorioPessoaTipoRenda
        {

            get
            {
                if (_repositorioTipoRendas == null)
                    _repositorioTipoRendas = new RepositorioPessoaTipoRendas(Session);

                return _repositorioTipoRendas;
            }
            set { _repositorioTipoRendas = value; }

        }
        #endregion

        #region Controle de acessos Inicial e Cadastro
        private void ControleAcessoInicial()
        {
            //Bloqueando
            GridCampos.IsEnabled = !GridCampos.IsEnabled;
            btSalvar.IsEnabled = !btSalvar.IsEnabled;
            btExcluir.IsEnabled = !btExcluir.IsEnabled;

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
            foreach (var item in GridControls.Children)
            {
                if (item is TextBox)
                    (item as TextBox).Text = string.Empty;
                if (item is ComboBox)
                    (item as ComboBox).SelectedIndex = 0;
                if (item is CheckBox)
                    (item as CheckBox).IsChecked = false;
                if (item is RadioButton)
                    (item as RadioButton).IsChecked = false;

                if (item is GroupBox)
                {
                    foreach (var groupBox in StackPanelCamposRenda.Children)
                    {
                        if (groupBox is TextBox)
                            (groupBox as TextBox).Text = string.Empty;
                        if (groupBox is ComboBox)
                            (groupBox as ComboBox).SelectedIndex = 0;
                    }
                }
                if (item is Grid)
                {
                    foreach (var grid in GridTotais.Children)
                    {
                        if (grid is TextBox)
                            (grid as TextBox).Text = string.Empty;
                        if (grid is ListView)
                            pessoaTipoRenda.Clear();
                    }
                }

            }
        }
        #endregion

        #region PreencheGrid
        private ObservableCollection<PessoaTipoRendas> pessoaTipoRenda;
        public void PreencheDataGrid()
        {
            pessoaTipoRenda = new ObservableCollection<PessoaTipoRendas>();
            lstRendas.ItemsSource = pessoaTipoRenda;
            //DataGridRendas.ItemsSource = new ObservableCollection<PessoaTipoRendas>(pessoa.PessoaTipoRendas);
        }
        #endregion

        #region Preenche Objeto para Salvar
        private bool PreencheObjeto()
        {
            try
            {
                pessoa.Nome = txtNome.Text;
                pessoa.Situacao = (Situacao)cmbSituacao.SelectedIndex;
                pessoa.ValorTotalBruto = Decimal.Parse(txtTotalBruto.Text);
                pessoa.ValorTotalLiquido = Decimal.Parse(txtTotalLiquido.Text);
                pessoa.PessoaTipoRendas = (System.Collections.Generic.IList<PessoaTipoRendas>)lstRendas.ItemsSource;
                return true;
            }
            catch
            {
                return false;
            }

        }

        private void PreencheObjetoComListaDadaGrid()
        {
            if (pessoa.PessoaTipoRendas.Count != 0)
            {
                foreach (var item in pessoa.PessoaTipoRendas)
                {
                    PessoaTipoRendas pessoaTipoRendas = new PessoaTipoRendas();
                    pessoaTipoRendas.Pessoa = item.Pessoa;
                    pessoaTipoRendas.TipoRenda = item.TipoRenda;
                    pessoaTipoRendas.RendaBruta = item.RendaBruta;
                    pessoaTipoRendas.RendaLiquida = item.RendaLiquida;
                }
            }
        }
        #endregion

        #region Preenche campos no user control
        private void PreencheCampos()
        {
            if (pessoa != null)
            {
                txtCodigo.Text = pessoa.Id.ToString();
                txtNome.Text = pessoa.Nome;
                cmbSituacao.SelectedIndex = pessoa.Situacao.GetHashCode();
                txtTotalBruto.Text = pessoa.ValorTotalBruto.ToString("N2");
                txtTotalLiquido.Text = pessoa.ValorTotalLiquido.ToString("N2");
                var listaPessoaRendas = pessoa.PessoaTipoRendas;
                foreach (var item in listaPessoaRendas)
                {
                    pessoaTipoRenda.Add(item);
                }

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

        #region Soma Total renda bruta e liquida
        Decimal somaRendaBruta = 0;
        Decimal somaRendaLiquida = 0;
        private void SomaTotalBrutoeLiquido()
        {
            somaRendaBruta = pessoaTipoRenda.Sum(c => c.RendaBruta);
            txtTotalBruto.Text = somaRendaBruta.ToString("N2");
            somaRendaLiquida = pessoaTipoRenda.Sum(c => c.RendaLiquida);
            txtTotalLiquido.Text = somaRendaLiquida.ToString("N2");
        }
        #endregion

        #region Adicionar e Remover item do listview
        //Adicionar
        public void AdicionarItemLista()
        {
            if (cmbRenda.SelectedItem != null)
            {
                pessoaTipoRenda.Add(new PessoaTipoRendas()
                {
                    TipoRenda = (TipoRenda)cmbRenda.SelectedItem,
                    RendaBruta = txtRendaBruto.Text != string.Empty ? Decimal.Parse(txtRendaBruto.Text) : 0,
                    RendaLiquida = txtRendaLiquida.Text != string.Empty ? Decimal.Parse(txtRendaLiquida.Text) : Decimal.Parse(txtRendaBruto.Text)
                });
                SomaTotalBrutoeLiquido();
            }
        }

        //Remover
        public void RemoverItemLista()
        {
            var selecao = lstRendas.SelectedItem;
            foreach (var item in pessoaTipoRenda)
            {
                if (item == selecao)
                {
                    pessoaTipoRenda.Remove(item);
                    RepositorioPessoaTipoRenda.Excluir(item);
                    break;
                }
            }
            SomaTotalBrutoeLiquido();
        }
        #endregion

        public UserControlPessoa(Pessoa _pessoa, ISession _session)
        {
            InitializeComponent();
            Session = _session;
            pessoa = _pessoa;
        }

        private void btCancelar_Click(object sender, RoutedEventArgs e)
        {
            if (GridCampos.IsEnabled)
            {
                ControleAcessoInicial();
                FocoNoCampoCodigo();
                LimpaCampos();
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
                // verifico se campo codigo esta preenchido para incluir novos dados ou carregar um existente
                if (String.IsNullOrEmpty(txtCodigo.Text))
                {
                    pessoa = new Pessoa();
                    LimpaCampos();
                    ControleAcessoCadastro();
                    CorPadrãoBotaoPesquisar();
                }
                else
                {
                    try
                    {
                        pessoa = Repositorio.ObterPorId(Int64.Parse(txtCodigo.Text));
                        if (pessoa != null)
                        {
                            PreencheCampos();
                            ControleAcessoCadastro();
                            CorPadrãoBotaoPesquisar();
                        }
                        else
                        {
                            txtCodigo.SelectAll();
                            txtCodigo.Focus();
                            btPesquisar.Background = Brushes.DarkRed;
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.ToString());
                        pessoa = null;
                    }
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

        private void btSalvar_Click(object sender, RoutedEventArgs e)
        {
            if (PreencheObjeto())
            {
                if ((pessoa.Id == 0) && (String.IsNullOrEmpty(txtCodigo.Text)))
                {
                    pessoa.DataGeracao = DateTime.Now;
                    pessoa.PessoaTipoRendas.ToList().ForEach(x => x.Pessoa = pessoa);
                    Repositorio.Salvar(pessoa);
                    PreencheObjetoComListaDadaGrid();
                    txtCodigo.Text = pessoa.Id.ToString();
                }
                else
                {
                    IList<PessoaTipoRendas> ptr = new List<PessoaTipoRendas>(pessoa.PessoaTipoRendas.Count);
                    foreach (var item in (IList<PessoaTipoRendas>)lstRendas.ItemsSource)
                        ptr.Add(item);

                    pessoa.PessoaTipoRendas.Clear();
                    foreach (var item in ptr)
                    {
                        if (item.Pessoa == null)
                            item.Pessoa = this.pessoa;
                        pessoa.PessoaTipoRendas.Add(item);
                    }
                    Repositorio.Alterar(pessoa);
                }

                ControleAcessoInicial();
                FocoNoCampoCodigo();
            }
        }

        private void btExcluir_Click(object sender, RoutedEventArgs e)
        {
            if (pessoa != null)
            {
                MessageBoxResult d = MessageBox.Show("Deseja realmente excluir o registro: " + pessoa.Nome + " ? ", " Atenção ", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (d == MessageBoxResult.Yes)
                {
                    //if(pessoa.PessoaTipoRendas.Count > 0)
                    //{
                    //    foreach (var item in pessoa.PessoaTipoRendas)
                    //    {
                    //        if (item.Pessoa == pessoa)
                    //        {
                    //            RepositorioPessoaTipoRenda.Excluir(item);
                                
                    //        }
                    //    }
                    //}
                  
                    Repositorio.Excluir(pessoa);
                    LimpaCampos();
                    ControleAcessoInicial();
                }
            }
        }

        private void btAdd_Click(object sender, RoutedEventArgs e)
        {
            AdicionarItemLista();
            cmbRenda.Focus();
        }

        private void btRem_Click(object sender, RoutedEventArgs e)
        {
            RemoverItemLista();
            cmbRenda.Focus();
        }

        private void txtRendaBruto_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = Regex.IsMatch(e.Text, "[^0-9]+");
        }

        private void txtRendaBruto_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }

        private void txtRendaLiquida_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = Regex.IsMatch(e.Text, "[^0-9]+");
        }

        private void txtRendaLiquida_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }

        private void btPesquisar_Click(object sender, RoutedEventArgs e)
        {
            PesquisaPessoas p = new PesquisaPessoas(Session);
            p.ShowDialog();
            if (p.objeto != null)
            {
                pessoa = p.objeto;
                PreencheCampos();
                ControleAcessoCadastro();
                CorPadrãoBotaoPesquisar();
            }
        }
    }
}
