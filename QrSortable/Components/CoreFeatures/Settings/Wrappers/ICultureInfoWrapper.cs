namespace QrSortable.Components.CoreFeatures.Settings.Wrappers
{
    using System.Globalization;

    /// <summary>
    ///     Wrapper interface for CultureInfo.
    /// </summary>
    public interface ICultureInfoWrapper
    {

        /// <summary>
        ///     Gets the current culture Information.
        /// </summary>
        CultureInfo CurrentCulture { get; }

        /// <summary>
        ///     Gets the default UI culture Information.
        /// </summary>
        CultureInfo DefaultUiCultureInfo { get; }

        /// <summary>
        ///     Gets the default thread current culture Information.
        /// </summary>
        CultureInfo DefaultThreadCurrentCulture { get; }

        /// <summary>
        ///     Gets the default thread current UI culture Information.
        /// </summary>
        CultureInfo DefaultThreadCurrentUiCulture { get; }

        /// <summary>
        ///     The method to set the culture.
        /// </summary>
        /// <param name="cultureInfo">The culture info</param>
        void SetCulture(CultureInfo cultureInfo);

    }
}
