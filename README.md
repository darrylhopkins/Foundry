# Foundry C# SDK
C# SDK for the [EVERFI](https://www.everfi.com) Foundry API

## General Info
The Foundry API allows EVERFI partners to manage their organization by adding users, tracking progress, and other features. This C# SDK makes it easy for .NET developers to use the Foundry API. 

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
TODO: Obtaining id and secret
### Creating a client
Once you have obtained your client id and secret, you can simply create a new instance of the API. The OAuth is taken care of in the backend!
```c#
string client_id = "ydWpq0jt4k8tsgAovEUQf8c4FOYHVyr1uMkkZHaMTp4";
string client_secret = "7RkwoL2IAMcOAgeMdItlBMg7dch7ZdQ7rzyaXRSAwm8";

API foundry = new API(client_id, client_secret);
```
You can now interact with API with the all of it's functionalities.
## Organization Users
User description goes here
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
There are other attributes you can add to a user, which are not required: SSO Id, Student Id, Employee Id, Position, First Day, Last Day.
### Adding a user to your organization
```c#
user = foundry.AddUser(user);
```
You need to assign your original user to the AddUser function so that the user is updated with the new UserId, which won't exist until you add it to Foundry. You cannot add a UserId to a user yourself.
### Getting a user by id
```c#
string UserId = "58c836fd-ebe3-4533-aee4-7a6f1a064de9";
User retrievedUser = foundry.GetUserById(UserId);
```
### Getting users
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

while (keepGoing) {
    List<User> currentUsers = new List<User>();
    (currentUsers, keepGoing) = foundry.GetUsers(); // Returns the next 100 users and whether or not there are more.
    myUsers = myUsers.Concat(currentUsers).ToList();
}
```
### Updating an existing user
```c#
// Make some change to the user (ex. user.Email = lastfirst@everfi.com)
foundry.UpdateUser(user);
```
When updating a user it's important to note that you cannot remove any of the required fields (i.e. there must be a name, email, and at least one UserRole), but you can still update them.
## Organization Locations
Location description goes here
### Getting your organization's location(s)
```c#
List<Location> Locations = foundry.GetLocation();
```
### Creating a new location
```c#
Location newLocation = new Location();
// TODO: Add necessary fields
```
### Adding another location to your organization 
Like adding a user to your organization, you need to assign the new location to return value of the AddLocation function in order to update the location with it's location id.
```c#
newLocation = foundry.AddLocation(newLocation);

```
## Categories
