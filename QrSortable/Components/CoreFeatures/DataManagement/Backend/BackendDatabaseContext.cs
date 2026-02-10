namespace QrSortable.Components.CoreFeatures.DataManagement.Backend
{
    using Microsoft.EntityFrameworkCore;
    using System.Runtime.CompilerServices;
    using QrSortable.Components.CoreFeatures.DataManagement.Backend.Models;

    /// <summary>
    ///     The database context enabling the communication with the database used for backend synchronization.
    /// </summary>
    public class BackendDatabaseContext : BaseDatabaseContext
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="BackendDatabaseContext" /> class.
        ///     Empty constructor used to load database migrations.
        /// </summary>
        public BackendDatabaseContext()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="BackendDatabaseContext" /> class.
        /// </summary>
        /// <param name="databasePath">The path to be used for the location of the database.</param>
        public BackendDatabaseContext(string databasePath) : base(databasePath)
        {
        }

        /// <summary>
        ///     Gets or sets the storage entiries of the database.
        /// </summary>
        public DbSet<DtoStorageEntryModel> StorageEntriesDto { get; set; }

        /// <summary>
        ///     Gets or sets the ordered of the database.
        /// </summary>
        public DbSet<DtoOrdersModel> OrderedDto { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            // ONLY use compiled model on iOS in NativeAOT mode
#if IOS
                 if (!RuntimeFeature.IsDynamicCodeSupported)
                     {
                         optionsBuilder.UseModel(
                                QrSortable.Components.CoreFeatures.DataManagement.Backend.CompiledModels.BackendDatabaseContextModel.Instance);
                      }
#endif
        }

    }

}