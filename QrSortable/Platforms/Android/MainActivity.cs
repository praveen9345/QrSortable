namespace QrSortable
{
    using Android.App;
    using Android.Content.PM;
    using Android.OS;
    using Microsoft.Maui.Controls.PlatformConfiguration.AndroidSpecific;
    using Microsoft.Maui.Embedding;

    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        private MauiContext _mauiContext;

        /// <summary>
        ///     Called when the activity is starting. Used for handling notification clicks if the app is closed.
        /// </summary>
        /// <param name="savedInstanceState">The saved instance state.</param>
        protected override void OnCreate(Bundle? savedInstanceState)
        {
            Platform.Init(this, savedInstanceState);
            base.OnCreate(savedInstanceState);
            if (Microsoft.Maui.Controls.Application.Current != null)
                    Microsoft.Maui.Controls.Application.Current
                        .On<Microsoft.Maui.Controls.PlatformConfiguration.Android>()
                        .UseWindowSoftInputModeAdjust(WindowSoftInputModeAdjust.Pan);


            MauiAppBuilder builder = MauiApp.CreateBuilder();
            builder.UseMauiEmbedding<Microsoft.Maui.Controls.Application>();
            MauiApp mauiApp = builder.Build();
            _mauiContext = new MauiContext(mauiApp.Services, this);
        }
    }
}