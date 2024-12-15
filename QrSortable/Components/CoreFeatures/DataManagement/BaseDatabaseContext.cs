namespace QrSortable.Components.CoreFeatures.DataManagement
{
    using Microsoft.EntityFrameworkCore;
    using SQLitePCL;

    /// <summary>
    ///     The base database context enabling the communication with the databases.
    /// </summary>
    public class BaseDatabaseContext : DbContext
    {
        private readonly string _databasePath;

        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseDatabaseContext" /> class.
        ///     Empty constructor used to load database migrations.
        /// </summary>
        public BaseDatabaseContext()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseDatabaseContext" /> class.
        /// </summary>
        /// <param name="databasePath">The path to be used for the location of the database.</param>
        public BaseDatabaseContext(string databasePath)
        {
            _databasePath = databasePath;
        }

        /// <summary>
        ///     <para>
        ///         Override this method to configure the database (and other options) to be used for this context.
        ///         This method is called for each instance of the context that is created. 
        ///         The base implementation does nothing.
        ///     </para>
        ///     <para>
        ///         In situations where an instance of <see cref="T:Microsoft.EntityFrameworkCore.DbContextOptions" /> may or may
        ///         not have been passed to the constructor, you can use
        ///         <see cref="P:Microsoft.EntityFrameworkCore.DbContextOptionsBuilder.IsConfigured" /> to determine if
        ///         the options have already been set, and skip some or all of the logic in
        ///         <see cref="M:Microsoft.EntityFrameworkCore.DbContext.OnConfiguring(Microsoft.EntityFrameworkCore.DbContextOptionsBuilder)" />.
        ///     </para>
        /// </summary>
        /// <param name="optionsBuilder">
        ///     A builder used to create or modify options for this context. Databases (and other extensions)
        ///     typically define extension methods on this object that allow you to configure the context.
        /// </param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            Batteries_V2.Init();

            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder
                    .UseSqlite($"Filename={_databasePath}");
            }
        }
    }
}
