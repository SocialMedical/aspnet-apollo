using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;

namespace Sic.Apollo.Models.Medical
{
    [Table("tbPatientFile", Schema="med")]
    public class PatientFile : Sic.Data.Entity.EntityBase
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.DatabaseGeneratedOption.Identity)]
        public int PatientFileId { get; set; }

        public int ProfessionalId { get; set; }

        public int PatientId { get; set; }

        public string UrlContentPictureMin
        {
            get
            {
                return string.Format("~/Content/db/patients/{0}/{1}", this.PatientId, Sic.Web.Mvc.Utility.Thumbnail.GetPictureMinFromOriginal(this.PatientFileName));                
            }
        }

        public string UrlContent
        {
            get
            {
                return string.Format("~/Content/db/patients/{0}/{1}", this.PatientId, this.PatientFileName);
            }
        }

        public DateTime UploadDate { get; set; }

        public string Name { get; set; }

        public string MimeType { get; set; }

        public bool IsImageType
        {
            get
            {
                return Sic.Web.Mvc.Utility.MimeType.IsImage(Path.GetExtension(this.PatientFileName));
            }
        }

        public string MainType
        {
            get
            {
                return Sic.Web.Mvc.Utility.MimeType.GetMainType(Path.GetExtension(this.PatientFileName));
            }
        }

        public string PatientFileName { get; set; }

        public string Comment { get; set; }

        public Patient Patient { get; set; }
    }
}
