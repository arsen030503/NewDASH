namespace LiveChartsDashboard.Models
{
    public class WeatherData
    {
        public Main Main { get; set; } = new();
        public string Name { get; set; } = string.Empty;
    }

    public class Main
    {
        public double Temp { get; set; }
        public double Feels_Like { get; set; }
        public int Pressure { get; set; }
        public int Humidity { get; set; }
    }
}
