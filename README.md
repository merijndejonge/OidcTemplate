https://dotnetcore.gaprogman.com/2017/04/20/dotnet-new-angular-single-page-application-setup-and-how-the-template-works/
http://josephwoodward.co.uk/2017/02/many-different-ways-specifying-host-port-asp-net-core
https://www.youtube.com/watch?v=3rtq8M1s95c
http://docs.identityserver.io/en/release/
https://blogs.msdn.microsoft.com/webdev/2017/01/23/asp-net-core-authentication-with-identityserver4/
http://benjii.me/2017/10/identity-server-4-solution-architecture/
http://benjii.me/2017/06/creating-self-signed-certificate-identity-server-azure/
http://benjii.me/2017/05/enable-entity-framework-core-migrations-visual-studio-2017/



dotnet restore
cd src\OidcTemplate.Client
npm install -d
cd ..\..
dotnet build
cd src\OidcTemplate.Auth && dotnet run
cd src\OidcTemplate.Api && dotnet run
cd src\OidcTemplate.Client && dotnet run

