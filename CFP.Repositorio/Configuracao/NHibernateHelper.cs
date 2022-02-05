using CFP.Ferramentas;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NHibernate.Tool.hbm2ddl;
using Repositorio.Mapeamentos;

namespace SGE.Repositorio.Configuracao
{
    public class NHibernateHelper
    {
        private static ISessionFactory _sessionFactory;
        private static Configuration cfg = new Configuration();


        public static ISession GetSession(bool revalidar = false)
        {
            if (_sessionFactory == null || revalidar)
            {
                var configure = new Configuration();
                configure.DataBaseIntegration(db =>
                {
                    db.Dialect<NHibernate.Dialect.MySQL55InnoDBDialect>();
                    db.Driver<NHibernate.Driver.MySqlDataDriver>();
                    db.ConnectionString = ArquivosXML.StringConexao();
                        //db.ConnectionString = "Server=localhost;Port=3306;Database=cfp;Uid=root;Pwd=123456;";
                        db.Timeout = 10;

                    db.LogFormattedSql = false;
                    db.LogSqlInConsole = false;
                    db.AutoCommentSql = false;
                });

                var mapper = new ModelMapper();
                mapper.AddMappings(typeof(BancoMAP).Assembly.GetTypes());

                HbmMapping mapping = mapper.CompileMappingForAllExplicitlyAddedEntities();
                configure.AddMapping(mapping);

                _sessionFactory = configure.BuildSessionFactory();
               //BuildSchema(configure);
            }
            return _sessionFactory.OpenSession();
        }

        private NHibernateHelper()
        {

        }

        private static void BuildSchema(Configuration config)
        {
            new SchemaExport(config).SetOutputFile(@"D:\Projetos\ControleFinanceiroPessoal\Doc\cfp.sql").SetDelimiter(";").Create(false, false);
        }


    }
}
