using HeartCareAI.Repositories;
using HeartCareAI.Services;
using HeartCareAI.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace HeartCareAI.Controllers;

public class RiskAssessmentController : Controller
{
    private readonly IPredictionRepository _predictionRepository;
    private readonly IRiskPredictionService _riskPredictionService;

    public RiskAssessmentController(
        IPredictionRepository predictionRepository,
        IRiskPredictionService riskPredictionService)
    {
        _predictionRepository = predictionRepository;
        _riskPredictionService = riskPredictionService;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View(new RiskInputViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Analyze(RiskInputViewModel input)
    {
        if (!ModelState.IsValid)
        {
            return View("Index", input);
        }

        var prediction = _riskPredictionService.Analyze(input);
        await _predictionRepository.AddAsync(prediction);

        return View("Result", RiskResultViewModel.FromPrediction(prediction));
    }

    [HttpGet]
    public async Task<IActionResult> Result(Guid id)
    {
        var prediction = await _predictionRepository.GetByIdAsync(id);

        if (prediction is null)
        {
            return NotFound();
        }

        return View(RiskResultViewModel.FromPrediction(prediction));
    }
}
