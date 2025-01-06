Support for ASP.NET Core Identity was added to your project.

For setup and configuration information, see https://go.microsoft.com/fwlink/?linkid=2116645.
 Add-Migration -Context HotelIdentityDBContext -OutputDir "MigrationsIdentity" CreateIdentityShema
 Add-Migration -Context HotelBookingDbContext
 Update-Database -Context HotelIdentityDBContext 
 Update-Database -Context HotelBookingDbContext 
  DROP INDEX IX_Hotels_UserID ON dbo.Hotels;
  ALTER TABLE Bookings NOCHECK CONSTRAINT FK_Bookings_Hotels_HotelID;
ALTER TABLE Payments NOCHECK CONSTRAINT FK_Payments_Bookings_BookingID;