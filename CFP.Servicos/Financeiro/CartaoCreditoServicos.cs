using CFP.Dominio.Dominio;
using CFP.Dominio.ObjetoValor;
using CFP.Repositorio.Repositorio;
using Dominio.Dominio;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFP.Servicos.Financeiro
{
    public class CartaoCreditoServicos
    {
        public static void NovaFaturaCartaoCredito(CartaoCredito cartaoCredito, Usuario usuarioLogado, ISession session)
        {
            CartaoCredito CartaoCredito = new CartaoCredito()
            {
                MesReferencia = cartaoCredito.MesReferencia,
                AnoReferencia = cartaoCredito.AnoReferencia,
                Cartao = cartaoCredito.Cartao,
                ValorFatura = 0,
                SituacaoFatura = SituacaoFatura.Aberta,
                DataGeracao = DateTime.Now,
                UsuarioCriacao = usuarioLogado
            };
            new RepositorioCartaoCredito(session).SalvarLote(CartaoCredito);
            ContaServicos.NovaContaRefCartaoCredito(usuarioLogado, CartaoCredito, session);
        }
    }
}
