# Foundry C# SDK
C# SDK for the [EVERFI](https://www.everfi.com) Foundry API

## General Info
The Foundry API allows EVERFI partners to manage their organization by adding users, tracking progress, and other features. This C# SDK makes it easy for .NET developers to use the Foundry API.
### Contents
+ [Create a Client](#Creating-a-client)
+ [Organization Users](#Organization-Users)
    + [Create a new user](#Creating-a-new-user)
    + [Adding a user](#Adding-a-user-to-your-organization)
    + [Retrieving users](#Retrieving-users)
    + [Updating users](#Updating-an-existing-user)
+ [Organization Locations](#Organization-Locations)
    + [Getting your location](#Getting-your-organization's-location)
    + [Creating a new location](#Creating-a-new-location)
    + [Adding a location](#Adding-a-location-to-your-organization)
    + [Updating a location](#Updating-an-existing-location)
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
Once you have obtained your client id and secret, you can simply create a new instance of the API. The OAuth is taken care of in the backend!
```c#
string client_id = "ydWpq0jt4k8tsgAovEUQf8c4FOYHVyr1uMkkZHaMTp4";
string client_secret = "7RkwoL2IAMcOAgeMdItlBMg7dch7ZdQ7rzyaXRSAwm8";

API foundry = new API(client_id, client_secret);
```
You can now interact with the API and all of it's functionalities.
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
Additionally, you should add the user's [LocationId](#Organization-Locations) when adding or updating a user.
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
### Updating an existing user
```c#
// Make some change(s) to the user (ex. user.Email = lastfirst@everfi.com)
foundry.UpdateUser(user);
```
When updating a user it's important to note that you cannot remove any of the required fields (i.e. there must be a name, email, and at least one UserRole), but you can still update them.
## Organization Locations
An organization always has at least one location, and it can have multiple. If you are adding or updating users, you must have the user's **LocationId** defined for the request.
### Getting your organization's location
The locations for an organization rarely change, so rather than retrieving the locations multiple times, we recommend sending a request once and storing the values in a list or dictionary for later use.
#### Getting all locations
```c#
List<Location> locationsList = foundry.GetLocations();
// Saving locationIds for later use:
List<string> locationId = new List<string>();
foreach (Location loc in locationsList)
{
    locationId.Add(loc.Id);
}
```
#### Getting a location by id
If you already know the id of your location and want to retrieve more information about it, you can simply retrieve it by it's id.
```c#
Location loc = foundry.GetLocationById("15751");
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
### Updating an existing location
```c#
// Make some change(s) to the user (ex. location.Name = "Test Location (UPDATED)")
foundry.UpdateLocation(location)
```
## Categories
