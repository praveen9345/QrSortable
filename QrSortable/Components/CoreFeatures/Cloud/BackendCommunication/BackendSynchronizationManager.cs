namespace QrSortable.Components.CoreFeatures.Cloud.BackendCommunication
{
    using Microsoft.EntityFrameworkCore;
    using QrSortable.Components.CoreFeatures.DataManagement.Backend;
    using QrSortable.Components.CoreFeatures.DataManagement.Backend.Models;
    using QrSortable.Components.CoreFeatures.DataManagement.General;
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Connectivity-aware queue + persistent retry.
    /// Uploads persisted queue entries on startup/resume and removes them from DB when uploaded.
    /// </summary>
    public class BackendSynchronizationManager : IBackendSynchronizationManager
    {
        private readonly IBackendCommunicationService _backendCommunicationService;
        private readonly IConnectivityService _connectivityService;
        private readonly IBackendDatabaseManager _backendDatabaseManager;
        private readonly IGeneralInformationManager _generalInformationManager;

        public BackendSynchronizationManager(
            IBackendCommunicationService backendCommunicationService,IConnectivityService connectivityService,
            IGeneralInformationManager generalInformationManager, IBackendDatabaseManager backendDatabaseManager)
        {
            _backendCommunicationService = backendCommunicationService;
            _backendDatabaseManager = backendDatabaseManager;
            _connectivityService = connectivityService;
            _generalInformationManager = generalInformationManager;

            _connectivityService.InternetConnectivityChanged += ConnectivityServiceOnInternetConnectivityChanged;
        }


        public async Task<bool> SynchronizeStoredObjectsAsync()
        {
         
            var isInternetConnectionAvailable = await _connectivityService.CheckInternetConnectionAvailableAsync();
            if (!isInternetConnectionAvailable)
            {
                return false;
            }

            try
            {
                var generalInformation = await _generalInformationManager.GetGeneralInformationAsync();

                //only sync storage entries if backend is used
                if (generalInformation.IsBackendUsed)
                {
                    var dbStoreEntries = await (await _backendDatabaseManager
                    .GetAllAsync<DtoStorageEntryModel>()).OrderBy(dto => dto.ID)
                    .ToListAsync();

                    if (dbStoreEntries.Any())
                    {
                        foreach (var dto in dbStoreEntries)
                        {
                            if(dto.IsUpdateData == "true")
                            { 
                                var result = await _backendCommunicationService.UpdateAsync(dto, true);

                                if (result)
                                {
                                    await _backendDatabaseManager.DeleteAsync(dto);
                                }
                            }
                            else if (dto.IsUpdateData == "false")
                            {
                                var result = await _backendCommunicationService.InsertAsync(dto, true);
                                if (result)
                                {
                                    await _backendDatabaseManager.DeleteAsync(dto);
                                }
                            }
                            else if (dto.IsUpdateData == "delete")
                            {
                                var result = await _backendCommunicationService.DeleteAsync(dto, true);

                                if (result)
                                {
                                    await _backendDatabaseManager.DeleteAsync(dto);
                                }
                            }

                        }
                    }
                }

                var orderedEntries = await (await _backendDatabaseManager
                .GetAllAsync<DtoOrdersModel>()).OrderBy(dto => dto.ID)
                .ToListAsync();

                if (orderedEntries.Any())
                {
                    foreach (var dto in orderedEntries)
                    {
                        if (dto.IsUpdateData == "true")
                        {
                            var result = await _backendCommunicationService.UpdateAsync(dto, true);
                            if (result)
                            {
                                await _backendDatabaseManager.DeleteAsync(dto);
                            }
                        }
                        else if (dto.IsUpdateData == "false")
                        {
                            var result = await _backendCommunicationService.InsertAsync(dto, true);
                            if (result)
                            {
                                await _backendDatabaseManager.DeleteAsync(dto);
                            }
                        }
                    }
                }

                var subscriptionEntries = await (await _backendDatabaseManager
                .GetAllAsync<DtoSubscriptionEntityModel>()).OrderBy(dto => dto.ID)
                .ToListAsync();

                if (subscriptionEntries.Any())
                {
                    foreach (var dto in subscriptionEntries)
                    {
                        if (dto.IsUpdateData == "true")
                        {
                            var result = await _backendCommunicationService.UpdateAsync(dto, true);
                            if (result)
                            {
                                await _backendDatabaseManager.DeleteAsync(dto);
                            }
                        }
                        else if (dto.IsUpdateData == "false")
                        {
                            var result = await _backendCommunicationService.InsertAsync(dto, true);
                            if (result)
                            {
                                await _backendDatabaseManager.DeleteAsync(dto);
                            }
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error during backend synchronization: " + ex.Message);
                return false;
            }
           
        }

        /// <summary>
        ///     Trigger the synchronization of objects that could not have been synced before as soon as internet connection exists.
        /// </summary>
        private async void ConnectivityServiceOnInternetConnectivityChanged(object sender, EventArgs e)
        {
            var isInternetConnectionAvailable = await _connectivityService.CheckInternetConnectionAvailableAsync();
            if (!isInternetConnectionAvailable)
            {
                return;
            }

            await SynchronizeStoredObjectsAsync();
        }
    }

}