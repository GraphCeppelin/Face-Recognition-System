using System.Data.Entity;
using System.Linq;
using FaceRecognitionSystem.Entities;
using System.Collections.Generic;

namespace FaceRecognitionSystem.Contexts
{
    public class DBContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Person> People { get; set; }
        public DbSet<Photo> Photos { get; set; }

        public DBContext()
        {

        }

        // Helper methods. User can also directly access "Users" property
        public void AddPerson(Person person)
        {
            People.Add(person);
            SaveChanges();
        }
        // Helper methods. User can also directly access "Users" property
        public void AddUser(User user)
        {
            Users.Add(user);
            SaveChanges();
        }

        // Helper methods. User can also directly access "Users" property
        public void AddPhoto(Photo photo)
        {
            Photos.Add(photo);
            SaveChanges();
        }

        private void GetPhotos(ref User usr)
        {
            int uid = usr.UserID;
            var photos = Photos.Select(x => x).Where(x => x.PersonID == uid);
            foreach (var p in photos)
                usr.Person.Photos.Add(p);
        }

        public User GetUser(int personID)
        {
            var user = Users.SingleOrDefault(u => u.Person.PersonID == personID);
          //  if (user != null)
          //      GetPhotos(ref user);
            return user;
        }

        public User GetUser(string userName)
        {
            var user = Users.SingleOrDefault(u => u.UserName == userName);
          //  if (user != null)
           //     GetPhotos(ref user);
            return user;
        }

        public User GetUser(string userName, string password)
        {
            int uss = Users.Count();
            var user = Users.SingleOrDefault(u => u.UserName == userName && u.Password == password);
         //   if (user != null)
         //       GetPhotos(ref user); 
            return user;
        }

        public Person GetPerson(int id)
        {
            var person = People.SingleOrDefault(p => p.PersonID == id);
            return person;
        }


        public Dictionary<int, List<byte[]>> GetAllPersonsPhotos()
        {
            return Photos.GroupBy(x => x.PersonID).ToDictionary(gdc => gdc.Key, gdc => gdc.Select(x => x.PhotoStream).ToList());
        }
    }
}
