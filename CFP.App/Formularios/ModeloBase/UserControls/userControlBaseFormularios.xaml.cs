using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace CFP.App.Formularios.ModeloBase.UserControls
{
    /// <summary>
    /// Interação lógica para userControlBaseFormularios.xam
    /// </summary>
    public partial class userControlBaseFormularios : UserControl
    {
        ISession Session;

        #region Carrega Combos
        private void CarregaCombos()
        {
            //carrega combo Situacao e define Ativo 
            //cmbSituacao.ItemsSource = Enum.GetValues(typeof(Situacao));
        }
        #endregion

        #region Repositorio
        //private RepositorioFormaPagamento _repositorio;
        //public RepositorioFormaPagamento Repositorio
        //{
        //    get
        //    {
        //        if (_repositorio == null)
        //            _repositorio = new RepositorioFormaPagamento(Session);

        //        return _repositorio;
        //    }
        //    set { _repositorio = value; }
        //}
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
            //txtNome.Focus();
            //txtNome.Select(txtNome.Text.Length, 0);

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
        //    foreach (var item in GridControls.Children)
        //    {
        //        if (item is TextBox)
        //            (item as TextBox).Text = string.Empty;
        //        if (item is ComboBox)
        //            (item as ComboBox).SelectedIndex = -1;
        //        if (item is CheckBox)
        //            (item as CheckBox).IsChecked = false;
        //        if (item is RadioButton)
        //            (item as RadioButton).IsChecked = false;
        //    }
        }
        #endregion

        #region Preenche Objeto para Salvar
       // private bool PreencheObjeto()
        //{
        //    try
        //    {
        //        formaPagamento.Nome = txtNome.Text;
        //        formaPagamento.QtdParcelas = Int32.Parse(txtQtdParcelas.Text);
        //        formaPagamento.DiasParaVencimento = Int32.Parse(txtDiasVencimento.Text);
        //        formaPagamento.Situacao = (Situacao)cmbSituacao.SelectedIndex;
        //        return true;
        //    }
        //    catch
        //    {
        //        return false;
        //    }

        //}
        #endregion

        #region Preenche campos no user control
        //private void PreencheCampos()
        //{
        //    if (formaPagamento != null)
        //    {
        //        txtCodigo.Text = formaPagamento.Id.ToString();
        //        txtNome.Text = formaPagamento.Nome;
        //        txtQtdParcelas.Text = formaPagamento.QtdParcelas.ToString();
        //        txtDiasVencimento.Text = formaPagamento.DiasParaVencimento.ToString();
        //        cmbSituacao.SelectedIndex = formaPagamento.Situacao.GetHashCode();
        //    }
        //}
        #endregion

        public userControlBaseFormularios()
        {
            InitializeComponent();
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
    }
}
