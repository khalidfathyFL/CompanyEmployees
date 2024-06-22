namespace Entities.LinkModels;

public class Link
{
  public string? Href { get; set; }
  public string? Rel { get; set; }
  public string? Method { get; set; }

  // for xml serialization purpose
  public Link()
  { }

  public Link(string href, string rel, string method)
  {
    Href = href;
    Rel = rel;
    Method = method;
  }
}