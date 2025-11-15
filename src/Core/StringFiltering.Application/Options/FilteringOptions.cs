namespace StringFiltering.Application.Options;

public class FilteringOptions
{
    public const string SectionName = "Filtering";
    public double Threshold { get; init; } 
    public string[] BadWords { get; init; } = null!;
}