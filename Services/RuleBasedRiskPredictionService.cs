using HeartCareAI.Models;
using HeartCareAI.ViewModels;

namespace HeartCareAI.Services;

public class RuleBasedRiskPredictionService : IRiskPredictionService
{
    private const double BaselineScore = 8d;

    public Prediction Analyze(RiskInputViewModel input)
    {
        var contributions = BuildContributions(input);
        var score = BaselineScore + contributions.Sum(item => item.Score);
        var probability = Math.Clamp(score, 5, 96);
        var level = probability < 40
            ? RiskLevel.Low
            : probability < 70 ? RiskLevel.Medium : RiskLevel.High;

        var factors = contributions
            .Where(item => item.Score >= 5)
            .OrderByDescending(item => item.Score)
            .Select(item => $"{item.Factor}: {item.Description}")
            .ToList();

        if (factors.Count == 0)
        {
            factors.Add("Các chỉ số đang trong vùng nguy cơ thấp");
        }

        return new Prediction
        {
            Patient = new Patient
            {
                FullName = input.FullName.Trim(),
                Age = input.Age,
                Gender = input.Gender
            },
            SystolicBloodPressure = input.SystolicBloodPressure,
            Cholesterol = input.Cholesterol,
            FastingBloodSugarHigh = input.FastingBloodSugarHigh,
            EcgResult = input.EcgResult,
            IsSmoker = input.IsSmoker,
            ChestPainType = input.ChestPainType,
            MaxHeartRate = input.MaxHeartRate,
            ExerciseAngina = input.ExerciseAngina,
            StDepression = input.StDepression,
            RiskProbability = probability,
            RiskLevel = level,
            Recommendation = GetRecommendation(level),
            InfluentialFactors = factors,
            RiskContributions = contributions,
            CreatedAt = DateTime.Now
        };
    }

    private static List<RiskContribution> BuildContributions(RiskInputViewModel input)
    {
        var items = new List<RiskContribution>
        {
            AnalyzeAge(input.Age),
            AnalyzeBloodPressure(input.SystolicBloodPressure),
            AnalyzeCholesterol(input.Cholesterol),
            AnalyzeBloodSugar(input.FastingBloodSugarHigh),
            AnalyzeEcg(input.EcgResult),
            AnalyzeSmoking(input.IsSmoker),
            AnalyzeGender(input.Gender, input.Age),
            AnalyzeChestPain(input.ChestPainType),
            AnalyzeMaxHeartRate(input.MaxHeartRate),
            AnalyzeExerciseAngina(input.ExerciseAngina),
            AnalyzeStDepression(input.StDepression)
        };

        return items.Where(item => item.MaxScore > 0).ToList();
    }

    private static RiskContribution AnalyzeAge(int age)
    {
        var score = age >= 60 ? 22 : age >= 45 ? 14 : age >= 35 ? 6 : 0;

        return new RiskContribution
        {
            Factor = "Tuổi",
            Score = score,
            MaxScore = 22,
            Level = age >= 60 ? "Cao" : age >= 45 ? "Tăng" : age >= 35 ? "Theo dõi" : "Ổn định",
            Description = age >= 60
                ? $"{age} tuổi, nguy cơ tăng rõ theo tuổi"
                : age >= 45
                    ? $"{age} tuổi, bắt đầu vào nhóm cần theo dõi"
                    : $"{age} tuổi"
        };
    }

    private static RiskContribution AnalyzeBloodPressure(int systolicBloodPressure)
    {
        var score = systolicBloodPressure >= 160 ? 20 : systolicBloodPressure >= 140 ? 14 : systolicBloodPressure >= 120 ? 7 : 0;

        return new RiskContribution
        {
            Factor = "Huyết áp",
            Score = score,
            MaxScore = 20,
            Level = systolicBloodPressure >= 160 ? "Rất cao" : systolicBloodPressure >= 140 ? "Cao" : systolicBloodPressure >= 120 ? "Tăng nhẹ" : "Ổn định",
            Description = $"{systolicBloodPressure} mmHg"
        };
    }

    private static RiskContribution AnalyzeCholesterol(int cholesterol)
    {
        var score = cholesterol >= 240 ? 17 : cholesterol >= 200 ? 10 : 0;

        return new RiskContribution
        {
            Factor = "Cholesterol",
            Score = score,
            MaxScore = 17,
            Level = cholesterol >= 240 ? "Cao" : cholesterol >= 200 ? "Tăng" : "Ổn định",
            Description = $"{cholesterol} mg/dL"
        };
    }

    private static RiskContribution AnalyzeBloodSugar(bool fastingBloodSugarHigh)
    {
        return new RiskContribution
        {
            Factor = "Đường huyết",
            Score = fastingBloodSugarHigh ? 9 : 0,
            MaxScore = 9,
            Level = fastingBloodSugarHigh ? "Cao" : "Ổn định",
            Description = fastingBloodSugarHigh ? "> 120 mg/dL" : "≤ 120 mg/dL"
        };
    }

    private static RiskContribution AnalyzeEcg(string ecgResult)
    {
        var score = ecgResult switch
        {
            "STT Abnormality" => 10,
            "Left Ventricular Hypertrophy" => 14,
            _ => 0
        };

        return new RiskContribution
        {
            Factor = "ECG",
            Score = score,
            MaxScore = 14,
            Level = score > 0 ? "Bất thường" : "Bình thường",
            Description = ecgResult switch
            {
                "STT Abnormality" => "Bất thường ST-T",
                "Left Ventricular Hypertrophy" => "Gợi ý phì đại thất trái",
                _ => "Bình thường"
            }
        };
    }

    private static RiskContribution AnalyzeSmoking(bool isSmoker)
    {
        return new RiskContribution
        {
            Factor = "Hút thuốc",
            Score = isSmoker ? 12 : 0,
            MaxScore = 12,
            Level = isSmoker ? "Có nguy cơ" : "Không ghi nhận",
            Description = isSmoker ? "Có tiền sử hút thuốc" : "Không hút thuốc"
        };
    }

    private static RiskContribution AnalyzeGender(string gender, int age)
    {
        var score = string.Equals(gender, "Nam", StringComparison.OrdinalIgnoreCase) && age >= 45 ? 4 : 0;

        return new RiskContribution
        {
            Factor = "Giới tính",
            Score = score,
            MaxScore = 4,
            Level = score > 0 ? "Theo dõi" : "Trung tính",
            Description = score > 0 ? "Nam giới trên 45 tuổi" : gender
        };
    }

    private static RiskContribution AnalyzeChestPain(string chestPainType)
    {
        var score = chestPainType switch
        {
            "Typical Angina" => 14,
            "Atypical Angina" => 9,
            "Non-anginal Pain" => 5,
            "Asymptomatic" => 16,
            _ => 0
        };

        return new RiskContribution
        {
            Factor = "Đau ngực",
            Score = score,
            MaxScore = 16,
            Level = score >= 14 ? "Cao" : score > 0 ? "Theo dõi" : "Không ghi nhận",
            Description = chestPainType switch
            {
                "Typical Angina" => "Đau ngực điển hình",
                "Atypical Angina" => "Đau ngực không điển hình",
                "Non-anginal Pain" => "Đau không do thắt ngực",
                "Asymptomatic" => "Không triệu chứng rõ",
                _ => "Không ghi nhận"
            }
        };
    }

    private static RiskContribution AnalyzeMaxHeartRate(int? maxHeartRate)
    {
        if (!maxHeartRate.HasValue)
        {
            return new RiskContribution { Factor = "Nhịp tim tối đa", MaxScore = 0 };
        }

        var score = maxHeartRate < 120 ? 10 : maxHeartRate < 150 ? 5 : 0;

        return new RiskContribution
        {
            Factor = "Nhịp tim tối đa",
            Score = score,
            MaxScore = 10,
            Level = maxHeartRate < 120 ? "Thấp" : maxHeartRate < 150 ? "Theo dõi" : "Ổn định",
            Description = $"{maxHeartRate} bpm"
        };
    }

    private static RiskContribution AnalyzeExerciseAngina(bool exerciseAngina)
    {
        return new RiskContribution
        {
            Factor = "Gắng sức",
            Score = exerciseAngina ? 12 : 0,
            MaxScore = 12,
            Level = exerciseAngina ? "Có triệu chứng" : "Không ghi nhận",
            Description = exerciseAngina ? "Đau thắt ngực khi gắng sức" : "Không đau khi gắng sức"
        };
    }

    private static RiskContribution AnalyzeStDepression(double? stDepression)
    {
        if (!stDepression.HasValue)
        {
            return new RiskContribution { Factor = "ST depression", MaxScore = 0 };
        }

        var score = stDepression >= 2 ? 14 : stDepression >= 1 ? 8 : 0;

        return new RiskContribution
        {
            Factor = "ST depression",
            Score = score,
            MaxScore = 14,
            Level = stDepression >= 2 ? "Tăng rõ" : stDepression >= 1 ? "Tăng nhẹ" : "Ổn định",
            Description = stDepression.Value.ToString("0.0")
        };
    }

    private static string GetRecommendation(RiskLevel level)
    {
        return level switch
        {
            RiskLevel.Low => "Duy trì lối sống lành mạnh, vận động đều đặn và kiểm tra sức khỏe định kỳ.",
            RiskLevel.Medium => "Theo dõi huyết áp, cholesterol, đường huyết và nên trao đổi với nhân viên y tế khi có triệu chứng bất thường.",
            _ => "Nên thăm khám chuyên khoa tim mạch sớm để được đánh giá lâm sàng và tư vấn điều trị phù hợp."
        };
    }
}
