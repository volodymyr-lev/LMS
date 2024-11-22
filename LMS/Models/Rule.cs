namespace LMS.Models;

public class Rule
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }

    public ICollection<RuleParameter> Parameters { get; set; }
}