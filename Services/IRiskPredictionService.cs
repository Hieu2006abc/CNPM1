using HeartCareAI.Models;
using HeartCareAI.ViewModels;

namespace HeartCareAI.Services;

public interface IRiskPredictionService
{
    Prediction Analyze(RiskInputViewModel input);
}
