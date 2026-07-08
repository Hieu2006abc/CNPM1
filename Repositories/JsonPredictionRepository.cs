using System.Text.Json;
using HeartCareAI.Models;

namespace HeartCareAI.Repositories;

public class JsonPredictionRepository : IPredictionRepository
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true
    };

    private readonly string _filePath;
    private readonly object _sync = new();

    public JsonPredictionRepository(IWebHostEnvironment environment)
    {
        _filePath = Path.Combine(environment.ContentRootPath, "App_Data", "predictions.json");
        Directory.CreateDirectory(Path.GetDirectoryName(_filePath)!);
        EnsureSeedData();
    }

    public Task<IReadOnlyList<Prediction>> GetAllAsync()
    {
        lock (_sync)
        {
            var predictions = LoadPredictions()
                .OrderByDescending(prediction => prediction.CreatedAt)
                .ToList();

            return Task.FromResult((IReadOnlyList<Prediction>)predictions);
        }
    }

    public Task<Prediction?> GetByIdAsync(Guid id)
    {
        lock (_sync)
        {
            var prediction = LoadPredictions().FirstOrDefault(item => item.Id == id);
            return Task.FromResult(prediction);
        }
    }

    public Task AddAsync(Prediction prediction)
    {
        lock (_sync)
        {
            var predictions = LoadPredictions();
            predictions.Add(prediction);
            SavePredictions(predictions);
        }

        return Task.CompletedTask;
    }

    private void EnsureSeedData()
    {
        if (File.Exists(_filePath) && new FileInfo(_filePath).Length > 0)
        {
            return;
        }

        SavePredictions(CreateSeedPredictions());
    }

    private List<Prediction> LoadPredictions()
    {
        if (!File.Exists(_filePath))
        {
            return [];
        }

        var json = File.ReadAllText(_filePath);

        if (string.IsNullOrWhiteSpace(json))
        {
            return [];
        }

        return JsonSerializer.Deserialize<List<Prediction>>(json, JsonOptions) ?? [];
    }

    private void SavePredictions(List<Prediction> predictions)
    {
        var json = JsonSerializer.Serialize(predictions, JsonOptions);
        File.WriteAllText(_filePath, json);
    }

    private static List<Prediction> CreateSeedPredictions()
    {
        return
        [
            CreateSeed("Nguyễn Minh An", 34, "Nam", 118, 178, false, "Normal", false, 24, 22, ["Tuổi trẻ", "Huyết áp ổn định"]),
            CreateSeed("Trần Hoài Thu", 49, "Nữ", 138, 224, false, "STT Abnormality", false, 51, 54, ["Cholesterol tăng", "ECG bất thường ST-T"]),
            CreateSeed("Lê Quốc Bảo", 63, "Nam", 162, 268, true, "Left Ventricular Hypertrophy", true, 74, 84, ["Tuổi cao", "Huyết áp cao", "Cholesterol cao", "Hút thuốc"]),
            CreateSeed("Phạm Khánh Linh", 41, "Nữ", 126, 202, false, "Normal", false, 39, 36, ["Cholesterol hơi cao"]),
            CreateSeed("Võ Đức Long", 57, "Nam", 146, 238, true, "STT Abnormality", true, 70, 73, ["Huyết áp cao", "Đường huyết cao", "Hút thuốc"])
        ];
    }

    private static Prediction CreateSeed(
        string fullName,
        int age,
        string gender,
        int systolicBloodPressure,
        int cholesterol,
        bool fastingBloodSugarHigh,
        string ecgResult,
        bool isSmoker,
        int daysAgo,
        double riskProbability,
        List<string> factors)
    {
        var level = riskProbability < 40
            ? RiskLevel.Low
            : riskProbability < 70 ? RiskLevel.Medium : RiskLevel.High;

        return new Prediction
        {
            Id = Guid.NewGuid(),
            Patient = new Patient
            {
                Id = Guid.NewGuid(),
                FullName = fullName,
                Age = age,
                Gender = gender,
                CreatedAt = DateTime.Now.AddDays(-daysAgo)
            },
            SystolicBloodPressure = systolicBloodPressure,
            Cholesterol = cholesterol,
            FastingBloodSugarHigh = fastingBloodSugarHigh,
            EcgResult = ecgResult,
            IsSmoker = isSmoker,
            ChestPainType = riskProbability >= 70 ? "Asymptomatic" : "None",
            MaxHeartRate = riskProbability >= 70 ? 122 : 156,
            ExerciseAngina = riskProbability >= 70,
            StDepression = riskProbability >= 70 ? 2.2 : 0.6,
            RiskProbability = riskProbability,
            RiskLevel = level,
            Recommendation = level switch
            {
                RiskLevel.Low => "Duy trì lối sống lành mạnh và theo dõi sức khỏe định kỳ.",
                RiskLevel.Medium => "Nên kiểm tra các chỉ số tim mạch định kỳ và điều chỉnh chế độ sinh hoạt.",
                _ => "Nên thăm khám chuyên khoa tim mạch để được tư vấn và chẩn đoán chính xác."
            },
            InfluentialFactors = factors,
            CreatedAt = DateTime.Now.AddDays(-daysAgo)
        };
    }
}
