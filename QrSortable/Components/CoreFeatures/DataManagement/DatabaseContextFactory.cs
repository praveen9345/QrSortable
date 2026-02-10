namespace QrSortable.Components.CoreFeatures.DataManagement
{
    using Microsoft.EntityFrameworkCore.Design;
    using QrSortable.Components.CoreFeatures.DataManagement.General;
    using QrSortable.Components.CoreFeatures.DataManagement.Backend;

    /// <summary>
    /// Design-time factory for DatabaseContext - used by EF Core tools
    /// </summary>
    public class DatabaseContextFactory : IDesignTimeDbContextFactory<DatabaseContext>
    {
        public DatabaseContext CreateDbContext(string[] args)
        {
            // Use a temporary path for design-time operations
            var tempPath = Path.Combine(Path.GetTempPath(), "QrSortable_DesignTime.sqlite3");
            return new DatabaseContext(tempPath);
        }
    }

    /// <summary>
    /// Design-time factory for BackendDatabaseContext - used by EF Core tools
    /// </summary>
    public class BackendDatabaseContextFactory : IDesignTimeDbContextFactory<BackendDatabaseContext>
    {
        public BackendDatabaseContext CreateDbContext(string[] args)
        {
            var tempPath = Path.Combine(Path.GetTempPath(), "QrSortableBackend_DesignTime.sqlite3");
            return new BackendDatabaseContext(tempPath);
        }
    }
}