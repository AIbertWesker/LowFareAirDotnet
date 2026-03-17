using PostgresDbUp;

namespace Tool.DbUper
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var migrator = new PostgresDbUper(
                connectionString: "Host=localhost;Port=7004;Database=lowfareair;Username=postgres;Password=OtworSpecjalista666;Pooling=true;",
                scriptsPath: "scripts"
            );

            migrator.RunMigration();
        }
    }
}
