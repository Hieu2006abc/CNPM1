using System.ComponentModel.DataAnnotations;

namespace HeartCareAI.ViewModels;

public class RiskInputViewModel
{
    [Required(ErrorMessage = "Vui lòng nhập họ tên bệnh nhân.")]
    [StringLength(120, ErrorMessage = "Họ tên không vượt quá 120 ký tự.")]
    [Display(Name = "Họ tên bệnh nhân")]
    public string FullName { get; set; } = string.Empty;

    [Range(1, 120, ErrorMessage = "Tuổi phải nằm trong khoảng 1-120.")]
    [Display(Name = "Tuổi")]
    public int Age { get; set; } = 45;

    [Required]
    [Display(Name = "Giới tính")]
    public string Gender { get; set; } = "Nam";

    [Range(70, 260, ErrorMessage = "Huyết áp tâm thu nên nằm trong khoảng 70-260 mmHg.")]
    [Display(Name = "Huyết áp tâm thu")]
    public int SystolicBloodPressure { get; set; } = 120;

    [Range(80, 600, ErrorMessage = "Cholesterol nên nằm trong khoảng 80-600 mg/dL.")]
    [Display(Name = "Cholesterol")]
    public int Cholesterol { get; set; } = 190;

    [Display(Name = "Đường huyết lúc đói > 120 mg/dL")]
    public bool FastingBloodSugarHigh { get; set; }

    [Required]
    [Display(Name = "Kết quả ECG")]
    public string EcgResult { get; set; } = "Normal";

    [Display(Name = "Tiền sử hút thuốc")]
    public bool IsSmoker { get; set; }

    [Display(Name = "Loại đau ngực")]
    public string ChestPainType { get; set; } = "None";

    [Range(60, 230, ErrorMessage = "Nhịp tim tối đa nên nằm trong khoảng 60-230.")]
    [Display(Name = "Nhịp tim tối đa")]
    public int? MaxHeartRate { get; set; }

    [Display(Name = "Đau thắt ngực khi gắng sức")]
    public bool ExerciseAngina { get; set; }

    [Range(0, 8, ErrorMessage = "ST depression nên nằm trong khoảng 0-8.")]
    [Display(Name = "Chỉ số ST depression")]
    public double? StDepression { get; set; }
}
