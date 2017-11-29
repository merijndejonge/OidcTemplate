
When model changed or for initial migration, run the following commands in the toplevel folder of the OidcTemplate.Data project:
 
 dotnet ef migrations add InitialMigration -c IdentityContext --startup-project ..\OidcTemplate.Auth\Oidctemplate.Auth.csproj
 dotnet ef migrations add InitialMigration -c PersistedGrantDbContext --startup-project ..\OidcTemplate.Auth\Oidctemplate.Auth.csproj

