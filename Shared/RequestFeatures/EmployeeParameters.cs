namespace Shared.RequestFeatures;

// we use it to deserialize the data from the  query
public class EmployeeParameters : RequestParameters
{
  public EmployeeParameters() => OrderBy = "name";
  public uint MinAge { get; set; }
  public uint MaxAge { get; set; } = int.MaxValue;
  public bool ValidAgeRange => MaxAge > MinAge;
  public string? SearchTerm { get; set; }
}