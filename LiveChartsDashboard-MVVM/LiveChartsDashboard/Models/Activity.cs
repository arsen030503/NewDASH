using System;

namespace LiveChartsDashboard.Models
{
    // NOTE: the original project defined "Activity" twice (once in Models/Activity.cs,
    // once again inside DatabaseService.cs). Merged into a single definition here.
    public class Activity
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public double HoursWorked { get; set; }
        public string MachineName { get; set; } = string.Empty;

        // Fields used by the static "sample activities" list on the dashboard
        public string Name { get; set; } = string.Empty;
        public double Hours { get; set; }
        public string Category { get; set; } = string.Empty;
    }
}
