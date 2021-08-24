using Dominio.Interfaces;
using Dominio.ObejtoValor;
using Dominio.ObjetoValor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Dominio
{
    public class Conta : IEntidade
    {
        public virtual Int64 ID { get; set; }
        public virtual TipoConta TipoConta { get; set; }
        public virtual TipoConta2 TipoConta2 { get; set; }
        public virtual Int64 QtdMeses { get; set; }
        public virtual Pessoa Pessoa { get; set; }
        //public virtual ContaPagamento ContaPagamento { get; set; }
        public virtual Int64 SalarioMensal { get; set; }
        public virtual String Nome { get; set; }
        public virtual GrupoGasto GrupoGasto { get; set; }
        public virtual Double ValorTotal { get; set; }
        public virtual Double ValorParcelado { get; set; }
        public virtual DateTime DataEmissao { get; set; } 
        public virtual DateTime DataVencimento { get; set; }
        public virtual Int64 NumeroDocumento { get; set; }
        public virtual String Observacao { get; set; }
        public virtual SituacaoConta Situacao { get; set; }
    }
}
