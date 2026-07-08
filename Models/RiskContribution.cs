namespace HeartCareAI.Models;

public class RiskContribution
{
    public string Factor { get; set; } = string.Empty;

    public double Score { get; set; }

    public double MaxScore { get; set; }

    public string Level { get; set; } = "Ổn định";

    public string Description { get; set; } = string.Empty;

    public int Percent => MaxScore <= 0 ? 0 : (int)Math.Round(Math.Clamp(Score / MaxScore, 0, 1) * 100);

    public string CssClass => Score switch
    {
        >= 14 => "risk-high",
        >= 8 => "risk-medium",
        _ => "risk-low"
    };
}
