using HeartCareAI.Models;
using Microsoft.AspNetCore.Mvc;

namespace HeartCareAI.Controllers;

public class ModelInfoController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        IReadOnlyList<ModelMetric> metrics =
        [
            new() { MetricName = "Accuracy", MetricValue = 0.86, Description = "Tỷ lệ dự đoán đúng trên tập kiểm thử." },
            new() { MetricName = "Precision", MetricValue = 0.84, Description = "Độ chính xác khi hệ thống dự đoán nguy cơ cao." },
            new() { MetricName = "Recall", MetricValue = 0.88, Description = "Khả năng phát hiện đúng các ca nguy cơ cao." },
            new() { MetricName = "F1-score", MetricValue = 0.86, Description = "Trung bình điều hòa giữa Precision và Recall." },
            new() { MetricName = "ROC-AUC", MetricValue = 0.91, Description = "Khả năng phân tách giữa hai nhóm nguy cơ." }
        ];

        return View(metrics);
    }
}
