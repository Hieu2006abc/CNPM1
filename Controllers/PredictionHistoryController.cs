using HeartCareAI.Models;
using HeartCareAI.Repositories;
using HeartCareAI.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace HeartCareAI.Controllers;

public class PredictionHistoryController : Controller
{
    private readonly IPredictionRepository _predictionRepository;

    public PredictionHistoryController(IPredictionRepository predictionRepository)
    {
        _predictionRepository = predictionRepository;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? search, RiskLevel? riskLevel)
    {
        var predictions = await _predictionRepository.GetAllAsync();
        var query = predictions.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(item =>
                item.Patient.FullName.Contains(search, StringComparison.OrdinalIgnoreCase));
        }

        if (riskLevel.HasValue)
        {
            query = query.Where(item => item.RiskLevel == riskLevel.Value);
        }

        var viewModel = new PredictionHistoryViewModel
        {
            Search = search,
            RiskLevel = riskLevel,
            Predictions = query.ToList()
        };

        return View(viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid id)
    {
        var prediction = await _predictionRepository.GetByIdAsync(id);

        if (prediction is null)
        {
            return NotFound();
        }

        return View(RiskResultViewModel.FromPrediction(prediction));
    }
}
