namespace LMS.Models;

public class RuleRuleParameter
{
    public int RuleId { get; set; }
    public Rule Rule { get; set; }

    public int RuleParameterId { get; set; }
    public RuleParameter RuleParameter { get; set; }
}
