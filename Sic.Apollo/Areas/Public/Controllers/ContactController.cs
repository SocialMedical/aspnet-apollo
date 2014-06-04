using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sic.Web.Mvc;
using Sic.Apollo.Models;
using System.IO;
using Sic.Web.Mvc.Controllers;
using Sic.Apollo.Models.General;
using System.Drawing;

namespace Sic.Apollo.Areas.Public.Controllers
{
    public class ContactController : BaseController
    {
        
        ContextService db = new ContextService();

        private const int maxSizeUploadFile = 5242880;//2 MB;
        private const string contactImagePath = "~/Content/images/contacts";
        private const string contactImageSavePath = "/Content/images/contacts/";

        private const string contacLocationImagePath = "~/Content/images/contactLocations";
        private const string contacLocationImageSavePath = "/Content/images/contactLocations/";

        int ProfilePictureWidth = 320;
        int ProfilePictureHeight = 469;

        int ProfilePictuteMinWidth = 75;
        int ProfilePictuteMinHeight = 110;

        int ProfilePictuteMedWidth = 130;
        int ProfilePictuteMedHeight = 190;

        #region ContactLocationGallery

        public ActionResult ContactLocationGallery(int contactLocationId, string defaultPicture = null)
        {
            var pictures = db.ContactLocationPictures.Get(p => p.ContactLocationId == contactLocationId, includeProperties: "ContactLocation");
            return View("Gallery",pictures);
        }

        #endregion

        

        #region Contact Picture Upload

        public ActionResult EditProfilePicture()
        {            
            return PartialView(db.Contacts.GetByID(Sic.Web.Mvc.Session.UserId));
        }

        public ActionResult ProfilePicture()
        {
            return PartialView(db.Contacts.GetByID(Sic.Web.Mvc.Session.UserId));
        }

        [HttpPost]
        public WrappedJsonResult DeleteProfilePicture()
        {
            string message = Sic.Apollo.Resources.Resources.MessageForDeletePictureOk;
            bool valid = false;
            Contact pictureDelete = db.Contacts.Get(p => 
                p.ContactId == Sic.Web.Mvc.Session.UserId).SingleOrDefault();
            
            try
            {
                if (pictureDelete != null)
                {
                    var deleteImagePath = Path.Combine(Server.MapPath(Url.Content(contactImagePath)), Path.GetFileName(pictureDelete.Picture));
                    FileInfo FileDetele = new FileInfo(deleteImagePath);
                    if (FileDetele.Exists)
                        FileDetele.Delete();

                    pictureDelete.Picture = null;

                    db.Contacts.Update(pictureDelete);
                    db.Save();
                    valid = true;
                }
                else
                    message = Sic.Apollo.Resources.Resources.MessageForNotPictureThatDelete;
            }
            catch (Exception)
            {
                message = Sic.Apollo.Resources.Resources.MessageForDeletePictureFailure;
            }
            
            return new WrappedJsonResult() { Data = new { Message = message, IsValid = valid } };
        }

        [HttpPost]
        public WrappedJsonResult UploadProfilePicture(HttpPostedFileWrapper file)
        {
            try
            {
                if (file == null || file.ContentLength == 0)
                {
                    return new WrappedJsonResult
                    {
                        Data = new
                        {
                            IsValid = false,
                            Message = Sic.Apollo.Resources.Resources.MessageForPictureUploadedFailure,
                            ImagePath = string.Empty
                        }
                    };
                }

                if (file.ContentLength > maxSizeUploadFile)
                {
                    return new WrappedJsonResult
                    {
                        Data = new
                        {
                            IsValid = false,
                            Message = String.Format(Sic.Apollo.Resources.Resources.MessageForPictureMaxSizeValidation, (maxSizeUploadFile / 1048576)),
                            ImagePath = string.Empty
                        }
                    };
                }

                var fileName = String.Format("{0}{1}", Guid.NewGuid(), System.IO.Path.GetExtension(file.FileName));
                var imagePath = Path.Combine(Server.MapPath(Url.Content(contactImagePath)), fileName);

                //var contact = db.Contacts.GetByID(Sic.Web.Mvc.Session.UserId);
                //if (contact != null)
                //{
                //    if (!string.IsNullOrEmpty(contact.Picture))
                //    {
                //        var deleteImagePath = Path.Combine(Server.MapPath(Url.Content(contactImagePath)), Path.GetFileName(contact.Picture));
                //        var deleteMiniImagePath = Path.Combine(Server.MapPath(Url.Content(contactImagePath)),
                //            Path.GetFileNameWithoutExtension(contact.Picture) + "_min" + Path.GetExtension(contact.Picture));
                //        FileInfo FileDetele = new FileInfo(deleteImagePath);
                //        if (FileDetele.Exists)
                //            FileDetele.Delete();
                //        FileInfo FileDeteleMin = new FileInfo(deleteMiniImagePath);
                //        if (FileDeteleMin.Exists)
                //            FileDeteleMin.Delete();                    
                //    }

                //    contact.Picture = contactImageSavePath + fileName;
                //    db.Contacts.Update(contact);
                //    db.Save();
                //}

                file.SaveAs(imagePath);
                //SaveThumbnail(imagePath);                

                return new WrappedJsonResult
                {
                    Data = new
                    {
                        IsValid = true,
                        Message = Sic.Apollo.Resources.Resources.MessageForFileUploadedSuccess,
                        ImagePath = Url.Content(contactImageSavePath + fileName),
                        Width = ProfilePictureWidth,
                        Height = ProfilePictureHeight
                    }
                };
            }
            catch
            {
                return new WrappedJsonResult
                {
                    Data = new
                    {
                        IsValid = false,
                        Message = Sic.Apollo.Resources.Resources.MessageForFileUploadedFailure                        
                    }
                };
            }
        }

        [HttpPost]    
        [Authorize(UserType.Professional,UserType.Customer)]
        public JsonResult SaveContactProfilePicture(string fileName,double x, double y, double width, double height)
        {
            try
            {
                fileName = Path.Combine(Server.MapPath(Url.Content(contactImagePath)), Path.GetFileName(fileName));

                Rectangle cropRect = new Rectangle(Convert.ToInt32(x), Convert.ToInt32(y), Convert.ToInt32(width), Convert.ToInt32(height));
                Bitmap src = Image.FromFile(fileName) as Bitmap;
                Bitmap target = new Bitmap(cropRect.Width, cropRect.Height);

                using (Graphics g = Graphics.FromImage(target))
                {
                    g.DrawImage(src, new Rectangle(0, 0, target.Width, target.Height),
                                     cropRect,
                                     GraphicsUnit.Pixel);
                }
               
                FileInfo fileDelete = new FileInfo(fileName);

                var newfileName = String.Format("{0}{1}", Guid.NewGuid(), System.IO.Path.GetExtension(fileDelete.Name));
                var imagePath = Path.Combine(Server.MapPath(Url.Content(contactImagePath)), newfileName);
                
                //fileDelete.Delete();

                target.Save(imagePath);
                string minFileName = Sic.Web.Mvc.Utility.Thumbnail.SaveThumbnail(imagePath, ProfilePictuteMinWidth, ProfilePictuteMinHeight, "min");
                Sic.Web.Mvc.Utility.Thumbnail.SaveThumbnail(imagePath, ProfilePictuteMedWidth, ProfilePictuteMedHeight, "med");

                var contact = db.Contacts.GetByID(Sic.Web.Mvc.Session.UserId);
                if (contact != null)
                {
                    if (!string.IsNullOrEmpty(contact.Picture))
                    {
                        var deleteImagePath = Path.Combine(Server.MapPath(Url.Content(contactImagePath)), Path.GetFileName(contact.Picture));
                        var deleteMiniImagePath = Path.Combine(Server.MapPath(Url.Content(contactImagePath)),
                            Path.GetFileNameWithoutExtension(contact.Picture) + "_min" + Path.GetExtension(contact.Picture));
                        FileInfo FileDetele = new FileInfo(deleteImagePath);
                        if (FileDetele.Exists)
                            FileDetele.Delete();
                        FileInfo FileDeteleMin = new FileInfo(deleteMiniImagePath);
                        if (FileDeteleMin.Exists)
                            FileDeteleMin.Delete();
                    }

                    contact.Picture = contactImageSavePath + newfileName;
                    db.Contacts.Update(contact);
                    db.Save();
                }

                return new JsonResult
                {                    
                    Data = new
                    {
                        IsValid = true,
                        Message = Sic.Apollo.Resources.Resources.MessageForPictureUploadedOk,
                        MessageType = Sic.Constants.MessageType.Success,
                        ImagePath = Url.Content(contactImageSavePath + newfileName),
                        ThumbnailImagePath = contact.PictureMin
                    }
                };
            }
            catch (Exception)
            {
                return new JsonResult
                {
                    Data = new
                    {
                        IsValid = false,
                        Message = Sic.Apollo.Resources.Resources.MessageForPictureUploadedFailure,
                        MessageType = Sic.Constants.MessageType.Error
                    }
                };
            }
        }

        #endregion

        #region ContactLocation Picture Upload

        public ActionResult EditContactLocationPictures(int contactLocationId)
        {
            return View(db.ContactLocations.Get(p=>p.ContactLocationId == contactLocationId,includeProperties:"ContactLocationPictures").Single());
        }

        [HttpPost]
        public JsonResult ContactLocationPictureDelete(ContactLocationPicture picture)
        {
            string message = Sic.Apollo.Resources.Resources.MessageForDeletePictureOk;
            bool valid = false;
            ContactLocationPicture pictureDelete = db.ContactLocationPictures.Get(p => p.ContactLocationId == picture.ContactLocationId && p.Priority == picture.Priority).SingleOrDefault();
            try
            {
                if (pictureDelete != null)
                {
                    var deleteImagePath = Path.Combine(Server.MapPath(Url.Content(contacLocationImagePath)), Path.GetFileName(pictureDelete.Picture));
                    FileInfo FileDetele = new FileInfo(deleteImagePath);
                    if (FileDetele.Exists)
                        FileDetele.Delete();

                    db.ContactLocationPictures.Delete(pictureDelete);                    
                    db.Save();
                    valid = true;
                }
                else
                    message = Sic.Apollo.Resources.Resources.MessageForNotPictureThatDelete;
            }
            catch (Exception)
            {
                message = Sic.Apollo.Resources.Resources.MessageForDeletePictureFailure;
            }
            return new JsonResult() { Data = new { Message = message, IsValid = valid } };
        }

        [HttpPost]
        public JsonResult ContactLocationPictureSaveDescription(ContactLocationPicture picture)
        {
            ContactLocationPicture pictureUpdate = db.ContactLocationPictures.Get(p => p.ContactLocationId == picture.ContactLocationId && p.Priority == picture.Priority).SingleOrDefault();
            string message = "";
            if (pictureUpdate != null)
            {
                pictureUpdate.PictureDescription = picture.PictureDescription;
                db.ContactLocationPictures.Update(pictureUpdate);
                db.Save();
                message = Sic.Apollo.Resources.Resources.MessageForSaveOk;
            }
            else
                message = Sic.Apollo.Resources.Resources.MessageForPictureRequired;

            return new JsonResult() { Data = new { Message = message } };
        }

        [HttpPost]
        public WrappedJsonResult UploadContactLocationPictures(HttpPostedFileWrapper file, int contactLocationId, short priority,
            string pictureDescription)
        {
            if (file == null || file.ContentLength == 0)
            {
                return new WrappedJsonResult
                {
                    Data = new
                    {
                        IsValid = false,
                        MessageType = Sic.Constants.MessageType.Error,
                        Message = Sic.Apollo.Resources.Resources.MessageForPictureUploadedFailure,
                        Priority = priority,
                        ContactLocationId = contactLocationId,
                        ImagePath = string.Empty
                    }
                };
            }

            if (file.ContentLength > maxSizeUploadFile)
            {
                return new WrappedJsonResult
                {
                    Data = new
                    {
                        IsValid = false,
                        MessageType = Sic.Constants.MessageType.Error,
                        Message = String.Format(Sic.Apollo.Resources.Resources.MessageForPictureMaxSizeValidation, (maxSizeUploadFile / 1048576)),
                        Priority = priority,
                        ContactLocationId = contactLocationId,
                        ImagePath = string.Empty
                    }
                };
            }

            ContactLocationPicture picture = db.ContactLocationPictures.Get(p=> p.ContactLocationId == contactLocationId && p.Priority == priority).SingleOrDefault(); 
            
            var fileName = String.Format("{0}{1}", Guid.NewGuid().ToString(), System.IO.Path.GetExtension(file.FileName));
            var imagePath = Path.Combine(Server.MapPath(Url.Content(contacLocationImagePath)), fileName);

            if(picture!=null)
            {
                var deleteImagePath = Path.Combine(Server.MapPath(Url.Content(contacLocationImagePath)), Path.GetFileName(picture.Picture));
                var deleteMiniImagePath = Path.Combine(Server.MapPath(Url.Content(contacLocationImagePath)),
                        Path.GetFileNameWithoutExtension(picture.Picture) + "_min" + Path.GetExtension(picture.Picture));

                FileInfo FileDetele = new FileInfo(deleteImagePath);
                if(FileDetele.Exists)
                    FileDetele.Delete();
                FileInfo FileDeteleMin = new FileInfo(deleteMiniImagePath);
                if (FileDeteleMin.Exists)
                    FileDeteleMin.Delete();    

                db.ContactLocationPictures.Update(picture);               
            }
            else
            {
                picture = new ContactLocationPicture()
                {
                    Priority = priority,
                    ContactLocationId = contactLocationId,                    
                };

                db.ContactLocationPictures.Insert(picture);
            }

            picture.PictureDescription = pictureDescription;
            picture.Picture = contacLocationImageSavePath + fileName;

            file.SaveAs(imagePath);
            
            Sic.Web.Mvc.Utility.Thumbnail.SaveThumbnail(imagePath);

            db.Save();

            return new WrappedJsonResult
            {
                Data = new
                {
                    IsValid = true,
                    MessageType = Sic.Constants.MessageType.Success,
                    Message = Sic.Apollo.Resources.Resources.MessageForPictureUploadedOk,
                    Priority = priority,
                    ContactLocationId = contactLocationId,
                    ImagePath = Url.Content(contacLocationImageSavePath + fileName),
                    ThumbnailImagePath = picture.Thumbnail
                }
            };
        }

        #endregion
    }
}
