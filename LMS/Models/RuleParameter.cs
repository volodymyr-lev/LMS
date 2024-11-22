namespace LMS.Models;

public class RuleParameter
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Value { get; set; }

    public int RuleId { get; set; }
    public Rule Rule { get; set; }
}