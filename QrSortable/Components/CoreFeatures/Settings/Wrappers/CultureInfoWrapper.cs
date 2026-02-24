namespace QrSortable.Components.CoreFeatures.Settings.Wrappers
{
    using System;
    using System.Globalization;

    /// <summary>
    ///     Implementation of the <see cref="ICultureInfoWrapper" />.
    /// </summary>
    public class CultureInfoWrapper : ICultureInfoWrapper
    {

        /// <summary>
        ///     Gets the current culture information.
        /// </summary>
        public CultureInfo CurrentCulture => CultureInfo.CurrentCulture;

        /// <summary>
        ///     Gets the default UI culture.
        /// </summary>
        public CultureInfo DefaultUiCultureInfo => CultureInfo.DefaultThreadCurrentUICulture ?? CultureInfo.CurrentUICulture;

        /// <summary>
        ///     Gets the default thread current culture.
        /// </summary>
        public CultureInfo DefaultThreadCurrentCulture => CultureInfo.DefaultThreadCurrentCulture;

        /// <summary>
        ///     Gets the default thread current UI culture.
        /// </summary>
        public CultureInfo DefaultThreadCurrentUiCulture => CultureInfo.DefaultThreadCurrentUICulture ?? CultureInfo.CurrentUICulture;

        /// <summary>
        ///     The method to set the culture.
        /// </summary>
        /// <param name="cultureInfo">The culture info</param>
        public void SetCulture(CultureInfo cultureInfo)
        {
            CultureInfo.CurrentCulture = cultureInfo ?? throw new ArgumentNullException(nameof(cultureInfo));
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
            CultureInfo.CurrentUICulture = cultureInfo;
        }
    }
}
