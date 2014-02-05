using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sic.Apollo.Models
{
    public interface IPicture
    {
        string Picture { get; }
        string Thumbnail { get; }
        string Title { get; }
        string PictureDescription { get; }
        int ParentId { get; }
        short Priority { get; }
    }
}
