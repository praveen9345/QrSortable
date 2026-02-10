namespace QrSortable.Components.CoreFeatures.DataManagement.General
{
    using Models;
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

    /// <summary>
    ///     Provides logic for handling the general information entity.
    /// </summary>
    public class GeneralInformationManager : IGeneralInformationManager
    {
        private readonly IDatabaseManager _databaseManager;
        private GeneralInformation _generalInformation;
        private bool _wasGeneralInformationUpdated;

        /// <summary>
        ///     Initializes a new instance of the general information manager class.
        /// </summary>
        /// <param name="databaseManager">The database manager to use for database access.</param>
        public GeneralInformationManager(IDatabaseManager databaseManager)
        {
            _databaseManager = databaseManager;
        }

        /// <summary>
        ///     Gets the current general information entity.
        /// </summary>
        /// <returns>The current general information entity. This value is never null.</returns>
        /// <exception cref="CustomAttributeFormatException">If there are multiple entities in the database.</exception>
        public async Task<GeneralInformation> GetGeneralInformationAsync()
        {
            if (_generalInformation != null && !_wasGeneralInformationUpdated)
            {
                return _generalInformation;
            }

            _wasGeneralInformationUpdated = false;
            var generalInformationEntities = await _databaseManager.GetListAsync<GeneralInformation>();
            switch (generalInformationEntities.Count())
            {
                case 0:
                    _generalInformation = new GeneralInformation
                    {
                        NotificationPermissionStatus = PermissionStatus.Unknown
                    };
                    await _databaseManager.AddAsync(_generalInformation);
                    return _generalInformation;
                case 1:
                    _generalInformation = generalInformationEntities.Single();
                    return _generalInformation;
                default:
                    throw new CustomAttributeFormatException("There were multiple GeneralInformation entities in the database.");
            }
        }

        /// <summary>
        ///     Resets the general information saved in this class.
        /// </summary>
        public void ResetGeneralInformation()
        {
            _generalInformation = null;
            _wasGeneralInformationUpdated = true;
        }

        /// <summary>
        ///     Updates the status of the notification permission with a value.
        /// </summary>
        /// <param name="permissionStatus">The value indicating the current status of the permission.</param>
        /// <returns> True, if the setting was successful. False, otherwise. </returns>
        public async Task<bool> UpdateNotificationPermissionStatusAsync(PermissionStatus permissionStatus)
        {
            var generalInformation = await GetGeneralInformationAsync();
            Console.WriteLine($"Updating {nameof(GeneralInformation.NotificationPermissionStatus)} to state {Enum.GetName(typeof(PermissionStatus), permissionStatus)}");
            generalInformation.NotificationPermissionStatus = permissionStatus;

            return await UpdateGeneralInformationAsync(generalInformation);
        }

        /// <summary>
        ///     Updates the user's current OnboardingProgress.
        /// </summary>
        /// <param name="progress"> The user's current OnboardingProgress. </param>
        /// <returns> True, if the setting was successful. False, otherwise. </returns>
        public async Task<bool> UpdateOnboardingProgressAsync(OnboardingProgress progress)
        {
            var generalInformation = await GetGeneralInformationAsync();
            Console.WriteLine($"Updating {nameof(GeneralInformation.OnboardingProgress)} to state {Enum.GetName(typeof(OnboardingProgress), progress)}");
            generalInformation.OnboardingProgress = progress;

            return await UpdateGeneralInformationAsync(generalInformation);
        }

        
        public async Task<bool> UpdateTheMultiuserIdAsync(string multiuserId)
        {
            var generalInformation = await GetGeneralInformationAsync();

            if (generalInformation.MultiUserId == multiuserId)
            {
                return true;
            }

            Console.WriteLine($"Generating {nameof(GeneralInformation.MultiUserId)}");
            generalInformation.MultiUserId = multiuserId;

            return await UpdateGeneralInformationAsync(generalInformation);

        }

        /// <summary>
        ///     Updates whether the backend is used or not.
        /// </summary>
        /// <param name="isBackendUsed">The value indicating whether the backend is used or not.</param>
        /// <returns> True, if the update was successful. False, otherwise. </returns>
        public async Task<bool> UpdateIsBackendUsedAsync(bool isBackendUsed)
        {
            var generalInformation = await GetGeneralInformationAsync();

            Console.WriteLine($"Updating {nameof(GeneralInformation.IsBackendUsed)} to {isBackendUsed}");
            generalInformation.IsBackendUsed = isBackendUsed;

            return await UpdateGeneralInformationAsync(generalInformation);
        }

        private async Task<bool> UpdateGeneralInformationAsync(GeneralInformation generalInformation)
        {
            var updatedEntity = await _databaseManager.UpdateAsync(generalInformation);

            if (updatedEntity != null)
            {
                _wasGeneralInformationUpdated = true;
                return true;
            }

            return false;
        }

    }
}
