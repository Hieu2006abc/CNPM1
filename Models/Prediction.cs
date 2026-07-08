namespace HeartCareAI.Models;

public class Prediction
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Patient Patient { get; set; } = new();

    public int SystolicBloodPressure { get; set; }

    public int Cholesterol { get; set; }

    public bool FastingBloodSugarHigh { get; set; }

    public string EcgResult { get; set; } = "Normal";

    public bool IsSmoker { get; set; }

    public string? ChestPainType { get; set; }

    public int? MaxHeartRate { get; set; }

    public bool ExerciseAngina { get; set; }

    public double? StDepression { get; set; }

    public double RiskProbability { get; set; }

    public RiskLevel RiskLevel { get; set; }

    public string Recommendation { get; set; } = string.Empty;

    public List<string> InfluentialFactors { get; set; } = [];

    public List<RiskContribution> RiskContributions { get; set; } = [];

    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
