using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FaceRecognitionSystem.Infrastructure;
using FaceRecognitionSystem.Entities;
using FaceRecognitionSystem.Services;
using FaceRecognitionSystem.ImageProcessing;
using FaceRecognitionSystem.Models;
using OpenCvSharp;

namespace FaceRecognitionSystem.Controllers
{
    [HandleError]
    public class HomeController : Controller
    {
        #region Image handling

        /// <summary>
        /// View: PhotosManager. Remove image from database. This procedure is called by ajax from java script
        /// </summary>
        /// <param name="id">not used</param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RemoveImage(int? id)
        {
            int photoId = 0;
            if (!int.TryParse(Request["imageid"], out photoId))
                throw new Exception();
            if (Session["PersonForReview"] == null || (Session["PersonForReview"].GetType() != typeof(MembershipPerson)))
                throw new Exception();
            if ((Session["PersonForReview"] as MembershipPerson).person.Photos.RemoveAll(x => x.PhotoID == photoId) == 0)
                throw new Exception();
            (new AccountMembershipService()).UpdateUser(Session["PersonForReview"] as MembershipPerson);
            return null;
        }


        /// <summary>
        /// Convert buf array of image to URL /Home/ImageLoader?ImageID=...
        /// </summary>
        /// <returns></returns>
        public ActionResult ImageLoader()
        {
            FaceRecognitionSystem.Infrastructure.MembershipPerson usr = null;
            byte[] buf = null;
            if ((Request.QueryString["ShowComparedPhoto"] != null) && (Session["ComparedPhoto"] != null))
            {
                buf = Session["ComparedPhoto"] as byte[];
            }
            else
                if ((Request.QueryString["ImageID"] != null) && (Session["PersonForReview"] != null))
                {
                    usr = Session["PersonForReview"] as MembershipPerson;
                    if (Request.QueryString["ImageID"] == "Avatar")
                        buf = usr.person.Avatar;
                    else
                    {
                        FaceRecognitionSystem.Entities.Photo p = usr.person.Photos.Select(x => x)
                            .Where(x => x.PhotoID.ToString() == Request.QueryString["ImageID"]).First();
                        buf = p.PhotoStream;
                    }
                }
            if (buf != null)
            {
                Response.Buffer = true;
                Response.Charset = "";
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.ContentType = "image/jpeg"; ;
                Response.AddHeader("content-disposition", "attachment;filename=Photo_" + Request.QueryString["ImageID"]);
                Response.BinaryWrite(buf);
                Response.Flush();
                Response.End();
            }
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddPhoto(object model, string returnUrl)
        {
            if ((Session["PersonForReview"] != null)
                && (Session["ContentStream"] != null)
                && (Session["PersonForReview"].GetType() == typeof(MembershipPerson))
                && (Session["ContentStream"].GetType() == typeof(byte[])))
            {
                MembershipPerson usr = Session["PersonForReview"] as MembershipPerson;
                usr.person.Photos.Add(new Photo());
                //to grayscale

                IplImage img = support.ByteArrayToIplImage((byte[])Session["ContentStream"], OpenCvSharp.LoadMode.GrayScale);
                img = ((OpenCvSharp.CPlusPlus.Mat)Cv.EncodeImage(".jpg", img)).ToIplImage();
                usr.person.Photos[usr.person.Photos.Count - 1].PhotoStream = support.IplImageToByteArray(img);
                usr.person.Photos[usr.person.Photos.Count - 1].PersonID = (int)usr.person.PersonID;
                (new AccountMembershipService()).UpdateUser(usr);
                Session["ContentStream"] = null;
                Session["ContentLength"] = null;
                Session["ContentType"] = null;
                Cv.ReleaseImage(img);
            }
            return View("PhotosManager", model);
        }

        #endregion

        /// <summary>
        /// Show main page
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("LogOnByUserName", "Account");
            }

            ViewData["Message"] = "Welcome!";
            return View();
        }

        /// <summary>
        /// View about page
        /// </summary>
        /// <returns></returns>
        public ActionResult About()
        {
            return View();
        }

        /// <summary>
        /// Get the authenticated user info from session (or cookie if not found and write it into session)
        /// </summary>
        /// <returns></returns>
        public MembershipPerson GetCurrentUser()
        {
            if (Request.IsAuthenticated)
            {
                if (Session["CurrentUser"] == null)
                {
                    HttpCookie authCookie = Request.Cookies[System.Web.Security.FormsAuthentication.FormsCookieName];
                    System.Web.Security.FormsAuthenticationTicket ticket = System.Web.Security.FormsAuthentication.Decrypt(authCookie.Value);

                    //                 string cookiePath = ticket.CookiePath;
                    //                 DateTime expiration = ticket.Expiration;
                    //                 bool expired = ticket.Expired;
                    //                 bool isPersistent = ticket.IsPersistent;
                    //                 DateTime issueDate = ticket.IssueDate;
                    //                 string name = ticket.Name;
                    //                 string userData = ticket.UserData;
                    //                 string version = ticket.Version.ToString();

                    Session["CurrentUser"] = (new FaceRecognitionSystem.Services.AccountMembershipService()).GetUser(ticket.Name);
                }
                return Session["CurrentUser"] as MembershipPerson;
            }
            return null;
        }

        /// <summary>
        /// Update details of the authenticated user 
        /// </summary>
        /// <returns></returns>
        public ActionResult UpdateMyInformation()
        {
            if (!Request.IsAuthenticated)
                return RedirectToAction("LogOnByUserName", "Account");
            Session["PersonForReview"] = GetCurrentUser();
            Session["ComparedPhoto"] = null;
            InformationReview();
            return View("InformationReview");
        }

        /// <summary>
        /// Update details of the authenticated user 
        /// </summary>
        /// <returns></returns>
        public ActionResult AddNewPerson()
        {
            if (!Request.IsAuthenticated)
                return RedirectToAction("LogOnByUserName", "Account");
            Session["ComparedPhoto"] = null;
            Session["PersonForReview"] = new MembershipPerson("CustomMembershipProvider", new User(),
                                                            string.Empty, string.Empty,
                                                            true, false, DateTime.MinValue,
                                                            DateTime.MinValue,
                                                            DateTime.MinValue,
                                                            DateTime.Now, DateTime.Now);
            InformationReview();
            return View("InformationReview");
        }


        /// <summary>
        /// Update details of the authenticated user 
        /// </summary>
        /// <returns></returns>
        public ActionResult PersonSearch()
        {
            if (!Request.IsAuthenticated)
                return RedirectToAction("LogOnByUserName", "Account");
            return View("PersonSearch");
        }

        public ActionResult SearchByPersonID()
        {
            if (!Request.IsAuthenticated)
                return RedirectToAction("LogOnByUserName", "Account");
            Session["ContentStream"] = null;
            Session["ContentLength"] = null;
            Session["ContentType"] = null;
            Session["ComparedPhoto"] = null;
            int personID = 0;
            if (int.TryParse(Request.Form["PersonID"].ToString(), out personID))
            {
                MembershipPerson person = (new AccountMembershipService()).GetPerson(personID) as MembershipPerson;
                if (person != null)
                {
                    Session["PersonForReview"] = person;
                    return View("InformationReview");
                }
            }
            ModelState.AddModelError("", "No record found.");
            return View("PersonSearch");
        }

        [HttpPost]
        public ActionResult SearchByPhoto(PeopleSearchModel model, string returnUrl)
        {
            if (!Request.IsAuthenticated)
                return RedirectToAction("LogOnByUserName", "Account");
            Session["ComparedPhoto"] = null;
            byte[] userPhoto = (byte[])Session["ContentStream"];
            double runTime = -1;
            try
            {
                if (Request.Form["Algorithm"] == null)
                    throw new Exception();
                support.Algorithm alg = support.Algorithm.EigenFaces;
                if (!Enum.TryParse<support.Algorithm>(Request.Form["Algorithm"].ToString(), out alg))
                    throw new Exception();
                AccountMembershipService service = new AccountMembershipService();
                MembershipPerson person = null;
                if (service.ValidatePerson(userPhoto, out person, alg, out runTime))
                {
                    Session["LastRecognitionAlgorithm"] = Enum.GetName(typeof(support.Algorithm), alg);
                    Session["LastRecognitionTime"] = runTime;
                    if (person != null)
                    {
                        Session["PersonForReview"] = person;
                        Session["ComparedPhoto"] = userPhoto;
                        return View("PersonSearch");
                    }
                }
            }
            catch
            { }
            finally
            {
//                 Session["ContentStream"] = null;
//                 Session["ContentLength"] = null;
//                 Session["ContentType"] = null;
            }
            ModelState.AddModelError("", "No record found.");
            return View("PersonSearch");
        }


        public ActionResult InformationReview()
        {
            if (!Request.IsAuthenticated)
                return RedirectToAction("LogOnByUserName", "Account");
            Session["ContentStream"] = null;
            Session["ContentLength"] = null;
            Session["ContentType"] = null;
            return View();
        }


        public ActionResult PhotosManager()
        {
            if (!Request.IsAuthenticated)
                return RedirectToAction("LogOnByUserName", "Account");
            Session["ContentStream"] = null;
            Session["ContentLength"] = null;
            Session["ContentType"] = null;
            return View();
        }

    }
}
