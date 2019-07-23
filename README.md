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
You can now interact with the API and all of it's functionalities. See the [Foundry Wiki]("../../wiki") for more information.
