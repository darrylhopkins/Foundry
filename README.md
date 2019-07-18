# Foundry C# SDK
C# SDK for the [EVERFI](https://www.everfi.com) Foundry API

## General Info
The Foundry API allows EVERFI partners to manage their organization by adding users, tracking progress, and other features. This C# SDK makes it easy for .NET developers to use the Foundry API.
### Contents
+ [Create a Client](#Creating-a-client)
+ [Organization Locations](#Organization-Locations)
    + [Getting your location](#Getting-your-organizations-location)
    + [Creating a new location](#Creating-a-new-location)
    + [Adding a location](#Adding-a-location-to-your-organization)
    + [Updating a location](#Updating-an-existing-location)
+ [Organization Users](#Organization-Users)
    + [Create a new user](#Creating-a-new-user)
    + [Adding a user](#Adding-a-user-to-your-organization)
    + [Retrieving users](#Retrieving-users)
    + [Updating users](#Updating-an-existing-user)
+ [Categories and Labels](#Categories)

## Setup
To build the solution, you need a .NET developer environment.
```
$ cd Foundry
$ dotnet build
...
Build succeeded.
    0 Warning(s)
    0 Error(s)

Time Elapsed 00:00:02.43
```
The associated .dll file will be located in `/Foundry/bin/Debug`. When creating a new project, include the .dll file by adding a reference.

## Usage
You should get your client id and secret from the Admin Panel. More information can be found in the [public API documentation](https://api.fifoundry.net/v1).
### Creating a client
Once you have obtained your client id and secret, you can simply create a new instance of the API. The OAuth is taken care of in the backend.
```c#
// Your client id and secret below
// Rather than storing them in plaintext, you can store them with .NET's Secret Manager
string client_id = "";
string client_secret = "";

API foundry = new API(client_id, client_secret);
```
You can now interact with the API and all of it's functionalities.

## Organization Locations
An organization always has at least one location, and it can have multiple. If you are adding or updating users, you should provide the user's **Location** defined for the request. *(Note: When you load an new instance of the API, a list of valid Locations will already be updated in the backend.)*
### Getting your organization's location
If you want to work with locations or add locations to your users, you should retrieve all locations so you have a valid list to work with.
#### Getting all locations
```c#
List<Location> locationsList = foundry.GetLocations();
```
This is important because it also saves the valid Organization's Locations in the API.
#### Getting a location by id
If you already know the id of a particular location and want to retrieve more information about it, you can simply retrieve it by it's id.
```c#
string locationId = "15751";
Location loc = foundry.GetLocationById(locationId);
```
### Creating a new location
When creating a new location, it's important to note that you cannot add the Id property. A location's id is created when it is added to Foundry.
```c#
Location NewLocation = new Location
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
```
### Adding a location to your organization 
Like adding a user to your organization, you need to assign the new location to return value of the AddLocation function in order to update the location with it's location id.
```c#
newLocation = foundry.AddLocation(newLocation);
```
Once you add the location, the list of available locations in the API will also be updated.
### Updating an existing location
```c#
// Make some change(s) to the user (ex. location.Name = "Test Location (UPDATED)")
foundry.UpdateLocation(location)
```

## Organization Users
An organization will have users that are all assigned roles and types based on what they are doing in your organization. The API allows you to get information about a user, or add/update users.
### Creating a new user
There are four required components of any user: First Name, Last Name, Email, and at least one UserType.
```c#
User user = new User
{
    FirstName = "First",
    LastName = "Last",
    Email = "flast@everfi.com"
};
```
We have defined a UserType as it's own class, holding a user's UserRole, in order to make it easier to interact with them throughout the API. Here is an example of a **Faculty/Staff Learner** who is a **Nonsupervisor**.
```c#
user.UserTypes.Add(new UserType(UserRole.FacStaffNonSupervisor));

// All possible UserRoles: UndergraduateHE, GraduateHE, NonTraditionalHE, GreekHE, HEAdmin, FacStaffSupervisor, FacStaffNonSupervisor, FacStaffAdmin, CodeConductSupervisor, CodeConductNonSupervisor, CodeConductAdmin, AdultFinancialLearner, AdultFinancialAdmin, EventVolunteer, and EventManager.
```
The Location attribute, while not required but is recommended, allows Users to be added to specific organization locations. The location given to a User must be one that already exists in Foundry (retrieved from before, or created and then added to Foundry).
```c#
// Assigned to a particular location in your saved list:
user.Location = locationsList.ElementAt(0);
```
There are other attributes you can add to a user, which are not required: SSO Id, Student Id, Employee Id, Position, First Day, Last Day.
### Adding a user to your organization
```c#
user = foundry.AddUser(user);
```
You need to assign your original user to the AddUser function so that the user is updated with the new UserId, which won't exist until you add it to Foundry. You cannot add a UserId to a user yourself.
### Retrieving users
When retrieving users, you can choose to retrieve a single user by their UserId, or retrieve multiple using paging.
#### By UserId
```c#
string UserId = "58c836fd-ebe3-4533-aee4-7a6f1a064de9";
User retrievedUser = foundry.GetUserById(UserId);
```
#### Multiple users
The Foundry API is designed to return 100 users per page. Because of this, you can choose to access the users of your organization in different ways.
To get a specific page of users, you can specify the page number in the method call (Getting the first hundred: page = 1)
```c#
List<User> users = foundry.GetUsers(1);
```
You can also loop through and get some or all of the users of your organization. The paging is done by the caller, as the API only returns 100 users.
```c#
// Getting all users
List<User> allUsers = new List<User>();
bool keepGoing = true;

while (keepGoing)
{
    List<User> currentUsers = new List<User>();
    (currentUsers, keepGoing) = foundry.GetUsers(); // Returns the next 100 users and if there are more.
    myUsers = myUsers.Concat(currentUsers).ToList();
}
```
```c#
// Getting the first 300 (or less) users
List<User> my300Users = new List<User>();
bool keepGoing = true;
int i = 0;
while (i < 3 && keepGoing)
{
    List<User> retrievedUsers = new List<User>();
    (retrievedUsers, keepGoing) = foundry.GetUsers();
    my300Users = my300Users.Concat(retrievedUsers).ToList();
}
```
Once you have gotten the users you want, you can choose to reset the position of the paging by calling `foundry.ResetGetUsers()`. This allows you to reset the position so paging will begin at the beginning if `foundry.GetUsers()` is called again. Otherwise, you can simply continue paging. (Note, after reaching the end of the user list, the API will automatically reset the page.)
#### Searching for users
You can also use different terms to search for a user, or multiple users. To search with Foundry, you need to create a dictionary holding pairs of SearchTerms and the term (SearchTerms are an Enum that hold values such as FirstName, Email, StudentId, etc.).
```c#
List<User> myUsers = new List<User>();

Dictionary<SearchTerms, string> search = new Dictionary<SearchTerms, string>();
search.Add(SearchTerms.FirstName, "Aman");

myUsers = foundry.GetUsersBySearch(search);
```
### Updating an existing user
```c#
// Make some change(s) to the user (ex. user.Email = lastfirst@everfi.com)
foundry.UpdateUser(user);
```
When updating a user it's important to note that you cannot remove any of the required fields (i.e. there must be a name, email, and at least one UserRole), but you can still update them.

## Categories and Labels
Categories and Labels are ways you can group users of your organization. One organization can have multiple categories, and each category can have multiple labels. A single user can only have one label associated to them.

### Working with categories
Start by creating a new category you want to add to your organization. To create a category, all you need to do is assign a name.
```c#
Category newCategory = new Category
{
    Name = "New Category"
};
```
Once you have the category, you can add it to your organization. Assign the original object to the return value of the add function to update it with it's CategoryId.
```c#
newCategory = foundry.AddCategory(newCategory);
```
Now, you can update (the name) or delete your category using Foundry.
```c#
// Updating Category Name
newCategory.Name = "Updating using SDK";
newCategory = foundry.UpdateCategory(newCategory);

// Deleting Category
foundry.DeleteCategory(newCategory);
```
If you want to access the other categories you have in your organization, you can get all of them or a certain one by it's CategoryId. When retrieving categories, you can choose to retrieve them with or without it's associated list of labels. Simply set the **WithLabels** parameter to true or false
```c#
// All categories with list of labels
List<Category> categoriesWithLabels = foundry.GetCategories(WithLabels: true);
// You can access a categories labels using the .Labels property of a Category

// All categories without list of labels
List<Category> categoriesWithoutLabels = foundry.GetCategories(WithLabels: false);

// Single category by id
Category myCategory = foundry.GetCategoryById("1114", WithLabels: true);
```

### Working with labels
The process to working with labels is very similar to categories. Start by creating a label.
```c#
Label newLabel = new Label
{
    Name = "Label for New Category"
};
```
When you add a label, you need to specify which category the label is going to. We'll do this by finding a category by id.
```c#
// Find the category you want to add the label to
Category myCategory = foundry.GetCategoryById("1114", WithLabels: false);
// If you don't know the ID, you can always use foundry.GetCategories() to get the entire list and find which one to add the label to

// Add the label
newLabel = foundry.AddLabel(myCategory, newLabel); // Assigns newLabel.Id
```
Like with categories, you can update, delete, and retrieve a single label by id.
```c#
// Get label by Id
Label myLabel = foundry.GetLabelById("2910");

// Update it's name
myLabel.Name = "C# SDK Updated My Name!";
myLabel = foundry.UpdateLabel(myLabel);

// Delete the label
foundry.DeleteLabel(myLabel);
```

### Adding labels to users
Once you have a particular label, you can add groups of users to that label. Start by creating your group of users. We'll do this by [searching](#Searching-for-users) for users.
```c#
Dictionary<SearchTerms, string> search = new Dictionary<SearchTerms, string>();
search.Add(SearchTerms.FirstName, "Aman");

List<User> users = foundry.GetUsersBySearch(search);
```
Next, we find the label we want our users to be associated with.
```c#
Label label = foundry.GetLabelById("2837");
```
And finally, we can use the bulk add feature to add the label to all users at once.
```c#
foundry.BulkAssignLabels(users, label); // You can assign this to List<string> responses, and read out the responses of each batch that was added.
```
