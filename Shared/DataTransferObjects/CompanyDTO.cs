﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DataTransferObjects
{
  public record CompanyDTO(Guid Id, string Name, string FullAddress);
}