# Foundry C# SDK
C# SDK for the [EVERFI](https://www.everfi.com) Foundry API

## General Info
The Foundry API allows EVERFI partners to manage their organization by adding users, tracking progress, and other features. This C# SDK makes it easy for .NET developers to use the Foundry API.

## Setup
You can download v1.0.0 of the EVERFI Foundry SDK from [NuGet](https://www.nuget.org/packages/EVERFI/). Currently the SDK works for .NET 4.7.2.

## Usage
You should get your client id, secret, and endpoint from the Admin Panel. More information can be found in the [public API documentation](https://api.fifoundry.net/v1).
### Creating a client
Once you have obtained your client id and secret, you can simply create a new instance of the API. The OAuth is taken care of in the backend.
```c#
// Your client id and secret below
// Rather than storing them in plaintext, you can store them with .NET's Secret Manager
string client_id = "";
string client_secret = "";
string endpoint = "";

API foundry = new API(accountSid: client_id, secretKey: client_secret, BaseUrl: endpoint);
```
You can now interact with the API and all of its functionalities. See the [Foundry Wiki](../../wiki) for more information.
