using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Foundry;

namespace Foundry
{
    class UsageExamples
    {
        public enum CategoriesTest
        {
            Add,
            Update,
            GetWithLabels,
            GetWithoutLabels,
            GetById
        }

        public enum LabelsTest
        {
            Add,
            Update,
            Delete,
            GetById
        }

        public enum UserTest
        {

        }

        public enum LocationTest
        {

        }


        static void Main(string[] args)
        {
            string client_id = "ydWpq0jt4k8tsgAovEUQf8c4FOYHVyr1uMkkZHaMTp4";
            string client_secret = "7RkwoL2IAMcOAgeMdItlBMg7dch7ZdQ7rzyaXRSAwm8";

            API foundry = new API(accountSid: client_id, secretKey: client_secret); // create client -- auth done in backend -- location list updated in the backend as well

            //TestCategories(api: foundry, option: CategoriesTest.GetById);
            //TestLabels(api: foundry, option: LabelsTest.Delete);

             Console.ReadLine();
        }

        public static void TestCategories(API api, CategoriesTest option)
        {
            switch (option)
            {
                case CategoriesTest.Add:
                    Category newCategory = new Category
                    {
                        Name = "SDK Category"
                    };
                    newCategory = api.AddCategory(newCategory);
                    Console.WriteLine(API.CategoryJson(newCategory));
                    break;
 
                case CategoriesTest.GetWithLabels:
                    List<Category> categoriesWithLabels = api.GetCategories(WithLabels: true);
                    foreach (Category cat in categoriesWithLabels)
                    {
                        Console.WriteLine(API.CategoryJson(cat));
                    }
                    break;

                case CategoriesTest.GetWithoutLabels:
                    List<Category> categoriesWithoutLabels = api.GetCategories(WithLabels: false);
                    foreach (Category cat in categoriesWithoutLabels)
                    {
                        Console.WriteLine(API.CategoryJson(cat));
                    }
                    break;

                case CategoriesTest.GetById:
                    Category myCategory = api.GetCategoryById("1114", WithLabels: true);
                    Console.WriteLine(API.CategoryJson(myCategory));
                    break;
            }     
        }

        public static void TestLabels(API api, LabelsTest option)
        {
            switch(option)
            {
                case LabelsTest.Add:
                    // Create a new label
                    Label newLabel = new Label
                    {
                        Name = "Label2 for DEF"
                    };

                    // Find the category you want to add the label to
                    Category myCategory = api.GetCategoryById("1114", WithLabels: false);
                    // If you don't know the ID, you can always use api.GetCategories() to get the entire list and find which one to add the label to

                    // Add the label
                    newLabel = api.AddLabel(myCategory, newLabel); // newLabel.Id = "2910"

                    break;

                case LabelsTest.Delete:
                    Label labelToDelete = api.GetLabelById("2911");
                    api.DeleteLabel(labelToDelete);

                    break;

                case LabelsTest.GetById:
                    Label myLabel = api.GetLabelById("2910");
                    Console.WriteLine(API.LabelJson(api.GetCategoryById("1114", false), myLabel));
                    break;

                case LabelsTest.Update:
                    // Get existing label and change it's name
                    Label label = api.GetLabelById("2910");
                    label.Name = "C# SDK Updated My Name!";

                    // Update the label
                    label = api.UpdateLabel(label);
                    
                    break;
            }
        }

        public static void OtherTests()
        {
            // Creating a new user
            /*User user = new User
            {
                FirstName = "Testing",
                LastName = "From .NET",
                Email = "6260959@everfi.com",
                Position = "Student"
            };
            user.UserTypes.Add(new UserType(UserRole.FacStaffNonSupervisor));

            user = foundry.AddUser(user);

            User retrieved = foundry.GetUserById(user.UserId);
            Console.WriteLine(retrieved.GetJson()); */


            // Getting all users (caller paging)
            /*List<User> myUsers = new List<User>();
            bool keepGoing = true;

            while (keepGoing)
            {
                List<User> currentUsers = new List<User>();
                (currentUsers, keepGoing) = foundry.GetUsers();
                myUsers = myUsers.Concat(currentUsers).ToList();
            }*/

            // Getting the first 300 (or less) users
            /*List<User> my300Users = new List<User>();
            bool keepGoing = true;
            int i = 0;
            while (i < 3 && keepGoing)
            {
                List<User> retrievedUsers = new List<User>();
                (retrievedUsers, keepGoing) = foundry.GetUsers();
                my300Users = my300Users.Concat(retrievedUsers).ToList();
            }*/

            // If you want to continue from that point, create another loop
            // Otherwise, call foundry.ResetGetUsers(); and then loop.

            //string LocationId = foundry.GetLocations().ElementAt(0).Id;
            // or you can get all locations and figure out which id to assign to a particular user
            //User user = new User();
            //user.LocationId = LocationId;

            /*Location location = new Location
            {
                Name = "Test Location 2",
                ContactEmail = "ajaiman@everfi.com",
                ContactName = "Aman Jaiman",
                ContactPhone = "3019190324",
                StreetNumber = "1111",
                Route = "High Point Lane",
                City = "Some City",
                County = "Montgomery County",
                State = "MD",
                PostalCode = "20347",
                Country = "United States",
                CreatedAt = DateTime.Now
            };

            Console.WriteLine(location.ToJson());*/

            //location = foundry.AddLocation(location);

            /*List<Location> locations = foundry.GetLocations();

            foreach (Location loc in locations)
            {
                Console.WriteLine(loc.ToJson());
            }*/

            /*Location loc = foundry.GetLocationById("15751");
            Console.WriteLine(loc.ToJson());*/

            /*Location location = foundry.GetLocations().ElementAt(0);
            Console.WriteLine(location.ToJson());
            Console.ReadLine();
            location.Name = "Test Location 1 (UPDATED)";
            location.AddressName = "Fake Location";
            foundry.UpdateLocation(location);
            location = foundry.GetLocations().ElementAt(0);
            Console.WriteLine(location.ToJson());*/

            /*List<Location> locationsList = foundry.GetLocations();
            List<string> locationId = new List<string>();

            foreach (Location loc in locationsList)
            {
                locationId.Add(loc.Id);
            }*/

            /*List<User> allUsers = new List<User>();
            bool keepGoing = true;

            while (keepGoing)
            {
                List<User> retrievedUserOnPage = new List<User>();
                (retrievedUserOnPage, keepGoing) = foundry.GetUsers();

                allUsers = allUsers.Concat(retrievedUserOnPage).ToList();
            }

            Console.WriteLine("Total users in list: " + allUsers.Count);*/

            /*User user = new User
            {
                FirstName = "Testing",
                LastName = "From .NET",
                Email = "6260959@everfi.com",
                Location = foundry.GetLocationById("15751")
            };
            user.UserTypes.Add(new UserType(UserRole.FacStaffNonSupervisor));*/

            /*List<Location> validLocations = foundry.GetLocations();

            User amanDuplicate = new User
            {
                FirstName = "Aman",
                LastName = "Jaiman",
                Email = "aman@duplicate.com",
                Location = validLocations.ElementAt(0)
            };
            amanDuplicate.UserTypes.Add(new UserType(UserRole.UndergraduateHE));*/

            //foundry.AddUser(amanDuplicate);


            // Searching:
            /*List<User> myUsers = new List<User>();

            Dictionary<SearchTerms, string> search = new Dictionary<SearchTerms, string>();
            search.Add(SearchTerms.FirstName, "Aman");

            myUsers = foundry.GetUsersBySearch(search);

            Console.WriteLine(myUsers.Count);
            foreach (User u in myUsers)
            {
                Console.WriteLine(u.ToJson());
            }*/
        }
    }
}
