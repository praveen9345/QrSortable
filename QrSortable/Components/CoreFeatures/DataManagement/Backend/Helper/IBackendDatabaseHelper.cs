namespace QrSortable.Components.CoreFeatures.DataManagement.Backend.Helper
{
    using QrSortable.Components.CoreFeatures.DataManagement.Backend.Models;
    using QrSortable.Components.CoreFeatures.DataManagement.Models;

    public interface IBackendDatabaseHelper
    {
        void SaveToTheBackendAsync(DatabaseEntry backendData);

        DtoStorageEntryModel CreateDtoStorageEntryBackendData(dynamic dataDec, string isUpdateData);

        DtoOrdersModel CreateDtoOrdersBackendData(dynamic dataDec, string isUpdateData);
    }
}
