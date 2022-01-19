using CFP.Dominio.Dominio;
using Dominio.Dominio;
using NHibernate;
using NHibernate.Criterion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositorio.Repositorios
{
    public class RepositorioConta : RepositorioBase<Conta>
    {
        public RepositorioConta(ISession session) : base(session) { }

        public void CriarNovaContaPadrao(Usuario usuario, string nome, SubGrupoGasto subGrupoGasto, FormaPagamento formaPagamento, List<ContaPagamento> contaPagamento)
        {
            Conta conta = new Conta();
            conta.Codigo = RetornaUltimoCodigo() + 1;
            conta.UsuarioCriacao = usuario;
            conta.Nome = nome;
            conta.TipoConta = Dominio.ObjetoValor.TipoConta.Pagar;
            conta.SubGrupoGasto = subGrupoGasto;
            conta.TipoPeriodo = Dominio.ObejtoValor.TipoPeriodo.Unica;
            conta.Situacao = Dominio.ObjetoValor.SituacaoConta.Aberto;
            conta.DataEmissao = DateTime.Now;
            conta.Pessoa = null;
            conta.NumeroDocumento = null;
            conta.FormaCompra = formaPagamento;
            conta.Observacao = string.Empty;

            //tab Pagamento
            conta.ContaPagamentos = contaPagamento;

            Salvar(conta);
        }
    }
}
