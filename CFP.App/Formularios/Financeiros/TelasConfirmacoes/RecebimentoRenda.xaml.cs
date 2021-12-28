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
using System.Windows.Shapes;

namespace CFP.App.Formularios.Financeiros.TelasConfirmacoes
{
    /// <summary>
    /// Lógica interna para RecebimentoRenda.xaml
    /// </summary>
    public partial class RecebimentoRenda : Window
    {
        ISession Session;
        Caixa caixa;
        List<PessoaTipoRendas> PessoaTipoRendas;

        #region Carrega Combo
        private void CarregaCombo()
        {
            cmbBanco.ItemsSource = new RepositorioBanco(Session)
                .ObterPorParametros(x => x.Situacao == Situacao.Ativo)
                .OrderBy(x => x.Nome)
                .ToList();

            if(rgParaCaixa.IsChecked == true)
            {
                cmbPagamento.ItemsSource = new RepositorioFormaPagamento(Session)
                .ObterPorParametros(x => x.Situacao == Situacao.Ativo)
                .OrderBy(x => x.Nome)
                .ToList();
            }
            else
            {
                cmbPagamento.ItemsSource = new RepositorioFormaPagamento(Session)
                .ObterPorParametros(x => x.Situacao == Situacao.Ativo && x.TransacoesBancarias == SimNao.Sim)
                .OrderBy(x => x.Nome)
                .ToList();
            }
        }
        #endregion

        #region Repositorio
        private RepositorioCofre _repositorio;
        public RepositorioCofre RepositorioCofre
        {
            get
            {
                if (_repositorio == null)
                    _repositorio = new RepositorioCofre(Session);

                return _repositorio;
            }
            set { _repositorio = value; }
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

        #region Verificando onde será enviado
        private void CaixaOuCofre()
        {
            if (rgParaCaixa.IsChecked == true)
            {
                CarregaCombo();
                cmbBanco.IsEnabled = false;
                cmbPagamento.IsEnabled = true;
            }
            else
            {
                CarregaCombo();
                cmbBanco.IsEnabled = true;
                cmbPagamento.IsEnabled = true;
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

                    caixa.Codigo = RepositorioCaixa.RetornaUltimoCodigo() + 1;
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

        public RecebimentoRenda(List<PessoaTipoRendas> _pessoasTipoRendas, ISession _session)
        {
            InitializeComponent();
            Session = _session;
            PessoaTipoRendas = _pessoasTipoRendas;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            rgParaCaixa.IsChecked = true;
            txtValorRenda.Text = PessoaTipoRendas.Select(x => x.RendaLiquida).Sum().ToString("n2");
        }

        private void btCancelar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btConfirmar_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult d = MessageBox.Show("Confirma o recebimento?", "Informação", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (d == MessageBoxResult.Yes)
            {
                foreach (PessoaTipoRendas item in PessoaTipoRendas)
                {
                    if (rgParaCaixa.IsChecked == true)
                    {
                        if (VerificaCaixa())
                        {
                            FluxoCaixa fluxoCaixa = new FluxoCaixa
                            {
                                TipoFluxo = EntradaSaida.Entrada,
                                Nome = String.Format("Recebimento Renda {0} de {1}.", item.TipoRenda.Nome, item.Pessoa.Nome),
                                Valor = item.RendaLiquida,
                                DataGeracao = DateTime.Now,
                                Conta = null,
                                UsuarioCriacao = MainWindow.UsuarioLogado,
                                Caixa = caixa,
                                FormaPagamento = (FormaPagamento)cmbPagamento.SelectedItem
                            };
                            RepositorioFluxoCaixa.Salvar(fluxoCaixa);
                        }
                    }
                    else
                    {
                        Cofre cofreEntrada = new Cofre
                        {
                            Codigo = RepositorioCofre.RetornaUltimoCodigo() + 1,
                            Caixa = null,
                            Banco = (Banco)cmbBanco.SelectedItem,
                            Valor = item.RendaLiquida,
                            TransacoesBancarias = (FormaPagamento)cmbPagamento.SelectedItem,
                            Situacao = EntradaSaida.Entrada,
                            Nome = String.Format("Recebimento Renda {0} de {1}", item.TipoRenda.Nome, item.Pessoa.Nome),
                            DataGeracao = DateTime.Now,
                            UsuarioCriacao = MainWindow.UsuarioLogado
                        };
                        RepositorioCofre.Salvar(cofreEntrada);
                    }
                   
                }
            }
            DialogResult = true;
        }

        private void rgParaCaixa_Checked(object sender, RoutedEventArgs e)
        {
            CaixaOuCofre();
        }

        private void rgParaCofre_Checked(object sender, RoutedEventArgs e)
        {
            CaixaOuCofre();
        }
    }
}
