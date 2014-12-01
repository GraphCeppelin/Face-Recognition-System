using System;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using FaceRecognitionSystem.Interfaces;
using FaceRecognitionSystem.Models;
using FaceRecognitionSystem.Services;
using FaceRecognitionSystem.Infrastructure;
using FaceRecognitionSystem.ImageProcessing;
using System.Collections.Generic;

namespace FaceRecognitionSystem.Controllers
{
    public class AccountController : Controller
    {
        public IFormsAuthenticationService FormsService { get; set; }
        public IMembershipService MembershipService { get; set; }

        protected override void Initialize(RequestContext requestContext)
        {
            if (FormsService == null) { FormsService = new FormsAuthenticationService(); }
            if (MembershipService == null) { MembershipService = new AccountMembershipService(); }

            base.Initialize(requestContext);
        }

        #region Logon

        #region Ajax Submit

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult UpdateAvatar(int? id)
        {
            ActionResult res = AjaxSubmit(id);
            if ((Session["PersonForReview"] != null)
                && (Session["ContentStream"] != null)
                && (Session["PersonForReview"].GetType() == typeof(MembershipPerson))
                && (Session["ContentStream"].GetType() == typeof(byte[])))
            {
                OpenCvSharp.IplImage img =  support.ByteArrayToIplImage(Session["ContentStream"] as byte[], OpenCvSharp.LoadMode.Unchanged);
                if (img == null)
                    return res;
                img = ((OpenCvSharp.CPlusPlus.Mat)OpenCvSharp.Cv.EncodeImage(".jpg", img)).ToIplImage();
                byte[] buf = support.IplImageToByteArray(img);
                Session["ContentStream"] = buf;
                Session["ContentLength"] = buf.Length;
                (Session["PersonForReview"] as MembershipPerson).person.Avatar = buf;
            }
            return res;
        }

        /// <summary>
        /// Upload photo and detect a face
        /// </summary>
        /// <param name="id">not used</param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult UploadFace(int? id)
        {
            ActionResult res = AjaxSubmit(id);
//             string message = System.Web.HttpContext.Current.Request.MapPath(System.Web.HttpContext.Current.Request.ApplicationPath);
//             Response.Write("<script language=VBScript>MsgBox " + message + "</script>");
            if ((Session["ContentStream"] != null) && 
                (Session["ContentStream"].GetType() == typeof(byte[])))
            {
                byte[] arr = (byte[])Session["ContentStream"];
                OpenCvSharp.IplImage[] imgs = null;
                OpenCvSharp.IplImage img = support.ByteArrayToIplImage(arr, OpenCvSharp.LoadMode.Unchanged);
                if (img != null)
                    imgs = HaarCascade.GetFaces(img);
                if (imgs != null && imgs.Length > 0)
                {
                    Session["ContentStream"] = support.IplImageToByteArray(imgs[0]);
                    Session["ContentLength"] = imgs[0].ImageSize;
                }
                else
                {
                    Session["ContentLength"] = null;
                    Session["ContentStream"] = null;

                    Response.Write("error");
                 
                    return null;
                }
                OpenCvSharp.Cv.ReleaseImage(img);
            }
            return res;
        }

        /// <summary>
        /// Save data
        /// </summary>
        /// <param name="id">not used</param>
        /// <returns></returns>
        public ActionResult AjaxSubmit(int? id)
        {
            //If file size == 0 we do not needed to upload this file
            if (Request.Files[0].ContentLength != 0)
            {
                Session["ContentLength"] = Request.Files[0].ContentLength;
                Session["ContentType"] = Request.Files[0].ContentType;
                byte[] b = new byte[Request.Files[0].ContentLength];
                Request.Files[0].InputStream.Read(b, 0, Request.Files[0].ContentLength);
                Session["ContentStream"] = b;
            }
            else
            {
                Session["ContentLength"] = null;
                Session["ContentType"] = null;
                Session["ContentStream"] = null;
                Response.Close();
                throw new Exception();
            }
            return Content(Request.Files[0].ContentType + ";" + Request.Files[0].ContentLength);
        }
        #endregion

        #region ImageLoad
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult ImageLoad(int? id)
        {
            byte[] b = (byte[])Session["ContentStream"];
            int length = (int)Session["ContentLength"];
            string type = (string)Session["ContentType"];
            if (b == null || b.Length == 0 || length == 0)
            {
                Session["ContentLength"] = null;
                Session["ContentType"] = null;
                Session["ContentStream"] = null;
                Response.End();
                return Content("");
            }
            Response.Buffer = true;
            Response.Charset = "";
            Response.Cache.SetCacheability(System.Web.HttpCacheability.NoCache);
            Response.ContentType = type;
            Response.BinaryWrite(b);
            Response.Flush();
            Response.End();
            return Content("");
        }
        #endregion

        //
        // GET: /Account/LogOn
        // 
        public ActionResult LogOnByUserName()
        {
            return View();
        }

        //
        // POST: /Account/LogOn

        [HttpPost]
        public ActionResult LogOnByUserName(LogOnByUserNameModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (MembershipService.ValidateUser(model.UserName, model.Password))
                {
                    Session["ContentLength"] = null;
                    Session["ContentType"] = null;
                    Session["ContentStream"] = null;
                    Session["CurrentUser"] = MembershipService.GetUser(model.UserName);
                    FormsService.SignIn(model.UserName, model.RememberMe);
                    if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                        && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                    {
                        return Redirect(returnUrl);
                    }
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "The user name or password provided is incorrect.");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }


        //
        // GET: /Account/LogOn

        public ActionResult LogOnByPhoto()
        {
            return View();
        }

        //
        // POST: /Account/LogOn

        [HttpPost]
        public ActionResult LogOnByPhoto(LogOnByPhotoModel model, string returnUrl)
        {
            model.PhotoStream = (byte[])Session["ContentStream"];
            string userName = null;
            double runTime = -1;
            try
            {
                if (MembershipService.ValidateUser(model.PhotoStream, out userName, model.Algorithm, out runTime))
                {
                    model.UserName = userName;
                    Session["CurrentUser"] = MembershipService.GetUser(model.UserName);
                    Session["LastRecognitionAlgorithm"] = Enum.GetName(typeof(support.Algorithm), model.Algorithm);
                    Session["LastRecognitionTime"] = runTime;
                    FormsService.SignIn(model.UserName, model.RememberMe);
                    if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                        && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                    {
                        return Redirect(returnUrl);
                    }
                    return RedirectToAction("Index", "Home");
                }
            }
            catch
            { }
            finally
            {
                Session["ContentLength"] = null;
                Session["ContentType"] = null;
                Session["ContentStream"] = null;
            }
            ModelState.AddModelError("", "The user not found.");
            //Response.BinaryWrite((byte[])Session["ContentStream"]);
            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public ActionResult InformationUpdate(UpdateUserModel model, string returnUrl)
        {
            if ((Session["PersonForReview"] != null) && (Session["PersonForReview"].GetType() == typeof(MembershipPerson)))
            {
                MembershipPerson person = Session["PersonForReview"] as MembershipPerson;
                person.person.Gender = model.Gender;
                person.person.Name = model.Name; 
                if ((Session["PersonForReview"] as MembershipPerson).person.PersonID != 0)
                {
                    // usr.Avatar = Session["ContentStream"] == null ? null : (Session["ContentStream"] as byte[]);
                    MembershipService.UpdateUser(Session["PersonForReview"] as MembershipPerson);
                }
                else
                    if ((Session["PersonForReview"] as MembershipPerson).person.PersonID == 0)
                        MembershipService.CreatePerson(Session["PersonForReview"] as MembershipPerson);
                return View("../Home/InformationReview", model);
            }
            return null;
        }


        //
        // GET: /Account/LogOff

        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/Register

        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register

        [HttpPost]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                MembershipCreateStatus createStatus;
                MembershipPerson usr = MembershipService.CreateUser(model.UserName, model.Password, model.Email, model.Name, model.Gender, out createStatus) as MembershipPerson;
                #region DEBUG
#if DEBUG
//                 usr.Photos.Add(new Entities.Photo());
//                 usr.Photos[0].PhotoStream = new byte[] { 1, 2, 3 };
//                 usr.Photos[0].UserID = (int)usr.ProviderUserKey;
#endif
                #endregion
                MembershipService.UpdateUser(usr);
                if (createStatus == MembershipCreateStatus.Success)
                {
                    Session["CurrentUser"] = MembershipService.GetUser(model.UserName);
                    FormsService.SignIn(model.UserName, false /* createPersistentCookie */);
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", ErrorCodeToString(createStatus));
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ChangePassword

        [Authorize]
        public ActionResult ChangePassword()
        {
            return View();
        }

        // ChangePassword method not implemented in CustomMembershipProvider.cs
        // Feel free to update!

        //
        // POST: /Account/ChangePassword

        [Authorize]
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {

                // ChangePassword will throw an exception rather
                // than return false in certain failure scenarios.
                bool changePasswordSucceeded;
                try
                {
                    MembershipUser currentUser = Membership.GetUser(User.Identity.Name, true /* userIsOnline */);
                    changePasswordSucceeded = currentUser.ChangePassword(model.OldPassword, model.NewPassword);
                }
                catch (Exception)
                {
                    changePasswordSucceeded = false;
                }

                if (changePasswordSucceeded)
                {
                    return RedirectToAction("ChangePasswordSuccess");
                }
                else
                {
                    ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ChangePasswordSuccess

        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }

        #endregion



        #region Status Codes
        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion
    }
}
