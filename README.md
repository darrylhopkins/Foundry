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
List<Locations> validLocations = foundry.GetLocations(); // To update valid locations
```
We also retrieve the list of valid locations already in Foundry, which in turn updates the locations in the local API. These locations will be used later.

You can now interact with the API and all of it's functionalities.

## Organization Locations
An organization always has at least one location, and it can have multiple. If you are adding or updating users, you should provide the user's **Location** defined for the request.
### Getting your organization's location
The first thing you should do after creating your Foundry Client is save the Organization's Locations to a list or dictionary for later use.
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

## Categories
