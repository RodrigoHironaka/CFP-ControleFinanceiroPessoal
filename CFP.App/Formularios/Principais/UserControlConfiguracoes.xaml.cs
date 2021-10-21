using CFP.Dominio.Dominio;
using CFP.Repositorio.Repositorio;
using NHibernate;
using SGE.Repositorio.Configuracao;
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

namespace CFP.App.Formularios.Principais
{
    /// <summary>
    /// Interação lógica para UserControlConfiguracoes.xam
    /// </summary>
    public partial class UserControlConfiguracoes : UserControl
    {
        #region Session
        private static ISession session;
        protected static ISession Session
        {
            get
            {
                if (session == null || !session.IsOpen)
                {
                    if (session != null)
                        session.Dispose();
                    session = NHibernateHelper.GetSession();
                }
                return session;
            }
        }
        #endregion

        Configuracao configuracao = new Configuracao();

        #region Repositorio
        private RepositorioConfiguracao _repositorio;
        public RepositorioConfiguracao Repositorio
        {
            get
            {
                if (_repositorio == null)
                    _repositorio = new RepositorioConfiguracao(Session);

                return _repositorio;
            }
            set { _repositorio = value; }
        }
        #endregion

        #region Preenche Objeto para Salvar
        private bool PreencheObjeto()
        {
            try
            {
                configuracao.CaminhoArquivos = txtCaminhoArquivos.Text;
                configuracao.CaminhoBackup = txtCaminhoBackup.Text;
                configuracao.ServidorBD = txtServidor.Text;
                configuracao.BaseBD = txtBase.Text;
                configuracao.UsuarioBD = txtUsuario.Text;
                configuracao.SenhaBD = txtSenha.Password;
                configuracao.PortaBD = txtPorta.Text;
                return true;
            }
            catch
            {
                return false;
            }

        }
        #endregion

        #region Preenche campos no user control
        private void PreencheCampos()
        {
            List<Configuracao> todasConfiguracoes = (List<Configuracao>)Repositorio.ObterTodos().ToList();
            foreach (var item in todasConfiguracoes)
                configuracao = Repositorio.ObterPorId(item.Id);
            
            if (configuracao != null)
            {
                txtCodigo.Text = configuracao.Id.ToString();
                txtCaminhoArquivos.Text = configuracao.CaminhoArquivos;
                txtCaminhoBackup.Text = configuracao.CaminhoBackup;
                txtServidor.Text = configuracao.ServidorBD;
                txtBase.Text = configuracao.BaseBD;
                txtPorta.Text = configuracao.PortaBD;
            }
        }
        #endregion

        #region Escolher caminhos de pastas
        private string EscolherCaminho()
        {
            string retornando = string.Empty;
            using (var caminho = new System.Windows.Forms.FolderBrowserDialog())
            {
                System.Windows.Forms.DialogResult res = caminho.ShowDialog();
                if (res == System.Windows.Forms.DialogResult.OK)
                    retornando = caminho.SelectedPath.ToString();
            }
            return retornando;
        }
        #endregion

        #region DataAtualizacao
        private void UltimaAltualizacao()
        {
            if(configuracao != null && configuracao.DataAlteracao != DateTime.MinValue)
                lblAtualizadoEm.Text = string.Format("Atualizado em: {0}", configuracao.DataAlteracao);
            else
                lblAtualizadoEm.Text = configuracao != null && configuracao.DataGeracao != DateTime.MinValue ? string.Format("Atualizado em: {0}", configuracao.DataGeracao) : string.Empty;
        }
        #endregion

        public UserControlConfiguracoes()
        {
            InitializeComponent();
            PreencheCampos();
            UltimaAltualizacao();
        }

        private void btSair_Click(object sender, RoutedEventArgs e)
        {
            (this.Parent as Grid).Children.Remove(this);

        }

        private void btPesquisaCaminhoArquivos_Click(object sender, RoutedEventArgs e)
        {
            txtCaminhoArquivos.Text = EscolherCaminho();
        }

        private void btPesquisaCaminhoBackup_Click(object sender, RoutedEventArgs e)
        {
            txtCaminhoBackup.Text = EscolherCaminho();
        }

        private void btSalvar_Click(object sender, RoutedEventArgs e)
        {
            if (PreencheObjeto())
            {
                if (configuracao != null && configuracao.Id == 0)
                {
                    configuracao.DataGeracao = DateTime.Now;
                    Repositorio.Salvar(configuracao);
                    txtCodigo.Text = configuracao.Id.ToString();
                    UltimaAltualizacao();
                }
                else
                {
                    configuracao.DataAlteracao = DateTime.Now;
                    Repositorio.Alterar(configuracao);
                    UltimaAltualizacao();
                }
            }
        }
    }
}
