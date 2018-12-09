using METU.VRS.Services;
using System;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;


namespace METU.VRS.Migrations
{
    public class TestDatabase
    {
        private readonly string LocalDbMaster = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True";
        private readonly string DatabaseName = "METUVRS_TEST";
        private readonly string DataDirectory = null;

        public TestDatabase()
        {
            DataDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }

        public static void InitTestDatase()
        {
            var testDatabase = new Migrations.TestDatabase();
            testDatabase.CreateOrRefreshDatabase(Seed: () =>
            {
                var migrate = new MigrateDatabaseToLatestVersion<DatabaseContext, Migrations.ConfigurationDefault>();
                var dbContext = new DatabaseContext();
                migrate.InitializeDatabase(dbContext);
            });
            testDatabase.SetAppDomainDataDirectory();

        }

        public void SetAppDomainDataDirectory()
        {
            AppDomain.CurrentDomain.SetData("DataDirectory", DataDirectory);
        }

        public void CreateOrRefreshDatabase(Action Seed = null)
        {
            using (var connection = new SqlConnection(LocalDbMaster))
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandTimeout = 5;

                if (FindDatabase(cmd))
                {
                    try
                    {
                        DetachDatabase(cmd);
                    }
                    catch
                    {
                        TryReuseDatabase(cmd);
                        return;
                    }
                }

                CleanupDatabase();
                CreateNewDatabase(cmd);

                if (Seed != null)
                {
                    Seed.Invoke();
                    Console.Out.WriteLine("Test database is seeded");
                }
            }
        }

        private void CreateNewDatabase(SqlCommand cmd)
        {
            cmd.CommandText = string.Format("CREATE DATABASE {0} ON (NAME = N'{0}', FILENAME = '{1}\\{0}.mdf')", DatabaseName, DataDirectory);
            cmd.ExecuteNonQuery();
            Console.Out.WriteLine("A new test database is created");
        }
        private void DetachDatabase(SqlCommand cmd)
        {
            cmd.CommandText = $"exec sp_detach_db '{DatabaseName}'";
            cmd.ExecuteNonQuery();
        }
        private void TryReuseDatabase(SqlCommand cmd)
        {
            cmd.CommandText = $"SELECT filename FROM sys.sysdatabases WHERE name='{DatabaseName}'";
            string attachedDB = Convert.ToString(cmd.ExecuteScalar());
            if ($"{DataDirectory}\\{DatabaseName}.mdf" == attachedDB)
            {
                Console.Out.WriteLine($"--Couldn't detach database {DatabaseName}--");
            }
            else
            {
                throw new Exception($"--Couldn't detach the database from {attachedDB}--");
            }
        }
        private void CleanupDatabase()
        {
            try
            {
                if (File.Exists($"{DataDirectory}\\{DatabaseName}.mdf"))
                {
                    File.Delete($"{DataDirectory}\\{DatabaseName}.mdf");
                }

                if (File.Exists($"{DataDirectory}\\{DatabaseName}_log.ldf"))
                {
                    File.Delete($"{DataDirectory}\\{DatabaseName}_log.ldf");
                }

                Console.Out.WriteLine("Test database files are deleted");
            }
            catch
            {
                Console.Out.WriteLine("Could not delete the test database files");
            }
        }
        private bool FindDatabase(SqlCommand cmd)
        {
            cmd.CommandText = string.Format($"SELECT COUNT(*) FROM sys.sysdatabases WHERE name='{DatabaseName}'");
            return (1 == (int)cmd.ExecuteScalar());
        }
    }
}
