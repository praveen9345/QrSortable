namespace QrSortable.Components.PlatformUtils
{

    /// <summary>
    ///     This service helper class shall be used if it is not possible to inject the <see cref="IServiceProvider" /> in a
    ///     class there it is needed to resolve a specific service. Additionally it can be used in the platform specific code
    ///     to resolve the services of the app.
    /// </summary>
    public static class ServiceHelper
    {
        /// <summary>
        ///     This property contains the <see cref="IServiceProvider" /> of the <see cref="MauiApp" /> which needs to be set in
        ///     <see cref="Initialize" /> initially.
        /// </summary>
        public static IServiceProvider? Services { get; private set; }

        /// <summary>
        ///     This method needs to be called in <see cref="MauiProgram.CreateMauiApp" /> after the <see cref="MauiApp" /> has
        ///     been builded with the dedicated <see cref="MauiAppBuilder" />.
        /// </summary>
        public static void Initialize(IServiceProvider serviceProvider)
        {
            Services = serviceProvider;
        }

        /// <summary>
        /// Retrieves a registered service of the specified type from the application's service provider.
        /// </summary>
        /// <typeparam name="T">The type of service to retrieve. Must be a non-nullable type.</typeparam>
        /// <returns>The instance of the requested service.</returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the service provider has not been initialized by calling <see cref="Initialize(IServiceProvider)"/>.
        /// </exception>
        public static T GetService<T>() where T : notnull
        {
            if (Services == null)
                throw new InvalidOperationException("ServiceHelper is not initialized. Call Initialize() first.");

            return Services.GetRequiredService<T>();
        }
    }
}