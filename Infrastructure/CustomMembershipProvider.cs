using System;
using System.Security.Cryptography;
using System.Text;
using System.Web.Security;
using FaceRecognitionSystem.Contexts;
using FaceRecognitionSystem.Entities;
using FaceRecognitionSystem.Infrastructure;
using System.Linq;
using System.Web.UI.MobileControls;
using System.Collections.Generic;
using FaceRecognitionSystem.ImageProcessing;

namespace FaceRecognitionSystem.Infrastructure
{
    public class CustomMembershipProvider : MembershipProvider
    {
        public override string ApplicationName
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            throw new NotImplementedException();
        }

        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            throw new NotImplementedException();
        }

        public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
        {
            throw new NotImplementedException();
        }

        public MembershipUser CreateUser(string username, string password, string email, string name, string gender, string passwordQuestion,
            string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
        {
            var args = new ValidatePasswordEventArgs(username, password, true);
            OnValidatingPassword(args);

            if (args.Cancel)
            {
                status = MembershipCreateStatus.InvalidPassword;
                return null;
            }

            if (RequiresUniqueEmail && GetUserNameByEmail(email) != string.Empty)
            {
                status = MembershipCreateStatus.DuplicateEmail;
                return null;
            }

            var user = GetUser(username, true);

            if (user == null)
            {
                var userObj = new User(username, GetMd5Hash(password), email, null, name, gender);

                using (var dbContext = new DBContext())
                {
                    dbContext.AddUser(userObj);
                }

                status = MembershipCreateStatus.Success;

                return GetUser(username, true);
            }
            status = MembershipCreateStatus.DuplicateUserName;

            return null;
        }

        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            throw new NotImplementedException();
        }

        public override bool EnablePasswordReset
        {
            get { throw new NotImplementedException(); }
        }

        public override bool EnablePasswordRetrieval
        {
            get { throw new NotImplementedException(); }
        }

        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override int GetNumberOfUsersOnline()
        {
            throw new NotImplementedException();
        }

        public override string GetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }

        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            var dbContext = new DBContext();
            var user = dbContext.GetUser(username);
            if (user != null)
            {
                var memUser = new MembershipPerson("CustomMembershipProvider", user,
                                                            string.Empty, string.Empty,
                                                            true, false, DateTime.MinValue,
                                                            DateTime.MinValue,
                                                            DateTime.MinValue,
                                                            DateTime.Now, DateTime.Now);
                return memUser;
            }
            return null;
        }

        public override MembershipUser GetUser(object providerPersonKey, bool userIsOnline)
        {
            var dbContext = new DBContext();
            var user = dbContext.GetUser((int)providerPersonKey);
            if (user != null)
            {
                var memUser = new MembershipPerson("CustomMembershipProvider", user,
                                                            string.Empty, string.Empty,
                                                            true, false, DateTime.MinValue,
                                                            DateTime.MinValue,
                                                            DateTime.MinValue,
                                                            DateTime.Now, DateTime.Now);
                return memUser;
            }
            return null;
        }

        public MembershipPerson GetPerson(object providerPersonKey)
        {
            var dbContext = new DBContext();
            var person = dbContext.GetPerson((int)providerPersonKey);
            User usr = new User();
            usr.Person = person;
            if (person != null)
            {
                var memPerson = new MembershipPerson("CustomMembershipProvider", usr,
                                                            string.Empty, string.Empty,
                                                            true, false, DateTime.MinValue,
                                                            DateTime.MinValue,
                                                            DateTime.MinValue,
                                                            DateTime.Now, DateTime.Now);
                return memPerson;
            }
            return null;
        }

        public override string GetUserNameByEmail(string email)
        {
            throw new NotImplementedException();
        }

        public override int MaxInvalidPasswordAttempts
        {
            get { throw new NotImplementedException(); }
        }

        public override int MinRequiredNonAlphanumericCharacters
        {
            get { throw new NotImplementedException(); }
        }

        public override int MinRequiredPasswordLength
        {
            get { return 6; }
        }

        public override int PasswordAttemptWindow
        {
            get { throw new NotImplementedException(); }
        }

        public override MembershipPasswordFormat PasswordFormat
        {
            get { throw new NotImplementedException(); }
        }

        public override string PasswordStrengthRegularExpression
        {
            get { throw new NotImplementedException(); }
        }

        public override bool RequiresQuestionAndAnswer
        {
            get { throw new NotImplementedException(); }
        }

        public override bool RequiresUniqueEmail
        {
            get { return false; }
        }

        public override string ResetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }

        public override bool UnlockUser(string userName)
        {
            throw new NotImplementedException();
        }

        public override void UpdateUser(MembershipUser _user)
        {
            var dbContext = new DBContext();
            MembershipPerson personData = (_user as MembershipPerson);
            User usr = dbContext.GetUser(personData.person.PersonID);
            if (usr == null)
            {
                usr = new User();
                usr.Person = dbContext.GetPerson(personData.person.PersonID);
                if (usr.Person == null)
                    return;
            }
            usr.Person.Avatar = personData.person.Avatar;
            usr.Person.Name = personData.person.Name;
            usr.Person.Gender = personData.person.Gender;
            usr.UserName = personData.UserName;
            usr.UserEmailAddress = personData.Email;

            IQueryable<Photo> photos = (from p in dbContext.Photos where p.PersonID == usr.Person.PersonID select p);
            foreach (Photo p in photos)
            {
                System.Collections.Generic.IEnumerable<Photo> ps = personData.person.Photos.Select(x => x).Where(x => x.PhotoID == p.PhotoID);
                if (ps.Count() == 0)
                    dbContext.Entry(p).State = System.Data.EntityState.Deleted;
                else
                {
                    p.PhotoStream = ps.First().PhotoStream;
                }
            }
            IEnumerable<Photo> photosToAdd = personData.person.Photos.Where(x => x.PhotoID == 0).Select(x => x);
            foreach (Photo p in photosToAdd)
            {
                dbContext.AddPhoto(p);
            }
            dbContext.SaveChanges();
        }


        public MembershipPerson CreatePerson(MembershipPerson person)
        {
            var dbContext = new DBContext();
            dbContext.AddPerson(person.person);
            return GetPerson(person.person.PersonID);
        }


        //         public void UpdatePerson(MembershipUser _user)
        //         {
        //             var dbContext = new DBContext();
        //             MembershipPerson user = (_user as MembershipPerson);
        //             var person = dbContext.GetPerson(user.person.PersonID);
        //             person.Avatar = user.person.Avatar;
        //             person.Name = user.person.Name;
        //             person.Gender = user.person.Gender;
        //             IQueryable<Photo> photos = (from p in dbContext.Photos where p.PersonID == person.PersonID select p);
        //             foreach (Photo p in photos)
        //             {
        //                 System.Collections.Generic.IEnumerable<Photo> ps = user.photos.Select(x => x).Where(x => x.PhotoID == p.PhotoID);
        //                 if (ps.Count() == 0)
        //                     dbContext.Entry(p).State = System.Data.EntityState.Deleted;
        //                 else
        //                 {
        //                     p.PhotoStream = ps.First().PhotoStream;
        //                 }
        //             }
        //             IEnumerable<Photo> photosToAdd = user.photos.Where(x => x.PhotoID == 0).Select(x => x);
        //             foreach (Photo p in photosToAdd)
        //             {
        //                 dbContext.AddPhoto(p);
        //             }
        //             dbContext.SaveChanges();
        //         }

        public override bool ValidateUser(string username, string password)
        {
            var md5Hash = GetMd5Hash(password);

            using (var dbContext = new DBContext())
            {
                var requiredUser = dbContext.GetUser(username, md5Hash);
                return requiredUser != null;
            }
        }

        public bool ValidateUser(byte[] photo, out string userName, support.Algorithm algorithm, out double runTime)
        {
            userName = null;
            string err = "";
            int predictedPersonID = -1;
            runTime = -1;
            using (var dbContext = new DBContext())
            {
                Dictionary<int, List<byte[]>> photoStreams = dbContext.GetAllPersonsPhotos();
                DateTime timeStart = DateTime.Now;
                if (photoStreams != null &&
                    photoStreams.Count != 0 &&
                    photo != null &&
                    (algorithm == support.Algorithm.EigenFaces ?
                        EigenFaces.ProcessImageSample(out err, out predictedPersonID, photoStreams, photo) :
                        FisherFaces.ProcessImageSample(out err, out predictedPersonID, photoStreams, photo)) &&
                    predictedPersonID >= 0)
                {
                    runTime = (DateTime.Now - timeStart).TotalSeconds;
                    var requiredUser = dbContext.GetUser(predictedPersonID);
                    userName = requiredUser.UserName;
                    return requiredUser != null;
                }
            }
            return false;
        }

        public bool ValidatePerson(byte[] photo, out MembershipPerson person, support.Algorithm algorithm, out double runTime)
        {
            person = null;
            string err = "";
            int predictedPersonID = -1;
            runTime = -1;
            using (var dbContext = new DBContext())
            {
                Dictionary<int, List<byte[]>> photoStreams = dbContext.GetAllPersonsPhotos();
                DateTime timeStart = DateTime.Now;
                if (photoStreams != null &&
                    photoStreams.Count != 0 &&
                    photo != null &&
                    (algorithm == support.Algorithm.EigenFaces ?
                        EigenFaces.ProcessImageSample(out err, out predictedPersonID, photoStreams, photo) :
                        FisherFaces.ProcessImageSample(out err, out predictedPersonID, photoStreams, photo)) &&
                    predictedPersonID >= 0)
                {
                    runTime = (DateTime.Now - timeStart).TotalSeconds;
                    person = GetPerson(predictedPersonID);
                    return person != null;
                }
            }
            return false;
        }

        public static string GetMd5Hash(string value)
        {
            var md5Hasher = MD5.Create();
            var data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(value));
            var sBuilder = new StringBuilder();
            for (var i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }

        public Dictionary<int, List<byte[]>> GetAllTrainedData()
        {
            return (new DBContext()).GetAllPersonsPhotos();
        }
    }
}