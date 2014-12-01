using FaceRecognitionSystem.ImageProcessing;
using System.Web.Security;

namespace FaceRecognitionSystem.Interfaces
{
    public interface IMembershipService
    {
        int MinPasswordLength { get; }

        bool ValidateUser(string userName, string password);
        bool ValidateUser(byte[] photo, out string userName, support.Algorithm algorithm, out double runTime);
        MembershipUser CreateUser(string userName, string password, string email, string name, string gender, out MembershipCreateStatus status);
        void UpdateUser(FaceRecognitionSystem.Infrastructure.MembershipPerson person);
 //       void UpdatePerson(FaceRecognitionSystem.Infrastructure.MembershipPerson person);
        MembershipUser CreatePerson(FaceRecognitionSystem.Infrastructure.MembershipPerson person);
        bool ChangePassword(string userName, string oldPassword, string newPassword);
        MembershipUser GetUser(string userName);
        MembershipUser GetUser(int personNo);
        MembershipUser GetPerson(int personNo);
        System.Collections.Generic.Dictionary<int, System.Collections.Generic.List<byte[]>> GetAllPhotos();
    }
}
