using HeartCareAI.Models;
using HeartCareAI.Repositories;
using HeartCareAI.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace HeartCareAI.Controllers;

public class DashboardController : Controller
{
    private readonly IPredictionRepository _predictionRepository;

    public DashboardController(IPredictionRepository predictionRepository)
    {
        _predictionRepository = predictionRepository;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var predictions = await _predictionRepository.GetAllAsync();
        var predictionList = predictions.ToList();
        var total = predictionList.Count;
        var highRiskCount = predictionList.Count(item => item.RiskLevel == RiskLevel.High);

        var viewModel = new DashboardViewModel
        {
            TotalAssessments = total,
            LowRiskCount = predictionList.Count(item => item.RiskLevel == RiskLevel.Low),
            MediumRiskCount = predictionList.Count(item => item.RiskLevel == RiskLevel.Medium),
            HighRiskCount = highRiskCount,
            HighRiskRate = total == 0 ? 0 : Math.Round(highRiskCount * 100d / total, 1),
            AverageCholesterol = total == 0 ? 0 : Math.Round(predictionList.Average(item => item.Cholesterol), 1),
            AverageSystolicBloodPressure = total == 0 ? 0 : Math.Round(predictionList.Average(item => item.SystolicBloodPressure), 1),
            AverageRiskProbability = total == 0 ? 0 : Math.Round(predictionList.Average(item => item.RiskProbability), 1),
            SmokerCount = predictionList.Count(item => item.IsSmoker),
            HighBloodSugarCount = predictionList.Count(item => item.FastingBloodSugarHigh),
            AbnormalEcgCount = predictionList.Count(item => item.EcgResult != "Normal"),
            RiskDistribution = CreateRiskDistribution(predictionList),
            AgeDistribution = CreateAgeDistribution(predictionList),
            CholesterolByAge = CreateAverageByAge(predictionList, item => item.Cholesterol, "#0d9488"),
            BloodPressureByAge = CreateAverageByAge(predictionList, item => item.SystolicBloodPressure, "#2563eb"),
            GenderDistribution = CreateGenderDistribution(predictionList),
            EcgDistribution = CreateEcgDistribution(predictionList),
            SmokingDistribution = CreateBooleanDistribution(predictionList, item => item.IsSmoker, "Có hút thuốc", "Không hút", "#ef4444", "#10b981"),
            BloodSugarDistribution = CreateBooleanDistribution(predictionList, item => item.FastingBloodSugarHigh, "> 120 mg/dL", "≤ 120 mg/dL", "#f59e0b", "#10b981"),
            RiskTrend = CreateRiskTrend(predictionList),
            ProbabilityDistribution = CreateProbabilityDistribution(predictionList),
            RecentPredictions = predictionList.Take(8).ToList()
        };

        return View(viewModel);
    }

    private static IReadOnlyList<DashboardChartPoint> CreateRiskDistribution(List<Prediction> predictions)
    {
        return
        [
            new() { Label = "Thấp", Value = predictions.Count(item => item.RiskLevel == RiskLevel.Low), Color = "#10b981" },
            new() { Label = "Trung bình", Value = predictions.Count(item => item.RiskLevel == RiskLevel.Medium), Color = "#f59e0b" },
            new() { Label = "Cao", Value = predictions.Count(item => item.RiskLevel == RiskLevel.High), Color = "#ef4444" }
        ];
    }

    private static IReadOnlyList<DashboardChartPoint> CreateAgeDistribution(List<Prediction> predictions)
    {
        return GetAgeBuckets()
            .Select(bucket => new DashboardChartPoint
            {
                Label = bucket.Label,
                Value = predictions.Count(item => bucket.Predicate(item.Patient.Age)),
                Color = bucket.Color
            })
            .ToList();
    }

    private static IReadOnlyList<DashboardChartPoint> CreateAverageByAge(
        List<Prediction> predictions,
        Func<Prediction, double> selector,
        string color)
    {
        return GetAgeBuckets()
            .Select(bucket =>
            {
                var items = predictions.Where(item => bucket.Predicate(item.Patient.Age)).ToList();

                return new DashboardChartPoint
                {
                    Label = bucket.Label,
                    Value = items.Count == 0 ? 0 : Math.Round(items.Average(selector), 1),
                    Color = color
                };
            })
            .ToList();
    }

    private static IReadOnlyList<DashboardChartPoint> CreateGenderDistribution(List<Prediction> predictions)
    {
        return predictions
            .GroupBy(item => item.Patient.Gender)
            .Select((group, index) => new DashboardChartPoint
            {
                Label = group.Key,
                Value = group.Count(),
                Color = index % 2 == 0 ? "#2563eb" : "#0d9488"
            })
            .ToList();
    }

    private static IReadOnlyList<DashboardChartPoint> CreateEcgDistribution(List<Prediction> predictions)
    {
        var labels = new Dictionary<string, string>
        {
            ["Normal"] = "Bình thường",
            ["STT Abnormality"] = "ST-T",
            ["Left Ventricular Hypertrophy"] = "Phì đại thất trái"
        };

        return labels
            .Select((item, index) => new DashboardChartPoint
            {
                Label = item.Value,
                Value = predictions.Count(prediction => prediction.EcgResult == item.Key),
                Color = index switch
                {
                    0 => "#10b981",
                    1 => "#f59e0b",
                    _ => "#ef4444"
                }
            })
            .ToList();
    }

    private static IReadOnlyList<DashboardChartPoint> CreateBooleanDistribution(
        List<Prediction> predictions,
        Func<Prediction, bool> selector,
        string trueLabel,
        string falseLabel,
        string trueColor,
        string falseColor)
    {
        return
        [
            new() { Label = trueLabel, Value = predictions.Count(selector), Color = trueColor },
            new() { Label = falseLabel, Value = predictions.Count(item => !selector(item)), Color = falseColor }
        ];
    }

    private static IReadOnlyList<DashboardChartPoint> CreateRiskTrend(List<Prediction> predictions)
    {
        return predictions
            .OrderBy(item => item.CreatedAt)
            .GroupBy(item => item.CreatedAt.Date)
            .Select(group => new DashboardChartPoint
            {
                Label = group.Key.ToString("dd/MM"),
                Value = Math.Round(group.Average(item => item.RiskProbability), 1),
                Color = "#2563eb"
            })
            .TakeLast(7)
            .ToList();
    }

    private static IReadOnlyList<DashboardChartPoint> CreateProbabilityDistribution(List<Prediction> predictions)
    {
        return
        [
            new() { Label = "0-25%", Value = predictions.Count(item => item.RiskProbability <= 25), Color = "#10b981" },
            new() { Label = "26-50%", Value = predictions.Count(item => item.RiskProbability is > 25 and <= 50), Color = "#0d9488" },
            new() { Label = "51-75%", Value = predictions.Count(item => item.RiskProbability is > 50 and <= 75), Color = "#f59e0b" },
            new() { Label = "76-100%", Value = predictions.Count(item => item.RiskProbability > 75), Color = "#ef4444" }
        ];
    }

    private static IReadOnlyList<AgeBucket> GetAgeBuckets()
    {
        return
        [
            new("<40", age => age < 40, "#38bdf8"),
            new("40-49", age => age is >= 40 and <= 49, "#0d9488"),
            new("50-59", age => age is >= 50 and <= 59, "#f59e0b"),
            new("60+", age => age >= 60, "#ef4444")
        ];
    }

    private sealed record AgeBucket(string Label, Func<int, bool> Predicate, string Color);
}
