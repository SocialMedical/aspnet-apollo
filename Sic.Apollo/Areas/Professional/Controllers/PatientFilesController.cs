using Sic.Apollo.Models;
using Sic.Apollo.Models.Medical;
using Sic.Web.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sic.Apollo.Areas.Professional.Controllers
{
    public class PatientFilesController : Sic.Web.Mvc.Controllers.BaseController<ContextService>
    {
        private const int maxSizeUploadFile = 5242880;//5 MB;
        private const string FilePath = "~/Content/db/patients/{0}";
        private const string FileSavePath = "/Content/db/patients/{0/}";


        [Authorize(UserType.Professional, UserType.Assistant)]
        public ActionResult FileUpload(HttpPostedFileWrapper file, int patientId)
        {
            try
            {
                if (file == null || file.ContentLength == 0)
                {
                    this.AddErrorMessage(Sic.Apollo.Resources.Resources.MessageForPictureUploadedFailure);
                    return WrappedJson();
                }

                if (file.ContentLength > maxSizeUploadFile)
                {
                    this.AddErrorMessage(String.Format(Sic.Apollo.Resources.Resources.MessageForFileMaxSizeValidation, (maxSizeUploadFile / 1048576)));
                    return WrappedJson();
                }

                var fileName = String.Format("{0}.{1}{2}", Path.GetFileNameWithoutExtension(file.FileName), Guid.NewGuid(), System.IO.Path.GetExtension(file.FileName));
                var imagePath = Path.Combine(Server.MapPath(Url.Content(string.Format(FilePath, patientId))), fileName);
                var folder = Path.GetDirectoryName(imagePath);
                bool isExists = System.IO.Directory.Exists(folder);
                if (!isExists)
                    System.IO.Directory.CreateDirectory(folder);

                file.SaveAs(imagePath);
                if (Sic.Web.Mvc.Utility.MimeType.IsImage(Path.GetExtension(imagePath)))
                    Sic.Web.Mvc.Utility.Thumbnail.SaveThumbnail(imagePath);

                PatientFile patientFile = new PatientFile();
                patientFile.Name = Path.GetFileNameWithoutExtension(file.FileName);
                patientFile.PatientFileName = fileName;
                patientFile.PatientId = patientId;
                patientFile.ProfessionalId = Sic.Apollo.Session.ProfessionalId;
                patientFile.UploadDate = Sic.Web.Mvc.Session.CurrentDateTime;
                patientFile.MimeType = Sic.Web.Mvc.Utility.MimeType.GetMimeType(Path.GetExtension(patientFile.PatientFileName));
                DataBase.PatientFiles.Insert(patientFile);

                DataBase.Save();

                this.AddSuccessMessage(Sic.Apollo.Resources.Resources.MessageForFileUploadedSuccess);
                
                return WrappedJson(new {PatientFileId = patientFile.PatientFileId});                
            }
            catch
            {
                this.AddErrorMessage(Sic.Apollo.Resources.Resources.MessageForFileUploadedFailure);
                return WrappedJson();                
            }            
        }

        [Authorize(UserType.Professional, UserType.Assistant)]
        [ChildAction()]
        [HttpPost]
        public ActionResult Edit(int patientFileId, string name, string comment = null)
        {
            try
            {
                PatientFile patientFile = DataBase.PatientFiles.GetByID(patientFileId);
                patientFile.Comment = comment;
                patientFile.Name = name;

                DataBase.PatientFiles.Update(patientFile);
                DataBase.Save();

                this.AddDefaultSuccessMessage();
            }
            catch
            {
                this.AddDefaultErrorMessage();
            }
            return Json();
        }

        [Authorize(UserType.Professional, UserType.Assistant)]
        [ChildAction]
        public ActionResult Detail(int patientFileId)
        {
            PatientFile patientFile = DataBase.PatientFiles.GetByID(patientFileId);

            return PartialView("_PatientFile", patientFile);
        }

        [Authorize(UserType.Professional, UserType.Assistant)]
        [ChildAction]
        [HttpPost]
        public ActionResult Delete(int patientFileId)
        {
            try
            {
                PatientFile patientFile = DataBase.PatientFiles.GetByID(patientFileId);

                DataBase.PatientFiles.Delete(patientFile);
                DataBase.Save();

                var imagePath = Path.Combine(Server.MapPath(Url.Content(string.Format(FilePath, patientFile.PatientId))), patientFile.PatientFileName);
                FileInfo FileDetele = new FileInfo(imagePath);
                if (FileDetele.Exists)
                    FileDetele.Delete();

                if (patientFile.IsImageType)
                {
                    string nameMin = Sic.Web.Mvc.Utility.Thumbnail.GetPictureMinFromOriginal(patientFile.PatientFileName);
                    if (nameMin.StartsWith("\\"))
                    {
                        nameMin = nameMin.Substring(1);
                    }
                    var imagePathMini = Path.Combine(Server.MapPath(Url.Content(string.Format(FilePath, patientFile.PatientId))),
                        nameMin);
                    FileInfo FileDeteleMin = new FileInfo(imagePathMini);
                    if (FileDeteleMin.Exists)
                        FileDeteleMin.Delete();
                }

                this.AddDefaultSuccessMessage();                
            }
            catch
            {
                this.AddDefaultErrorMessage();
            }

            return Json();
        }
	}
}