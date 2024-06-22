using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DataTransferObjects
{
  // first solution to put serializable attribute second one to put it as below 
  //[Serializable]
  //public record CompanyDto(Guid Id, string Name, string FullAddress);

  // for having the real names in the xml  and init to give it just one initialization 

  public record CompanyDto
  {
    public Guid Id { get; init; }
    public string? Name { get; init; }
    public string? FullAddress { get; init; }
  }
}
