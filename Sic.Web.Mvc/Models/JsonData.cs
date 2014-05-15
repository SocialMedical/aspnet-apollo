using Sic.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sic.Web.Mvc.Models
{
    public class JsonData
    {
        public MessageCollection Messages { get; set; }

        public Message Message
        {
            get
            {
                if(Messages!=null && Messages.Any())
                    return Messages.FirstOrDefault();
                return null;
            }
        }

        public Object Content { get; set; }
    }
}
