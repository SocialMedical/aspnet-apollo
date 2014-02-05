using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Sic.Data.Entity;
using System.IO;

namespace Sic.Apollo.Models.General
{
    [Table("tbContactLocationPicture", Schema="gen")]
    public class ContactLocationPicture: EntityBase, IPicture
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.DatabaseGeneratedOption.Identity)]
        public int ContactLocationPictureId { get; set; }

        public int ContactLocationId { get; set; }

        public string Picture { get; set; }

        public string Thumbnail
        {
            get
            {
                if (string.IsNullOrEmpty(this.Picture))
                    return string.Empty;

                return Sic.Web.Mvc.Utility.Thumbnail.GetPictureMinFromOriginal(this.Picture);
            }
        }

        public string PictureDescription { get; set; }

        public short Priority { get; set; }

        public ContactLocation ContactLocation { get; set; }

        #region IPicture Members

        public string Title
        {
            get { 
                if (ContactLocation != null) 
                    return this.ContactLocation.Address;
                return string.Empty;
            }
        }

        public int ParentId
        {
            get { return this.ContactLocationId; }
        }

        #endregion        
    }
}