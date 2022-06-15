using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

namespace LiteNetLibTest
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var mainWindow = new MainWindow();
                desktop.MainWindow = mainWindow;
                if (desktop.Args[0] == "--server")
                {
                    mainWindow.InitializeAsServer();
                }
                if (desktop.Args[0] == "--client")
                {
                    mainWindow.InitializeAsClient(
                        desktop.Args.Length == 2 ? desktop.Args[1] : "localhost");
                }
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
