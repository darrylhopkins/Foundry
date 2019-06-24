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
### Adding a user
```c#
user = foundry.AddUser(user);
```
You need to assign your original user to the AddUser function so that the user is updated with the new UserId, which won't exist until you add it to Foundry. You cannot add a UserId to a user yourself.
### Getting a user by id
```c#
string UserId = "58c836fd-ebe3-4533-aee4-7a6f1a064de9";
User retrievedUser = foundry.GetUserById(UserId);
```
### Getting all users
```c#
List<User> allUsers = foundry.GetUsers();
```
