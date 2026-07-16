using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using LiveChartsDashboard.Models;
using LiveChartsDashboard.Services;

namespace LiveChartsDashboard.ViewModels
{
    // partial + ObservableObject/[ObservableProperty]/[RelayCommand] are CommunityToolkit.Mvvm
    // source generators: they generate the INotifyPropertyChanged boilerplate and the
    // ICommand-backed command properties (GetWeatherCommand) at compile time.
    public partial class MainWindowViewModel : ObservableObject
    {
        private readonly WeatherService _weatherService;
        private readonly DatabaseService _databaseService;

        // ---- Chart data (unchanged from the original static setup) ----
        public ObservableCollection<ISeries> OperatingSystemsSeries { get; }
        public ObservableCollection<ISeries> DailyActivitySeries { get; }
        public ObservableCollection<ISeries> WeekdaySeries { get; }
        public ObservableCollection<Axis> WeekdayAxes { get; }
        public ObservableCollection<ISeries> MachineSeries { get; }
        public ObservableCollection<ISeries> Series { get; } = new();
        public ObservableCollection<Axis> XAxes { get; } = new();
        public ObservableCollection<Activity> Activities { get; } = new();

        // ---- Weather form state ----
        // [ObservableProperty] generates a public "CityInput" property (PascalCase)
        // that raises PropertyChanged automatically when set from the view's binding.
        [ObservableProperty]
        private string cityInput = string.Empty;

        [ObservableProperty]
        private string cityName = "City: ";

        [ObservableProperty]
        private string temperature = "Temperature: ";

        [ObservableProperty]
        private string pressure = "Pressure: ";

        [ObservableProperty]
        private string humidity = "Humidity: ";

        [ObservableProperty]
        private string statusMessage = string.Empty;

        [ObservableProperty]
        private bool isBusy;

        // Parameterless constructor for the view / XAML designer.
        public MainWindowViewModel() : this(new WeatherService(), new DatabaseService())
        {
        }

        // Constructor that takes services as parameters — this is what makes the
        // ViewModel unit-testable: a test can pass in fakes/mocks for both services
        // instead of hitting the real HTTP API / SQLite file.
        public MainWindowViewModel(WeatherService weatherService, DatabaseService databaseService)
        {
            _weatherService = weatherService;
            _databaseService = databaseService;

            OperatingSystemsSeries = new ObservableCollection<ISeries>
            {
                new PieSeries<double> { Values = new double[] { 70 }, Name = "Windows", Fill = new SolidColorPaint(SKColors.Blue) },
                new PieSeries<double> { Values = new double[] { 30 }, Name = "Linux", Fill = new SolidColorPaint(SKColors.Green) }
            };

            DailyActivitySeries = new ObservableCollection<ISeries>
            {
                new PieSeries<double> { Values = new double[] { 64 }, Name = "Today", Fill = new SolidColorPaint(SKColors.Orange) }
            };

            WeekdaySeries = new ObservableCollection<ISeries>
            {
                new ColumnSeries<double>
                {
                    Values = new double[] { 2, 3, 1, 4, 2, 1, 3 },
                    Name = "Activity",
                    Fill = new SolidColorPaint(SKColors.Blue)
                }
            };

            WeekdayAxes = new ObservableCollection<Axis>
            {
                new Axis { Labels = new[] { "Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun" } }
            };

            MachineSeries = new ObservableCollection<ISeries>
            {
                new PieSeries<double> { Values = new double[] { 100 }, Name = "bi-n230719-01", Fill = new SolidColorPaint(SKColors.Blue) }
            };

            Activities.Add(new Activity { Name = "Coding", Hours = 5.0, Category = "Work" });
            Activities.Add(new Activity { Name = "Meeting", Hours = 2.0, Category = "Work" });
            Activities.Add(new Activity { Name = "Gaming", Hours = 1.5, Category = "Leisure" });

            // Fire-and-forget on purpose: constructors can't be async. In a bigger app
            // this would move to an explicit InitializeAsync() called from the view.
            _ = LoadChartDataAsync();
        }

        private async Task LoadChartDataAsync()
        {
            try
            {
                var activities = await _databaseService.GetActivitiesAsync();

                var groupedData = activities
                    .GroupBy(a => a.Date.Date)
                    .Select(g => new { Date = g.Key, HoursWorked = g.Sum(a => a.HoursWorked) })
                    .OrderBy(g => g.Date)
                    .ToList();

                var hoursWorkedValues = groupedData.Select(g => g.HoursWorked).ToList();
                var labels = groupedData.Select(g => g.Date.ToShortDateString()).ToArray();

                Series.Add(new ColumnSeries<double> { Values = hoursWorkedValues, Name = "Hours Worked" });
                XAxes.Add(new Axis { Labels = labels });
            }
            catch (Exception ex)
            {
                StatusMessage = $"Failed to load activity data: {ex.Message}";
            }
        }

        // [RelayCommand] generates a public ICommand property called "GetWeatherCommand"
        // that the view binds to via Command="{Binding GetWeatherCommand}" — this replaces
        // the old Click="GetWeatherButton_Click" code-behind event handler.
        [RelayCommand]
        private async Task GetWeatherAsync()
        {
            if (string.IsNullOrWhiteSpace(CityInput))
            {
                StatusMessage = "Please enter a city name.";
                return;
            }

            IsBusy = true;
            StatusMessage = string.Empty;

            try
            {
                var weather = await _weatherService.GetWeatherAsync(CityInput);
                CityName = $"City: {weather.Name}";
                Temperature = $"Temperature: {weather.Main.Temp} \u00b0C";
                Pressure = $"Pressure: {weather.Main.Pressure} hPa";
                Humidity = $"Humidity: {weather.Main.Humidity}%";
            }
            catch
            {
                StatusMessage = "Failed to fetch weather data. Please check the city name or try again later.";
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
