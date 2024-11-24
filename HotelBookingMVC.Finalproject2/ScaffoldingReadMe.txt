Support for ASP.NET Core Identity was added to your project.

For setup and configuration information, see https://go.microsoft.com/fwlink/?linkid=2116645.
 Add-Migration -Context HotelIdentityDBContext -OutputDir "MigrationsIdentity" CreateIdentityShema
 Add-Migration -Context HotelBookingDbContext
 Update-Database -Context HotelIdentityDBContext 
 Update-Database -Context HotelBookingDbContext 