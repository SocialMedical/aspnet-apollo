using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sic.Data.Entity
{
    public interface IIdentifiable
    {
        string Key { get; }
        string DescriptionName { get; }
    }
}
