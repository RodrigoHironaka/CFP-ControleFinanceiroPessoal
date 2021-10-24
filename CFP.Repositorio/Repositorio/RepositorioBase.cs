using Dominio.Interfaces;
using LinqKit;
using NHibernate;
using NHibernate.Criterion;
using SGE.Repositorio.Configuracao;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace Repositorio.Repositorios
{
    public class RepositorioBase<T> where T : class
    {
        public RepositorioBase(ISession session)
        {
            if (session == null)
                throw new ArgumentException("A session deve existir.", "session");
            _session = session;
        }
        private readonly ISession _session;

        protected ISession Session
        {
            get { return _session; }
        }

        public void Salvar(T entidade)
        {
            using (var trans = Session.BeginTransaction())
            {
                try
                {
                    //entidade.DataGeracao = DateTime.Now;
                    this.Session.Save(entidade);
                    trans.Commit();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw ex;
                }
            }
        }

        public void Alterar(T entidade)
        {
            using (var trans = Session.BeginTransaction())
            {
                try
                {
                    //entidade.DataAlteracao = DateTime.Now;
                    this.Session.Merge<T>(entidade);
                    trans.Commit();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw ex;
                }
            }

        }

        public void Excluir(T entidade)
        {
            using (var trans = Session.BeginTransaction())
            {
                try
                {
                    this.Session.Delete(entidade);
                    trans.Commit();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw ex;
                }
            }
        }

        public T ObterPorId(Int64 id)
        {
            var entidade = this.Session.Get<T>(id);
            return entidade;
        }

        public Expression<Func<T, bool>> CriarFiltro()
        {
            return PredicateBuilder.True<T>();
        }

        public IQueryable<T> ObterTodos()
        {
            var entidades = this
                .Session
                .CreateCriteria(typeof(T))
                .List<T>()
                .AsQueryable<T>();
            return entidades;
        }

        public IQueryable<T> ObterPorParametros(Expression<Func<T, bool>> predicado)
        {
            var a = from u in Session.Query<T>().Where(predicado)
                    select u;
            return a.AsQueryable<T>();
        }

        public Expression<Func<T, bool>> CriarPredicado()
        {
            return PredicateBuilder.True<T>();
        }

        public Int64 RetornaUltimoId() //retorna ultimo id da Tabela mas nao é o ultimo id Usado, ele nao pega da tabela uniquekey
        {
            var criterio = Session.CreateCriteria<T>().SetProjection(Projections.Max("Id"));
            try
            {
                return criterio.UniqueResult<Int64>();
            }
            catch
            {
                throw;
            }
        }

        public Int64 RetornaUltimoCodigo() //retorna ultimo codigo da Tabela
        {
            var criterio = Session.CreateCriteria<T>().SetProjection(Projections.Max("Codigo"));
            try
            {
                return criterio.UniqueResult<Int64>();
            }
            catch
            {
                throw;
            }
        }
    }
}
