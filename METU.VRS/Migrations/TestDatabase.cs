using METU.VRS.Services;
using System;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;


namespace METU.VRS.Migrations
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class TestDatabase
    {
        private readonly string LocalDbMaster = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True";
        private readonly string DatabaseName = "METUVRS_TEST";
        private readonly string DataDirectory = null;

        public TestDatabase()
        {
            DataDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }

        public static void InitTestDatabase()
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

        public static void WipeTestDatabase()
        {
            var testDatabase = new Migrations.TestDatabase();
            testDatabase.WipeDatabase(Seed: () =>
             {
                 var migrate = new Migrations.ConfigurationDefault();
                 var dbContext = new DatabaseContext();
                 migrate.SeedDB(dbContext);
                 dbContext.SaveChanges();
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

                CleanupDatabaseFiles();
                CreateNewDatabase(cmd);

                if (Seed != null)
                {
                    Seed.Invoke();
                    Console.Out.WriteLine("Test database is seeded");
                }
            }
        }

        public void WipeDatabase(Action Seed = null)
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
                        WipeDatabase(cmd);
                        Console.Out.WriteLine("Test database is wiped");
                    }
                    catch
                    {
                        Console.Out.WriteLine("Couldn't wiped the db");
                        return;
                    }
                }

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
                Console.Out.WriteLine($"--Couldn't detach the database from {attachedDB}--");
            }

            WipeDatabase(cmd);
        }

        private void WipeDatabase(SqlCommand cmd)
        {
            cmd.CommandText = $@"
                    USE {DatabaseName};
                    SET QUOTED_IDENTIFIER ON;
                    EXEC sp_MSforeachtable 'SET QUOTED_IDENTIFIER ON; ALTER TABLE ? NOCHECK CONSTRAINT ALL'  
                    EXEC sp_MSforeachtable 'SET QUOTED_IDENTIFIER ON; ALTER TABLE ? DISABLE TRIGGER ALL'  
                    EXEC sp_MSforeachtable 'SET QUOTED_IDENTIFIER ON; DELETE FROM ?'  
                    EXEC sp_MSforeachtable 'SET QUOTED_IDENTIFIER ON; ALTER TABLE ? CHECK CONSTRAINT ALL'  
                    EXEC sp_MSforeachtable 'SET QUOTED_IDENTIFIER ON; ALTER TABLE ? ENABLE TRIGGER ALL' 
                    EXEC sp_MSforeachtable 'SET QUOTED_IDENTIFIER ON;
                    
                    IF NOT EXISTS (
                        SELECT
                            *
                        FROM
                            SYS.IDENTITY_COLUMNS
                            JOIN SYS.TABLES ON SYS.IDENTITY_COLUMNS.Object_ID = SYS.TABLES.Object_ID
                        WHERE
                            SYS.TABLES.Object_ID = OBJECT_ID(''?'') AND SYS.IDENTITY_COLUMNS.Last_Value IS NULL
                    )
                    AND OBJECTPROPERTY( OBJECT_ID(''?''), ''TableHasIdentity'' ) = 1
                    
                        DBCC CHECKIDENT (''?'', RESEED, 0) WITH NO_INFOMSGS'";
            cmd.ExecuteNonQuery();
        }
        private void CleanupDatabaseFiles()
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
