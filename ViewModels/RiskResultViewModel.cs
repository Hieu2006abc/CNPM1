using HeartCareAI.Models;

namespace HeartCareAI.ViewModels;

public class RiskResultViewModel
{
    public Guid PredictionId { get; set; }

    public string FullName { get; set; } = string.Empty;

    public int Age { get; set; }

    public string Gender { get; set; } = string.Empty;

    public int SystolicBloodPressure { get; set; }

    public int Cholesterol { get; set; }

    public bool FastingBloodSugarHigh { get; set; }

    public string EcgResult { get; set; } = string.Empty;

    public bool IsSmoker { get; set; }

    public double RiskProbability { get; set; }

    public int RiskPercent => (int)Math.Round(RiskProbability);

    public RiskLevel RiskLevel { get; set; }

    public string RiskLevelText => RiskLevel switch
    {
        RiskLevel.Low => "Nguy cơ thấp",
        RiskLevel.Medium => "Nguy cơ trung bình",
        RiskLevel.High => "Nguy cơ cao",
        _ => "Chưa xác định"
    };

    public string RiskCssClass => RiskLevel switch
    {
        RiskLevel.Low => "risk-low",
        RiskLevel.Medium => "risk-medium",
        RiskLevel.High => "risk-high",
        _ => "risk-low"
    };

    public string Recommendation { get; set; } = string.Empty;

    public List<string> InfluentialFactors { get; set; } = [];

    public List<RiskContribution> RiskContributions { get; set; } = [];

    public IReadOnlyList<string> NextSteps => RiskLevel switch
    {
        RiskLevel.Low =>
        [
            "Duy trì vận động tối thiểu 150 phút mỗi tuần.",
            "Kiểm tra huyết áp, đường huyết và mỡ máu định kỳ.",
            "Giữ chế độ ăn ít muối, ít chất béo bão hòa."
        ],
        RiskLevel.Medium =>
        [
            "Theo dõi huyết áp tại nhà và ghi lại chỉ số mỗi tuần.",
            "Điều chỉnh chế độ ăn, tăng vận động và giảm cân nếu cần.",
            "Trao đổi với nhân viên y tế khi có đau ngực, khó thở hoặc hồi hộp."
        ],
        _ =>
        [
            "Nên đặt lịch khám chuyên khoa tim mạch sớm.",
            "Mang theo kết quả xét nghiệm, ECG và danh sách thuốc đang dùng.",
            "Không tự ý dùng thuốc điều trị tim mạch khi chưa có chỉ định."
        ]
    };

    public string RiskNarrative => RiskLevel switch
    {
        RiskLevel.Low => "Các chỉ số hiện chưa tạo thành cụm nguy cơ nổi bật. Hệ thống khuyến nghị tiếp tục duy trì thói quen lành mạnh và theo dõi định kỳ.",
        RiskLevel.Medium => "Một số yếu tố đã vượt ngưỡng khuyến nghị. Người dùng nên xem đây là tín hiệu cần điều chỉnh lối sống và theo dõi sát hơn.",
        _ => "Nhiều yếu tố nguy cơ xuất hiện đồng thời. Kết quả cần được xem như cảnh báo sàng lọc để khuyến khích thăm khám chuyên khoa."
    };

    public int ProtectivePercent => Math.Max(0, 100 - RiskPercent);

    public DateTime CreatedAt { get; set; }

    public static RiskResultViewModel FromPrediction(Prediction prediction)
    {
        return new RiskResultViewModel
        {
            PredictionId = prediction.Id,
            FullName = prediction.Patient.FullName,
            Age = prediction.Patient.Age,
            Gender = prediction.Patient.Gender,
            SystolicBloodPressure = prediction.SystolicBloodPressure,
            Cholesterol = prediction.Cholesterol,
            FastingBloodSugarHigh = prediction.FastingBloodSugarHigh,
            EcgResult = prediction.EcgResult,
            IsSmoker = prediction.IsSmoker,
            RiskProbability = prediction.RiskProbability,
            RiskLevel = prediction.RiskLevel,
            Recommendation = prediction.Recommendation,
            InfluentialFactors = prediction.InfluentialFactors,
            RiskContributions = prediction.RiskContributions.Count > 0
                ? prediction.RiskContributions
                : BuildContributionsFromPrediction(prediction),
            CreatedAt = prediction.CreatedAt
        };
    }

    private static List<RiskContribution> BuildContributionsFromPrediction(Prediction prediction)
    {
        var items = new List<RiskContribution>();

        items.Add(CreateAgeContribution(prediction.Patient.Age));
        items.Add(CreateBloodPressureContribution(prediction.SystolicBloodPressure));
        items.Add(CreateCholesterolContribution(prediction.Cholesterol));
        items.Add(new RiskContribution
        {
            Factor = "Đường huyết",
            Score = prediction.FastingBloodSugarHigh ? 9 : 0,
            MaxScore = 9,
            Level = prediction.FastingBloodSugarHigh ? "Cao" : "Ổn định",
            Description = prediction.FastingBloodSugarHigh ? "> 120 mg/dL" : "≤ 120 mg/dL"
        });
        items.Add(CreateEcgContribution(prediction.EcgResult));
        items.Add(new RiskContribution
        {
            Factor = "Hút thuốc",
            Score = prediction.IsSmoker ? 12 : 0,
            MaxScore = 12,
            Level = prediction.IsSmoker ? "Có nguy cơ" : "Không ghi nhận",
            Description = prediction.IsSmoker ? "Có tiền sử hút thuốc" : "Không hút thuốc"
        });

        if (!string.IsNullOrWhiteSpace(prediction.ChestPainType) && prediction.ChestPainType != "None")
        {
            items.Add(new RiskContribution
            {
                Factor = "Đau ngực",
                Score = prediction.ChestPainType switch
                {
                    "Typical Angina" => 14,
                    "Atypical Angina" => 9,
                    "Non-anginal Pain" => 5,
                    "Asymptomatic" => 16,
                    _ => 0
                },
                MaxScore = 16,
                Level = "Cần chú ý",
                Description = prediction.ChestPainType
            });
        }

        if (prediction.MaxHeartRate.HasValue)
        {
            items.Add(new RiskContribution
            {
                Factor = "Nhịp tim tối đa",
                Score = prediction.MaxHeartRate < 120 ? 10 : prediction.MaxHeartRate < 150 ? 5 : 0,
                MaxScore = 10,
                Level = prediction.MaxHeartRate < 150 ? "Theo dõi" : "Ổn định",
                Description = $"{prediction.MaxHeartRate} bpm"
            });
        }

        if (prediction.StDepression.HasValue)
        {
            items.Add(new RiskContribution
            {
                Factor = "ST depression",
                Score = prediction.StDepression >= 2 ? 14 : prediction.StDepression >= 1 ? 8 : 0,
                MaxScore = 14,
                Level = prediction.StDepression >= 1 ? "Bất thường" : "Ổn định",
                Description = prediction.StDepression.Value.ToString("0.0")
            });
        }

        return items;
    }

    private static RiskContribution CreateAgeContribution(int age)
    {
        return new RiskContribution
        {
            Factor = "Tuổi",
            Score = age >= 60 ? 22 : age >= 45 ? 14 : age >= 35 ? 6 : 0,
            MaxScore = 22,
            Level = age >= 60 ? "Cao" : age >= 45 ? "Tăng" : "Ổn định",
            Description = $"{age} tuổi"
        };
    }

    private static RiskContribution CreateBloodPressureContribution(int systolicBloodPressure)
    {
        return new RiskContribution
        {
            Factor = "Huyết áp",
            Score = systolicBloodPressure >= 160 ? 20 : systolicBloodPressure >= 140 ? 14 : systolicBloodPressure >= 120 ? 7 : 0,
            MaxScore = 20,
            Level = systolicBloodPressure >= 160 ? "Rất cao" : systolicBloodPressure >= 140 ? "Cao" : systolicBloodPressure >= 120 ? "Tăng nhẹ" : "Ổn định",
            Description = $"{systolicBloodPressure} mmHg"
        };
    }

    private static RiskContribution CreateCholesterolContribution(int cholesterol)
    {
        return new RiskContribution
        {
            Factor = "Cholesterol",
            Score = cholesterol >= 240 ? 17 : cholesterol >= 200 ? 10 : 0,
            MaxScore = 17,
            Level = cholesterol >= 240 ? "Cao" : cholesterol >= 200 ? "Tăng" : "Ổn định",
            Description = $"{cholesterol} mg/dL"
        };
    }

    private static RiskContribution CreateEcgContribution(string ecgResult)
    {
        return new RiskContribution
        {
            Factor = "ECG",
            Score = ecgResult switch
            {
                "STT Abnormality" => 10,
                "Left Ventricular Hypertrophy" => 14,
                _ => 0
            },
            MaxScore = 14,
            Level = ecgResult == "Normal" ? "Bình thường" : "Bất thường",
            Description = ecgResult
        };
    }
}
