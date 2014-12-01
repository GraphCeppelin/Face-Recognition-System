using System.Collections.Generic;
namespace FaceRecognitionSystem.Entities
{

    public class Photo
    {
        public int PhotoID { get; set; }
        public int PersonID { get; set; }
        public byte[] PhotoStream { get; set; }

        public virtual Person Person { get; set; }
    }

    public class Person
    {
        public int PersonID { get; set; }
        public byte[] Avatar { get; set; }
        public string Name { get; set; }
        public string Gender { get; set; }

        public Person()
        {
            this.Photos = new List<Photo>();
        }
        
        public Person(byte[] Avatar, string Name, string Gender)
        {
            this.Avatar = Avatar;
            this.Name = Name;
            this.Gender = Gender;
            this.Photos = new List<Photo>();
        }

        public virtual List<Photo> Photos { get; set; }

    }

    public class User
    {
        public int UserID { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string UserEmailAddress { get; set; }

        public User()
        {
        }

        public User(string UserName,  string Password, string UserEmailAddress, byte[] Avatar, string Name, string Gender)
        {
            this.UserID = UserID;
            this.UserName = UserName;
            this.Password = Password;
            this.UserEmailAddress = UserEmailAddress;
            Person = new Person(Avatar, Name, Gender);
        }

        public int PersonID { get; set; }
        public virtual Person Person { get; set; }
    }
}