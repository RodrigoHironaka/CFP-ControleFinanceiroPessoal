﻿using CFP.Dominio.Dominio;
using CFP.Dominio.ObjetoValor;
using CFP.Repositorio.Repositorio;
using Dominio.Dominio;
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
    /// Lógica interna para EntradaSaidaManual.xaml
    /// </summary>
    public partial class EntradaSaidaManual : Window
    {

        ISession Session;
        Boolean Entrada;
        Caixa caixa;

        #region Carrega combo
        private void CarregaCombo()
        {
            cmbFormaPagamento.ItemsSource = new RepositorioFormaPagamento(Session)
               .ObterPorParametros(x => x.Situacao == Situacao.Ativo)
               .OrderBy(x => x.Nome)
               .ToList();
        }
        #endregion
        public EntradaSaidaManual(Boolean _entrada, Caixa _caixa, ISession _session)
        {
            InitializeComponent();
            Session = _session;
            Entrada = _entrada;
            caixa = _caixa;
        }

        private void btConfirmar_Click(object sender, RoutedEventArgs e)
        {
            FluxoCaixa fluxoCaixa = new FluxoCaixa();
            if (Entrada)
            {
                fluxoCaixa.TipoFluxo = EntradaSaida.Entrada;
                fluxoCaixa.Nome = fluxoCaixa.Nome = String.Format("Entrada manual no caixa. Descrição: {0}", txtNome.Text);
            }
            else
            {
                fluxoCaixa.TipoFluxo = EntradaSaida.Saída;
                fluxoCaixa.Nome = fluxoCaixa.Nome = String.Format("Saída manual no caixa. Descrição: {0}", txtNome.Text);
            }
            fluxoCaixa.DataGeracao = DateTime.Now;
            fluxoCaixa.Conta = null;
            fluxoCaixa.UsuarioLogado = MainWindow.UsuarioLogado;
            fluxoCaixa.Valor = Decimal.Parse(txtValorInicial.Text);
            fluxoCaixa.Caixa = caixa;
            fluxoCaixa.FormaPagamento = (FormaPagamento)cmbFormaPagamento.SelectedItem;
            new RepositorioFluxoCaixa(Session).Salvar(fluxoCaixa);
            Close();
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