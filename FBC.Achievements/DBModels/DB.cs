using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace FBC.Achievements.DBModels
{
    public class DB : DbContext
    {
        public DbSet<DBUser> Users { get; set; }
        public DbSet<DBAchievement> Achievements { get; set; }
        public DbSet<DBUserAchievement> UserAchievements { get; set; }



        public string DbPath { get; }
        const string DATABASE_FILE_NAME = "FBC.Achievements.db";
        public DB()
        {
            string dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Database");
            DbPath = Path.Combine(dir, DATABASE_FILE_NAME);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
        }

        public static void MigrateDB()
        {
            using (var db = new DB())
            {
                try
                {
                    Console.WriteLine("Begin Migrate");

                    db.Database.Migrate();

                    if (!db.Users.Any(x => x.UserType == UserType.Admin))
                    {
                        Console.WriteLine("Create Admin User");
                        db.Users.Add(new DBUser()
                        {
                            UserType = UserType.Admin,
                            UserName = "admin",
                            Password = C.Tools.ToMD5("admin"),
                            FullName = "Administrator",
                        });
                        db.SaveChanges();
                        Console.WriteLine("Default Admin User Created with UserName: admin, Password: admin");
                    }

                    Console.WriteLine("End Migrate");

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Migrate: " + ex.Message);
                    //Console.WriteLine(ex.StackTrace);
                }
                finally
                {

                }
            }
        }

        // The following configures EF to create a Sqlite database file in the
        // special "local" folder for your platform.
        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite($"Data Source={DbPath}");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {


            base.OnModelCreating(modelBuilder);
            //modelBuilder.Entity<DBAchievement>()
            //    .Property(a => a.Title)
            //    .HasConversion(
            //     v => string.IsNullOrWhiteSpace(v) ? null : v.Trim(), // C# → DB
            //     v => string.IsNullOrWhiteSpace(v) ? null : v.Trim()  // DB → C#
            //    )
            //    //.HasMaxLength(100)
            //    .IsRequired();

        }



        #region Backup
        public string SaveChangesAndBackup()
        {
            SaveChanges();

            ExecuteCheckpointAndClose();
            return CreateBackup();
        }
        private void ExecuteCheckpointAndClose()
        {
            using (var connection = Database.GetDbConnection())
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    // Set the database to single user mode
                    //Sanıyorum ki .db-shm .db-val dosyalarını bu siliyor.
                    command.CommandText = "PRAGMA journal_mode = DELETE;";
                    command.ExecuteNonQuery();

                    // Perform a checkpoint to ensure all changes are written
                    command.CommandText = "PRAGMA wal_checkpoint(FULL);";
                    command.ExecuteNonQuery();

                    command.CommandText = "PRAGMA wal_checkpoint(TRUNCATE);";
                    command.ExecuteNonQuery();

                    command.CommandText = "VACUUM;";
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }

            // Ensure all finalizers have run and resources are released
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        private string CreateBackup()
        {
            string backupDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DatabaseBackups");
            if (!Directory.Exists(backupDir))
            {
                Directory.CreateDirectory(backupDir);
            }

            string backupPath = Path.Combine(backupDir, $"{Path.GetFileNameWithoutExtension(DATABASE_FILE_NAME)}_backup_{DateTime.Now:yyyyMMddHHmmss}.db");
            File.Copy(DbPath, backupPath, true);
            return backupPath;
        }
        #endregion
    }
}
