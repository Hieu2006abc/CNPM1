namespace HeartCareAI.Models;

public class Patient
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string FullName { get; set; } = string.Empty;

    public int Age { get; set; }

    public string Gender { get; set; } = "Nam";

    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
