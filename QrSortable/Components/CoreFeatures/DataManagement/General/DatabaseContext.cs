namespace QrSortable.Components.CoreFeatures.DataManagement.General
{
    using Microsoft.EntityFrameworkCore;
    using Models;

    /// <summary>
    ///     The database context enabling the communication with the general database.
    /// </summary>
    public class DatabaseContext : BaseDatabaseContext
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="DatabaseContext" /> class.
        ///     Empty constructor used to load database migrations.
        /// </summary>
        public DatabaseContext() : base()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="DatabaseContext" /> class.
        /// </summary>
        /// <param name="databasePath">The path to be used for the location of the database.</param>
        public DatabaseContext(string databasePath) : base(databasePath)
        {
        }

        /// <summary>
        ///     Gets or sets the general information of the database.
        /// </summary>
        public DbSet<GeneralInformation> GeneralInformation { get; set; }
        
        /// <summary>
        ///     Gets or sets the user information in the database.
        /// </summary>
        public DbSet<UserInfos> UserInformation { get; set; }

        /// <summary>
        ///      Method called implicitly when the model is being created. It registers entity property
        ///     conversions.
        /// </summary>
        /// <param name="modelBuilder">The instance used for building the model.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
          
        }
    }
}