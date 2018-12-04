# METU VRS
Middle East Technical University Vehicle Registration System

This project is created for the SM504 Team Software Project course.

## Getting Started
To create a local test database, open `Package Manager Console` and run following commands:

```powershell
Enable-Migrations -Force -ConnectionStringName DbConnectionTest
Add-Migration Init -ConnectionStringName DbConnectionTest -ConfigurationTypeName ConfigurationDefault 
Update-Database -ConfigurationTypeName ConfigurationDefault -ConnectionStringName DbConnectionTest
```
