using HeartCareAI.Models;

namespace HeartCareAI.ViewModels;

public class DashboardViewModel
{
    public int TotalAssessments { get; set; }

    public int LowRiskCount { get; set; }

    public int MediumRiskCount { get; set; }

    public int HighRiskCount { get; set; }

    public double HighRiskRate { get; set; }

    public double AverageCholesterol { get; set; }

    public double AverageSystolicBloodPressure { get; set; }

    public double AverageRiskProbability { get; set; }

    public int SmokerCount { get; set; }

    public int HighBloodSugarCount { get; set; }

    public int AbnormalEcgCount { get; set; }

    public IReadOnlyList<DashboardChartPoint> RiskDistribution { get; set; } = [];

    public IReadOnlyList<DashboardChartPoint> AgeDistribution { get; set; } = [];

    public IReadOnlyList<DashboardChartPoint> CholesterolByAge { get; set; } = [];

    public IReadOnlyList<DashboardChartPoint> BloodPressureByAge { get; set; } = [];

    public IReadOnlyList<DashboardChartPoint> GenderDistribution { get; set; } = [];

    public IReadOnlyList<DashboardChartPoint> EcgDistribution { get; set; } = [];

    public IReadOnlyList<DashboardChartPoint> SmokingDistribution { get; set; } = [];

    public IReadOnlyList<DashboardChartPoint> BloodSugarDistribution { get; set; } = [];

    public IReadOnlyList<DashboardChartPoint> RiskTrend { get; set; } = [];

    public IReadOnlyList<DashboardChartPoint> ProbabilityDistribution { get; set; } = [];

    public IReadOnlyList<Prediction> RecentPredictions { get; set; } = [];
}

public class DashboardChartPoint
{
    public string Label { get; set; } = string.Empty;

    public double Value { get; set; }

    public string Color { get; set; } = "#0d9488";
}
