namespace QrSortable
{
    using Android.App;
    using Android.Content;
    using Android.Content.PM;
    using Android.OS;
    using Microsoft.Maui.Controls.PlatformConfiguration.AndroidSpecific;
    using QrSortable.Components.PlatformUtils;
    using QrSortable.Platforms.Android.Components.PlatformUtils;

    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, Exported = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]

    [IntentFilter(
        new[] { Intent.ActionView },
        Categories = new[]
        {
            Intent.CategoryDefault,
            Intent.CategoryBrowsable
        },
        DataScheme = "myapp",
        DataHost = "payment-return")
    ]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            if (Microsoft.Maui.Controls.Application.Current != null)
            {
                Microsoft.Maui.Controls.Application.Current
                    .On<Microsoft.Maui.Controls.PlatformConfiguration.Android>()
                    .UseWindowSoftInputModeAdjust(WindowSoftInputModeAdjust.Pan);
            }
        }

        // ✅ REQUIRED for your custom permission service
        public override void OnRequestPermissionsResult(
            int requestCode,
            string[] permissions,
            Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            var permissionService =
                MauiApplication.Current.Services
                    .GetService(typeof(IPermissionService))
                    as AndroidPermissionService;

            permissionService?.OnRequestPermissionsResult(
                requestCode,
                permissions,
                grantResults);
        }
    }
}