using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using FaceRecognitionSystem.Entities;

namespace FaceRecognitionSystem.Infrastructure
{
    public class MembershipPerson : MembershipUser
    {
        public Person person { get; set; }
        public bool isUser { get; private set; }

        public MembershipPerson(string providerName, string userName, byte[] avatar, string name, string gender, object providerUserKey, 
            string email, string passwordQuestion, string comment, 
            bool isApproved, bool isLockedOut, DateTime creationDate, DateTime 
            lastLoginDate, DateTime lastActivityDate, DateTime lastPasswordChangedDate, DateTime lastLockoutDate):
            base(providerName, userName, providerUserKey, email, passwordQuestion, comment, isApproved, isLockedOut, creationDate, lastLoginDate, lastActivityDate, lastPasswordChangedDate, lastLockoutDate)
        {
            person = new Person(avatar, gender, name);
        }

        public MembershipPerson(string providerName, User user, string passwordQuestion, string comment,
                bool isApproved, bool isLockedOut, DateTime creationDate, DateTime
                lastLoginDate, DateTime lastActivityDate, DateTime lastPasswordChangedDate, DateTime lastLockoutDate) :
            base(providerName, user.UserName, user.UserID, user.UserEmailAddress, passwordQuestion, comment, isApproved, isLockedOut, creationDate, lastLoginDate, lastActivityDate, lastPasswordChangedDate, lastLockoutDate)
        {
            isUser = false;
            if (user != null && user.UserID != 0)
            {
                person = user.Person;
                isUser = true;
            }
            else if (user != null && user.Person != null)
                person = user.Person;
            else
                person = new Person();
        }
    }
}