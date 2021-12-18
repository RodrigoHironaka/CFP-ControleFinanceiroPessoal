using CFP.Dominio.ObjetoValor;
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
    /// Lógica interna para ConfirmaValorInicialCaixa.xaml
    /// </summary>
    public partial class ConfirmaValorInicialCaixa : Window
    {
        internal String valorDigitado;
        public ConfirmaValorInicialCaixa()
        {
            InitializeComponent();
        }

        private void btConfirmar_Click(object sender, RoutedEventArgs e)
        {
            valorDigitado = txtValorInicial.Text.Trim();
            this.DialogResult = true;
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

        private void txtValorInicial_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!String.IsNullOrEmpty(txtValorInicial.Text))
            {
                Decimal valido;
                var valor = decimal.TryParse(txtValorInicial.Text, out valido);
                if (!valor)
                {
                    MessageBox.Show("Texto colado é inválido! Por favor verifique.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtValorInicial.Clear();
                    txtValorInicial.Focus();
                }
            }
        }
    }
}
