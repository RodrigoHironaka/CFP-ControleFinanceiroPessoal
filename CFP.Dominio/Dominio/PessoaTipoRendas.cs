using Dominio.Interfaces;
using Dominio.ObejtoValor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Dominio
{
    public class PessoaTipoRendas
    {
        public virtual Int64 ID { get; set; }

        //criar um botao na frente de renda bruta onde apareca uma calculadora para calcular inss / desc sindicato(%),
        //add horaextra(precisa ser algo automatico, ao virar o mÊs ele puxa o total da tabela horaextra e calcula junto do salario 
        public virtual Double RendaBruta { get; set; }

        //se não clicar no botao calcular, o valor liquido sera o  mesmo do bruto;
        public virtual Double RendaLiquida { get; set; }
        public virtual TipoRenda TipoRenda { get; set; }
        public virtual Pessoa Pessoa { get; set; }
    }
}
