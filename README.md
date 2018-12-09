# METU VRS
Middle East Technical University Vehicle Registration System

This project is created for the SM504 Team Software Project course.

## Getting Started
Visual Studio 2017 v15.9 or higher is recommended to build the source code.

Some required 3rd party packages are downloaded and installed via Nuget during the first build.

To create a local test database, open `Package Manager Console` and run following commands:

```powershell
Enable-Migrations -Force -ConnectionStringName DbConnectionTest
Add-Migration Init -ConnectionStringName DbConnectionTest -ConfigurationTypeName ConfigurationDefault 
Update-Database -ConfigurationTypeName ConfigurationDefault -ConnectionStringName DbConnectionTest
```
## Test Site
The test web site is located on https://metuvrs.azurewebsites.net/

Test accounts that could be used to login the test site are shown below. 

Some of these accounts are created to realize future implementations. 

All test accounts’ passwords are equal to its login id.

| Login Id | Name | Division/Branch/Affiliate | User Category | Test |
|---|---|---|---|---|
| e100 | Test Student1 | Graduate School of Informatics | Student | Approval |
| e101 | Test Student2 | Faculty of Engineering | Student | Payment |
| e102 | Test Student3 | Faculty of Economic and Administrative Sciences | Student | Delivery |
| e103 | Test Student4 | Graduate School of Applied Mathematics | Student | Renew |
| e200 | Test Student11 | Graduate School of Informatics | Student | Approval |
| e201 | Test Student12 | Faculty of Engineering | Student | Payment |
| e202 | Test Student13 | Faculty of Economic and Administrative Sciences | Student | Delivery |
| e203 | Test Student14 | Graduate School of Applied Mathematics | Student | Renew |
| a100 | Test Academic1 | Graduate School of Informatics | Academic | Application |
| a101 | Test Academic2 | Faculty of Engineering | Academic | Application |
| a102 | Test Academic3 | Faculty of Economic and Administrative Sciences | Academic | Application |
| a103 | Test Academic4 | Graduate School of Applied Mathematics | Academic | Application |
| o100 | Test Officer1 | Directorate of Student Affairs | Administrative | Application |
| o101 | Test Approval Officer | Graduate School of Informatics | Administrative | Approval |
| o102 | Test Delivery Officer | Office of Domestic Services | Administrative | Delivery |
| o103 | Test Security Officer | Office of Domestic Services | Administrative | - |
| isbank | Test Affiliate1 | Türkiye İş Bankası | Affiliate | Affiliate Application |
| burgerking | Test Affiliate2 | BurgerKing | Affiliate | Affiliate Application |
| aselsan | Test Affiliate3 | ASELSAN | Affiliate | Affiliate Application | 
| cati | Test Affiliate4 | Çatı Cafe | Affiliate | Affiliate Application |

## Payment Specification
In the payment page, 3D Secure payment api provided by Asseco SEE is used. Example credit card numbers could be used for testing
