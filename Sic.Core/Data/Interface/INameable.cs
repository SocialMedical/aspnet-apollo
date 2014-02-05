using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sic.Data
{
    public interface INameable
    {        
        string Names { get; set; }

        string LastNames { get; set; }

        string DefaultFullName { get; }
    }
}
