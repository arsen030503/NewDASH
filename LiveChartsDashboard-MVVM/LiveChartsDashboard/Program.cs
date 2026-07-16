using Avalonia;
using Avalonia.ReactiveUI;
using System;
using System.IO;

namespace LiveChartsDashboard
{
    class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            InitializeDatabase();

            BuildAvaloniaApp()
                .StartWithClassicDesktopLifetime(args, ShutdownMode.OnMainWindowClose);
        }

        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .WithInterFont()
                .LogToTrace()
                .UseReactiveUI();

        private static void InitializeDatabase()
        {
            var databasePath = Path.Combine(AppContext.BaseDirectory, "database.db");
            DatabaseInitializer.InitializeDatabase(databasePath);
            Console.WriteLine("Инициализация базы данных завершена.");
        }
    }
}
