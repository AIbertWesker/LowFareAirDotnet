using PostgresDbUp;

namespace Tool.DbUper
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var migrator = new PostgresDbUper(
                connectionString: "Host=localhost;Port=7004;Database=appdb;Username=appuser;Password=appsecret;Pooling=true;",
                scriptsPath: "scripts"
            );

            migrator.RunMigration();
        }
    }
}
