namespace METU.VRS.Migrations
{
    using METU.VRS.Models;
    using System;
    using System.Data.Entity.Migrations;

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public sealed class ConfigurationDefault : DbMigrationsConfiguration<METU.VRS.Services.DatabaseContext>
    {
        public ConfigurationDefault()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(METU.VRS.Services.DatabaseContext context)
        {
            //UserRole
            UserRole authenticated_user = new UserRole() { UID = "authenticated_user", Description = "Authenticated User" };
            UserRole approval_user = new UserRole() { UID = "approval_user", Description = "Approval User" };
            UserRole delivery_user = new UserRole() { UID = "delivery_user", Description = "Delivery User" };
            UserRole security_officer = new UserRole() { UID = "security_officer", Description = "Security Officer" };
            UserRole admin = new UserRole() { UID = "admin", Description = "System Admin" };

            context.UserRoles.AddOrUpdate(a => a.UID, new UserRole[] { authenticated_user, approval_user, delivery_user, security_officer, admin });


            //UserCategory
            UserCategory student = new UserCategory() { UID = "student", Description = "Student" };
            UserCategory academic = new UserCategory() { UID = "academic", Description = "Academic" };
            UserCategory administrative = new UserCategory() { UID = "administrative", Description = "Administrative" };
            UserCategory affiliate = new UserCategory() { UID = "affiliate", Description = "Affiliate" };
            UserCategory alumni = new UserCategory() { UID = "alumni", Description = "Alumni" };

            context.UserCategories.AddOrUpdate(a => a.UID, new UserCategory[] { student, academic, administrative, affiliate, alumni });


            //BranchAffiliate
            BranchAffiliate II = new BranchAffiliate() { UID = "II", Name = "Graduate School of Informatics", Type = BranchAffiliateTypes.GraduateSchool };
            BranchAffiliate MUH = new BranchAffiliate() { UID = "MUH", Name = "Faculty of Engineering", Type = BranchAffiliateTypes.Faculty };
            BranchAffiliate FEAS = new BranchAffiliate() { UID = "FEAS", Name = "Faculty of Economic and Administrative Sciences", Type = BranchAffiliateTypes.Faculty };
            BranchAffiliate IAM = new BranchAffiliate() { UID = "IAM", Name = "Graduate School of Applied Mathematics", Type = BranchAffiliateTypes.GraduateSchool };
            BranchAffiliate IHM = new BranchAffiliate() { UID = "IHM", Name = "Office of Domestic Services", Type = BranchAffiliateTypes.AdministrativeUnit };
            BranchAffiliate OIDB = new BranchAffiliate() { UID = "OIDB", Name = "Directorate of Student Affairs", Type = BranchAffiliateTypes.AdministrativeUnit };
            BranchAffiliate ISBANK = new BranchAffiliate() { UID = "ISBANK", Name = "Türkiye İş Bankası", Type = BranchAffiliateTypes.Affiliate };
            BranchAffiliate BURGERKING = new BranchAffiliate() { UID = "BURGERKING", Name = "BurgerKing", Type = BranchAffiliateTypes.Affiliate };
            BranchAffiliate ASELSAN = new BranchAffiliate() { UID = "ASELSAN", Name = "ASELSAN", Type = BranchAffiliateTypes.Affiliate };
            BranchAffiliate CATI = new BranchAffiliate() { UID = "CATI", Name = "Çatı Cafe", Type = BranchAffiliateTypes.Affiliate };


            context.BranchsAffiliates.AddOrUpdate(a => a.UID, new BranchAffiliate[] { II, MUH, FEAS, IAM, IHM, OIDB, ISBANK, BURGERKING, ASELSAN, CATI });

            //User
            User e100 = new User() { UID = "e100", Name = "Test Student1", Category = student, Division = II, Roles = new UserRole[] { authenticated_user } };
            User e101 = new User() { UID = "e101", Name = "Test Student2", Category = student, Division = MUH, Roles = new UserRole[] { authenticated_user } };
            User e102 = new User() { UID = "e102", Name = "Test Student3", Category = student, Division = FEAS, Roles = new UserRole[] { authenticated_user } };
            User e103 = new User() { UID = "e103", Name = "Test Student4", Category = student, Division = IAM, Roles = new UserRole[] { authenticated_user } };
            User e200 = new User() { UID = "e200", Name = "Test Student11", Category = student, Division = II, Roles = new UserRole[] { authenticated_user } };
            User e201 = new User() { UID = "e201", Name = "Test Student12", Category = student, Division = MUH, Roles = new UserRole[] { authenticated_user } };
            User e202 = new User() { UID = "e202", Name = "Test Student13", Category = student, Division = FEAS, Roles = new UserRole[] { authenticated_user } };
            User e203 = new User() { UID = "e203", Name = "Test Student14", Category = student, Division = IAM, Roles = new UserRole[] { authenticated_user } };
            User a100 = new User() { UID = "a100", Name = "Test Academic1", Category = academic, Division = II, Roles = new UserRole[] { authenticated_user } };
            User a101 = new User() { UID = "a101", Name = "Test Academic2", Category = academic, Division = MUH, Roles = new UserRole[] { authenticated_user } };
            User a102 = new User() { UID = "a102", Name = "Test Academic3", Category = academic, Division = FEAS, Roles = new UserRole[] { authenticated_user } };
            User a103 = new User() { UID = "a103", Name = "Test Academic4", Category = academic, Division = IAM, Roles = new UserRole[] { authenticated_user } };
            User o100 = new User() { UID = "o100", Name = "Test Officer1", Category = administrative, Division = OIDB, Roles = new UserRole[] { authenticated_user } };
            User o101 = new User() { UID = "o101", Name = "Test Approval Officer", Category = administrative, Division = II, Roles = new UserRole[] { authenticated_user, approval_user } };
            User o102 = new User() { UID = "o102", Name = "Test Delivery Officer", Category = administrative, Division = IHM, Roles = new UserRole[] { authenticated_user, delivery_user } };
            User o103 = new User() { UID = "o103", Name = "Test Security Officer", Category = administrative, Division = IHM, Roles = new UserRole[] { authenticated_user, security_officer } };
            User isbank = new User() { UID = "isbank", Name = "Test Affiliate1", Category = affiliate, Division = ISBANK, Roles = new UserRole[] { authenticated_user } };
            User burgerking = new User() { UID = "burgerking", Name = "Test Affiliate2", Category = affiliate, Division = BURGERKING, Roles = new UserRole[] { authenticated_user } };
            User aselsan = new User() { UID = "aselsan", Name = "Test Affiliate3", Category = affiliate, Division = ASELSAN, Roles = new UserRole[] { authenticated_user } };
            User cati = new User() { UID = "cati", Name = "Test Affiliate4", Category = affiliate, Division = CATI, Roles = new UserRole[] { authenticated_user } };


            context.Users.AddOrUpdate(a => a.UID, new User[] {
                e100,e101,e102,e103,e200,e201,e202,e203,a100,a101,a102,a103,o100,o101,o102,o103,isbank,burgerking,aselsan,cati
            });

            //StickerTerm
            StickerTerm longterm = new StickerTerm() { Type = TermTypes.LongTerm, StartDate = new DateTime(2018, 1, 1), EndDate = new DateTime(2099, 1, 1) };
            StickerTerm calenderyear2018 = new StickerTerm() { Type = TermTypes.CalendarYear, StartDate = new DateTime(2018, 1, 1), EndDate = new DateTime(2019, 1, 1) };
            StickerTerm academicyear2017 = new StickerTerm() { Type = TermTypes.AcademicYear, StartDate = new DateTime(2017, 10, 1), EndDate = new DateTime(2018, 10, 1) };
            StickerTerm academicyear2018 = new StickerTerm() { Type = TermTypes.AcademicYear, StartDate = new DateTime(2018, 10, 1), EndDate = new DateTime(2019, 10, 1) };

            context.StickerTerms.AddOrUpdate(a => new { a.Type, a.StartDate, a.EndDate }, new StickerTerm[] { longterm, calenderyear2018, academicyear2018 });


            //StickerType
            StickerType st1 = new StickerType() { Class = StickerClasses.Staff, Color = "Red", Description = "Academican Sticker", TermType = TermTypes.LongTerm, UserCategory = academic };
            StickerType st2 = new StickerType() { Class = StickerClasses.Staff, Color = "Green", Description = "Administrative Attendant Sticker", TermType = TermTypes.LongTerm, UserCategory = administrative };
            StickerType st3 = new StickerType() { Class = StickerClasses.Foundation, Color = "Black", Description = "METU Foundation Member Sticker", TermType = TermTypes.LongTerm, UserCategory = administrative };
            StickerType st4 = new StickerType() { Class = StickerClasses.Visitor, Color = "Blue", Description = "Visitor Sticker", TermType = TermTypes.CalendarYear, UserCategory = administrative };
            StickerType st5 = new StickerType() { Class = StickerClasses.Visitor, Color = "Blue", Description = "Campus Services Sticker", TermType = TermTypes.CalendarYear, UserCategory = affiliate };
            StickerType st6 = new StickerType() { Class = StickerClasses.Visitor, Color = "Blue", Description = "Visiting Researcher Sticker", TermType = TermTypes.AcademicYear, UserCategory = academic };
            StickerType st7 = new StickerType() { Class = StickerClasses.Visitor, Color = "Blue", Description = "Sport Club Trainer", TermType = TermTypes.CalendarYear, UserCategory = administrative };
            StickerType st8 = new StickerType() { Class = StickerClasses.Visitor, Color = "Blue", Description = "Sport Club Member", TermType = TermTypes.CalendarYear, UserCategory = administrative };
            StickerType st9 = new StickerType() { Class = StickerClasses.Visitor, Color = "Blue", Description = "Lodgement Sticker", TermType = TermTypes.CalendarYear, UserCategory = administrative };
            StickerType st10 = new StickerType() { Class = StickerClasses.Alumni, Color = "Gray", Description = "Alumni Sticker", TermType = TermTypes.CalendarYear, UserCategory = alumni };
            StickerType st11 = new StickerType() { Class = StickerClasses.Technopolis, Color = "Orange", Description = "Technopolis Sticker", TermType = TermTypes.CalendarYear, UserCategory = affiliate };
            StickerType st12 = new StickerType() { Class = StickerClasses.Student, Color = "Yellow", Description = "Unlimited Student Sticker", TermType = TermTypes.AcademicYear, UserCategory = student };
            StickerType st13 = new StickerType() { Class = StickerClasses.Student, Color = "Brown", Description = "Limited Student Sticker", TermType = TermTypes.AcademicYear, UserCategory = student };
            StickerType st14 = new StickerType() { Class = StickerClasses.StudentParent, Color = "Purple", Description = "College Student Parent Sticker", TermType = TermTypes.AcademicYear, UserCategory = affiliate };
            StickerType st15 = new StickerType() { Class = StickerClasses.StudentParent, Color = "Purple", Description = "Kindergarten Student Parent Sticker", TermType = TermTypes.AcademicYear, UserCategory = affiliate };

            context.StickerTypes.AddOrUpdate(a => a.Description, new StickerType[] { st1, st2, st3, st4, st5, st6, st7, st8, st9, st10, st11, st12, st13, st14, st15 });

            //Quota
            Quota q1 = new Quota() { UID = "q1", Term = longterm, Division = null, StickerFee = 40, TotalQuota = -1, Type = st1 };
            Quota q2 = new Quota() { UID = "q2", Term = longterm, Division = null, StickerFee = 40, TotalQuota = -1, Type = st2 };
            Quota q3 = new Quota() { UID = "q3", Term = longterm, Division = null, StickerFee = 60, TotalQuota = -1, Type = st3 };
            Quota q4 = new Quota() { UID = "q4", Term = calenderyear2018, Division = null, StickerFee = 700, TotalQuota = 100, Type = st4 };
            Quota q5 = new Quota() { UID = "q5", Term = calenderyear2018, Division = null, StickerFee = 60, TotalQuota = 100, Type = st5 };
            Quota q6 = new Quota() { UID = "q6", Term = academicyear2018, Division = null, StickerFee = 60, TotalQuota = 100, Type = st6 };
            Quota q7 = new Quota() { UID = "q7", Term = calenderyear2018, Division = null, StickerFee = 700, TotalQuota = 100, Type = st7 };
            Quota q8 = new Quota() { UID = "q8", Term = calenderyear2018, Division = null, StickerFee = 60, TotalQuota = 100, Type = st8 };
            Quota q9 = new Quota() { UID = "q9", Term = calenderyear2018, Division = null, StickerFee = 60, TotalQuota = 1000, Type = st9 };
            Quota q10 = new Quota() { UID = "q10", Term = calenderyear2018, Division = null, StickerFee = 700, TotalQuota = -1, Type = st10 };
            Quota q11 = new Quota() { UID = "q11", Term = calenderyear2018, Division = null, StickerFee = 600, TotalQuota = 5000, Type = st11 };
            Quota q12 = new Quota() { UID = "q12", Term = academicyear2018, Division = II, StickerFee = 500, TotalQuota = 1, Type = st12 };
            Quota q13 = new Quota() { UID = "q13", Term = academicyear2018, Division = II, StickerFee = 100, TotalQuota = 100, Type = st13 };
            Quota q14 = new Quota() { UID = "q14", Term = academicyear2018, Division = null, StickerFee = 300, TotalQuota = 500, Type = st14 };
            Quota q15 = new Quota() { UID = "q15", Term = academicyear2018, Division = null, StickerFee = 60, TotalQuota = 500, Type = st15 };
            Quota q16 = new Quota() { UID = "q16", Term = academicyear2018, Division = FEAS, StickerFee = 500, TotalQuota = 1, Type = st12 };
            Quota q17 = new Quota() { UID = "q17", Term = academicyear2018, Division = FEAS, StickerFee = 100, TotalQuota = 100, Type = st13 };
            Quota q18 = new Quota() { UID = "q18", Term = academicyear2018, Division = IAM, StickerFee = 500, TotalQuota = 1, Type = st12 };
            Quota q19 = new Quota() { UID = "q19", Term = academicyear2018, Division = IAM, StickerFee = 100, TotalQuota = 100, Type = st13 };
            Quota q20 = new Quota() { UID = "q20", Term = academicyear2018, Division = MUH, StickerFee = 500, TotalQuota = 2, Type = st12 };
            Quota q21 = new Quota() { UID = "q21", Term = academicyear2018, Division = MUH, StickerFee = 100, TotalQuota = 100, Type = st13 };
            Quota q22 = new Quota() { UID = "q22", Term = academicyear2017, Division = IAM, StickerFee = 300, TotalQuota = 2, Type = st12 };

            context.Quotas.AddOrUpdate(a => a.UID, new Quota[] { q1, q2, q3, q4, q5, q6, q7, q8, q9, q10, q11, q12, q13, q14, q15, q16, q17, q18, q19, q20, q21, q22 });

            Vehicle ve100 = new Vehicle { OwnerName = e100.Name, PlateNumber = "06AA100", RegistrationNumber = "AA100000", Type = VehicleType.Car };
            Vehicle ve200 = new Vehicle { OwnerName = e200.Name, PlateNumber = "06AA200", RegistrationNumber = "AA200000", Type = VehicleType.Car };
            Vehicle ve101 = new Vehicle { OwnerName = e101.Name, PlateNumber = "06AA101", RegistrationNumber = "AA101000", Type = VehicleType.Car };
            Vehicle ve201 = new Vehicle { OwnerName = e201.Name, PlateNumber = "06AA201", RegistrationNumber = "AA201000", Type = VehicleType.Car };
            Vehicle ve102 = new Vehicle { OwnerName = e102.Name, PlateNumber = "06AA102", RegistrationNumber = "AA102000", Type = VehicleType.Car };
            Vehicle ve202 = new Vehicle { OwnerName = e202.Name, PlateNumber = "06AA202", RegistrationNumber = "AA202000", Type = VehicleType.Car };
            Vehicle ve103 = new Vehicle { OwnerName = e102.Name, PlateNumber = "06AA103", RegistrationNumber = "AA103000", Type = VehicleType.Car };
            Vehicle ve203 = new Vehicle { OwnerName = e202.Name, PlateNumber = "06AA203", RegistrationNumber = "AA203000", Type = VehicleType.Car };

            context.Vehicles.AddOrUpdate(v => v.ID, new Vehicle[] { ve100, ve200, ve101, ve201, ve102, ve202, ve103, ve203 });


            context.StickerApplications.AddOrUpdate(a => a.ID, new StickerApplication[]
            {
                new StickerApplication(){
                    User =e100,
                    CreateDate = DateTime.Now,LastModified=DateTime.Now,
                    Owner = new ApplicationOwner(){Name =e100.Name},
                    Quota =q12,
                    Status =StickerApplicationStatus.WaitingForApproval,
                    Vehicle = ve100
                },
                 new StickerApplication(){
                    User =e200,
                    CreateDate = DateTime.Now,LastModified=DateTime.Now,
                    Owner = new ApplicationOwner(){Name =e200.Name},
                    Quota =q12,
                    Status =StickerApplicationStatus.WaitingForApproval,
                    Vehicle = ve200
                },
                 new StickerApplication(){
                    User =e101,
                    CreateDate = DateTime.Now,
                    LastModified =DateTime.Now,
                    ApproveDate=DateTime.Now,
                    Owner = new ApplicationOwner(){Name =e101.Name},
                    Quota =q20,
                    Status =StickerApplicationStatus.WaitingForPayment,
                    Vehicle = ve101
                },
                 new StickerApplication(){
                    User =e201,
                    CreateDate = DateTime.Now,
                    LastModified =DateTime.Now,
                    ApproveDate=DateTime.Now,
                    Owner = new ApplicationOwner(){Name =e201.Name},
                    Quota =q20,
                    Status =StickerApplicationStatus.WaitingForPayment,
                    Vehicle = ve201
                },
                 new StickerApplication(){
                    User =e102,
                    Payment=new Payment{Amount=q16.StickerFee,TransactionDate = DateTime.Now,TransactionNumber="TEST1234"},
                    CreateDate = DateTime.Now,
                    LastModified =DateTime.Now,
                    ApproveDate=DateTime.Now,
                    Owner = new ApplicationOwner(){Name =e102.Name},
                    Quota =q16,
                    Status =StickerApplicationStatus.WaitingForDelivery,
                    Vehicle = ve102
                },
                 new StickerApplication(){
                    User =e202,
                    Payment=new Payment{Amount=q17.StickerFee,TransactionDate = DateTime.Now,TransactionNumber="TEST5678"},
                    CreateDate = DateTime.Now,
                    LastModified =DateTime.Now,
                    ApproveDate=DateTime.Now,
                    Owner = new ApplicationOwner(){Name =e202.Name},
                    Quota =q17,
                    Status =StickerApplicationStatus.WaitingForDelivery,
                    Vehicle = ve202
                },
                 new StickerApplication(){
                    User =e103,
                    Payment=new Payment{Amount=q22.StickerFee,TransactionDate = DateTime.Now,TransactionNumber="TEST1234"},
                    Sticker =new Sticker{SerialNumber=12345},
                    CreateDate = DateTime.Now,
                    LastModified =DateTime.Now,
                    ApproveDate=q22.Term.StartDate,
                    DeliveryDate=q22.Term.StartDate,
                    Owner = new ApplicationOwner(){Name =e103.Name},
                    Quota =q22,
                    Status =StickerApplicationStatus.Active,
                    Vehicle = ve103
                },
                 new StickerApplication(){
                    User =e203,
                    Payment=new Payment{Amount=q22.StickerFee,TransactionDate = DateTime.Now,TransactionNumber="TEST5678"},
                    Sticker =new Sticker{SerialNumber=23456},
                    CreateDate = DateTime.Now,
                    LastModified =DateTime.Now,
                    ApproveDate=q22.Term.StartDate,
                    DeliveryDate=q22.Term.StartDate,
                    Owner = new ApplicationOwner(){Name =e203.Name},
                    Quota =q22,
                    Status =StickerApplicationStatus.Active,
                    Vehicle = ve203
                }
            });
        }
    }
}
