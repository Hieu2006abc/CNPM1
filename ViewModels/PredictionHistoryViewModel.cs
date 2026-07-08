using HeartCareAI.Models;

namespace HeartCareAI.ViewModels;

public class PredictionHistoryViewModel
{
    public string? Search { get; set; }

    public RiskLevel? RiskLevel { get; set; }

    public IReadOnlyList<Prediction> Predictions { get; set; } = [];
}
