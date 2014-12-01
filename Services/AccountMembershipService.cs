using System;
using System.Web.Security;
using FaceRecognitionSystem.Interfaces;
using FaceRecognitionSystem.Infrastructure;
using System.Collections.Generic;

namespace FaceRecognitionSystem.Services
{
    public class AccountMembershipService : IMembershipService
    {
        private readonly MembershipProvider _provider;

        public AccountMembershipService()
            : this(null)
        {
        }

        public AccountMembershipService(MembershipProvider provider)
        {
            _provider = provider ?? Membership.Provider;
        }

        public int MinPasswordLength
        {
            get
            {
                return _provider.MinRequiredPasswordLength;
            }
        }

        public bool ValidateUser(byte[] photo, out string userName, ImageProcessing.support.Algorithm algorithm, out double runTime)
        {
            if (photo == null) throw new ArgumentException("Value cannot be null or empty.", "photo");

            return (_provider as CustomMembershipProvider).ValidateUser(photo, out userName, algorithm, out runTime);
        }

        public bool ValidatePerson(byte[] photo, out MembershipPerson person, ImageProcessing.support.Algorithm algorithm, out double runTime)
        {
            if (photo == null) throw new ArgumentException("Value cannot be null or empty.", "photo");

            return (_provider as CustomMembershipProvider).ValidatePerson(photo, out person, algorithm, out runTime);
        }

        public bool ValidateUser(string userName, string password)
        {
            if (String.IsNullOrEmpty(userName)) throw new ArgumentException("Value cannot be null or empty.", "userName");
            if (String.IsNullOrEmpty(password)) throw new ArgumentException("Value cannot be null or empty.", "password");

            return _provider.ValidateUser(userName, password);
        }

        public MembershipUser CreateUser(string userName, string password, string email, string name, string gender, out MembershipCreateStatus status)
        {
            if (String.IsNullOrEmpty(userName)) throw new ArgumentException("Value cannot be null or empty.", "userName");
            if (String.IsNullOrEmpty(password)) throw new ArgumentException("Value cannot be null or empty.", "password");
            if (String.IsNullOrEmpty(email)) throw new ArgumentException("Value cannot be null or empty.", "email");
            MembershipUser usr = (_provider as CustomMembershipProvider).CreateUser(userName, password, email, name, gender, null, null, true, null, out status);
            return usr;
        }

        public MembershipUser CreatePerson(MembershipPerson person)
        {
            MembershipUser usr = (_provider as CustomMembershipProvider).CreatePerson(person);
            return usr;
        }

        public void UpdateUser(FaceRecognitionSystem.Infrastructure.MembershipPerson usr)
        {
            (_provider as CustomMembershipProvider).UpdateUser(usr);
        }

//         public void UpdatePerson(FaceRecognitionSystem.Infrastructure.MembershipPerson person)
//         {
//             (_provider as CustomMembershipProvider).UpdatePerson(person);
//         }

        public MembershipUser GetUser(string userName)
        {
            MembershipUser usr = _provider.GetUser(userName, true);
            if (usr == null)
                _provider.GetUser(userName, false);
            return usr;
        }

        public MembershipUser GetUser(int personNo)
        {
            MembershipUser usr = _provider.GetUser(personNo, true);
            if (usr == null)
                _provider.GetUser(personNo, false);
            return usr;
        }

        public MembershipUser GetPerson(int personNo)
        {
            MembershipUser person =  (_provider as CustomMembershipProvider).GetPerson(personNo);
            if (person == null)
                (_provider as CustomMembershipProvider).GetPerson(personNo);
            return person;
        }

        public bool ChangePassword(string userName, string oldPassword, string newPassword)
        {
            if (String.IsNullOrEmpty(userName)) throw new ArgumentException("Value cannot be null or empty.", "userName");
            if (String.IsNullOrEmpty(oldPassword)) throw new ArgumentException("Value cannot be null or empty.", "oldPassword");
            if (String.IsNullOrEmpty(newPassword)) throw new ArgumentException("Value cannot be null or empty.", "newPassword");

            // The underlying ChangePassword() will throw an exception rather
            // than return false in certain failure scenarios.
            try
            {
                MembershipUser currentUser = _provider.GetUser(userName, true /* userIsOnline */);
                return currentUser.ChangePassword(oldPassword, newPassword);
            }
            catch (ArgumentException)
            {
                return false;
            }
            catch (MembershipPasswordException)
            {
                return false;
            }
        }

        public Dictionary<int, List<byte[]>> GetAllPhotos()
        {
            return (_provider as CustomMembershipProvider).GetAllTrainedData();
        }
    }
}