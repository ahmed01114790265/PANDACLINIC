using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PANDACLINIC.Domain.Comman.ValueObject
{
    public record Money(decimal Amount, string Currency = "EGY");
}
