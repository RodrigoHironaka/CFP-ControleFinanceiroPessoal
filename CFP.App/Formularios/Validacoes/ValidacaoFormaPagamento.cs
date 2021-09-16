using Dominio.ObjetoValor;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace CFP.App.Formularios.Validacoes
{
    public class ValidacaoFormaPagamento : ValidationRule
    {
        //    public String Nome { get; set; }
        //    public Situacao Situacao { get; set; }

        //    public string Error
        //    {
        //        get { throw new NotImplementedException(); }
        //    }

        //    public string this[string nomecoluna]
        //    {
        //        get
        //        {
        //            string resultado = null;
        //            if (nomecoluna == "Nome")
        //            {
        //                if (String.IsNullOrEmpty(Nome) || Nome.Length < 3)
        //                    resultado = "Informe um nome válido";
        //            }
        //            if (nomecoluna == "Situacao")
        //            {
        //                if (String.IsNullOrEmpty(Situacao.ToString()))
        //                    resultado = "Informe uma situação válida";
        //            }
        //            return resultado;
        //        }
        //    }
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            return !string.IsNullOrWhiteSpace((value ?? "").ToString())
                ? new ValidationResult(false, "Campo Requerido") : ValidationResult.ValidResult;
        }
    }
}
