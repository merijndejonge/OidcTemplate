# Description
This project forms a template project for setting-up an application with Asp.Net Core, IdentityServer4, and Angular. The code is based on real production code as presented by Ben Cull on his [NDC presentation on July 16, 2017 in Oslo](https://youtu.be/3rtq8M1s95c).

During his presentation, Ben gave an excellent tour through his application, explaining how to set up Identity Server 4 in an ASP.NET Core app as a token server, Entity Framework and ASP.NET Identity for security, ASP.NET Core MVC for an API and an Angular SPA application.

The cool thing is that Ben is showing real production code. The obvious drawback is that he is unable to make the source code available to the public. That is where `OidcTemplate` comes in. 

With `OidcTemplate` the code related to authentication is moved 
from Ben's presentation into a set of three open source projects. These projects are created from default project templates with only authentication related stuff add. With this approach only minimal changes needed to be made to the default project templates (we'll discuss this in more detail below). With `OidcTemplate` we can now all benefit from the expertise of Ben by using these templates as a start for building products with authentication using IdentityServer4.

# The templates
`OidcTempate` consists of the following three AspNet Core applications:
* `OidcTemplate.Auth`. This is the token server application that is using IdentityServer4. It was created with the command `dotnet new razor --auth individual`. The project was then extended with IdentityServer4, IdentityServer4.EntityFramework, migrations, profile service, etc.
* `OidcTemplate.Client`. This is the angular web application. It was created with the command `dotnet new angular`. We added the authentication layer to the Mvc part of the project, as explained in Ben's presentation. We also moved the SampleDataController controller to the api project (see below). To the Angular web app, we added the portal service to get access to the authentication information, like the access token and username, and we added the authenticated-http service, which adds a bearer token to http requests and deals with refreshing the access tokens.
* `OidcTemplate.Api`. This is the api project. It was created with the command `dotnet new webapi`. We added the authentication layer from Ben's presentation. Furthermore, we replaced the default ValuesController controller with the SampleDataController from the client project. To this controller we added the `Authorize(Policy = DomainPolicies.NormalUser)` attribute to prevent access to unauthorized clients.

The other two projects of `OidcTemplate` are the data project with entity framework mirgrations and the `TemporaryDbContextFactory` class, and the domain project containing authentication-related data (such as claim types and domain policies), domain configuration settings, and the `ApplicationUser` class.

# Configuration
Configuration is done in the `appsettings.json` file. Currently, this file is duplicated in the three projects. This needs to be refactored probably. Each `appsettings.json` file contains a section with domain settings and (optionally) additional application settings. For instance, `appsettings.json` for `OidcTemplate.Auth` looks as follows:
```json
{
  "AppSettings": {

    "ConnectionStrings": {
      "AuthContext": "Server=(localdb)\\MSSQLLocalDB;Database=Auth;Trusted_Connection=True;MultipleActiveResultSets=true"
    }
  },
  "DomainSettings": {
    "Client": {
      "Id": "oidc_web",
      "Secret": "My secret key",
      "Url": "http://localhost:5002"
    },
    "Api": {
      "Id": "Api1",
      "Secret": "My secret Api1 secret",
      "Url": "http://localhost:5001"
    },
    "Auth": {
      "Url": "http://localhost:5000"
    }
  }
}
```
As you can see, it defines the connection string for the database (`AuthContext`) as an application setting. The three services are configured in the `DomainSettings` section of the configuration file.

# Prerequisites
* Visual Studio 2017
* Node.js (https://nodejs.org/en/)
# Build / run 
Once you obtained the source code of `OidcTemplate` from github, you need to perform a few steps from the command line to install required dependencies.
```bat
cd OidcTemplate
dotnet restore
cd src\OidcTemplate.Client
npm install -d
cd ..\..
```

Next you can open `OidcTermplate` in Visual Studio, or build/run the individual projects from the command line.

To open in Visual Studio double click on `OidcTemplate.sln`. To build/run the individual projects from the commandline execute the following commands:
```bat
cd src\OidcTemplate.Auth && dotnet run
cd src\OidcTemplate.Api && dotnet run
cd src\OidcTemplate.Client && dotnet run
```
You can now access the application by navigating to http://localhost:5002 in your web browser.

This will redirect you to the login page of your IdentityServer4 token server at http://localhost:5000. Here you first need to create an account. After crfeating an account you are redirected back to http://localhost:5002. If you then click on the `Fetch data` tab, a request is made to the SampleDataController WebApi that is running at http://localhost:5001. This service consumes the provided access token that you received when logging in. Only with this valid access token, the service will execute and provide you with weather forecast data.

# Used resources
* [IdentityServer4 documentation page](https://identityserver4.readthedocs.io/en/release/)
* [Identity Server: Introduction](https://elanderson.net/2017/05/identity-server-introduction/)
* [Identity Server 4 from Basics to Brain Melt | Ben Cull at NDC Oslo 2017](https://www.youtube.com/watch?v=3rtq8M1s95c)
* [Identity Server 4 Solution Architecture](http://benjii.me/2017/10/identity-server-4-solution-architecture/)
* [ASP.NET Core Authentication with IdentityServer4](https://blogs.msdn.microsoft.com/webdev/2017/01/23/asp-net-core-authentication-with-identityserver4/)
* [[dotnet new] Angular Single Page Application – Setup and How The Template Works](https://dotnetcore.gaprogman.com/2017/04/20/dotnet-new-angular-single-page-application-setup-and-how-the-template-works/)
* [Building web apps powered by Angular 2.x using Visual Studio 2017](https://channel9.msdn.com/Events/Visual-Studio/Visual-Studio-2017-Launch/WEB-103)
* [Creating a Self-Signed Certificate for Identity Server and Azure](http://benjii.me/2017/06/creating-self-signed-certificate-identity-server-azure/)
* [Enable Entity Framework Core Migrations in Visual Studio 2017](http://benjii.me/2017/05/enable-entity-framework-core-migrations-visual-studio-2017/)
* [An in-depth look at the various ways of specifying the IP or host ASP.NET Core listens on](http://josephwoodward.co.uk/2017/02/many-different-ways-specifying-host-port-asp-net-core)
* [.NET Core command-line interface (CLI) tools](https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-new?tabs=netcore2x)

# More info
* Source code of `OidcTemplate` is available at [GitHub](https://github.com/merijndejonge/OidcTemplate).
* `OidcTemplate` is distributed under the [Apache 2.0 License](https://github.com/merijndejongeOidcTemplate/blob/master/LICENSE).
# Acknoledgements
* [Ben Cull](https://bencull.com/) for giving an excellent presentation (with corresponding recording) at NDC Oslo 2017 and for supporting this project.