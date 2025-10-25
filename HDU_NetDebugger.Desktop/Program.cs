using System;
using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Fonts;

namespace HDU_NetDebugger.Desktop;

sealed class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args) => BuildAvaloniaApp()
        .StartWithClassicDesktopLifetime(args);

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .ConfigureFonts(fm => fm.AddFontCollection(
                new EmbeddedFontCollection(
                    new Uri("fonts:HarmonyOS_Sans_SC", UriKind.Absolute),
                    new Uri("avares://HDU_NetDebugger/Assets/HarmonyOS_Sans_SC", UriKind.Absolute))))
            .With(new FontManagerOptions
            {
                DefaultFamilyName = "fonts:HarmonyOS_Sans_SC#HarmonyOS Sans SC",
                FontFallbacks =
                [
                    new FontFallback { FontFamily = new FontFamily("fonts:HarmonyOS_Sans_SC#HarmonyOS Sans") }
                ]
            })
            .WithInterFont()
            .LogToTrace()
            .WithDeveloperTools();
}
