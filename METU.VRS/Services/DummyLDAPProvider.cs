using METU.VRS.Services.Abstract;
using System.Collections.Generic;

namespace METU.VRS.Services
{
    public class DummyLDAPProvider : ILoginProvider
    {
        private static Dictionary<string, string[]> testLogins = new Dictionary<string, string[]>() {
            { "e100", new string[] { "Test Student1", "student","II" } },
            { "e101", new string[] { "Test Student2", "student","MUH" } },
            { "e102", new string[] { "Test Student3", "student","FEAS" } },
            { "e103", new string[] { "Test Student4", "student","IAM" } },
            { "e200", new string[] { "Test Student11", "student","II" } },
            { "e201", new string[] { "Test Student12", "student","MUH" } },
            { "e202", new string[] { "Test Student13", "student","FEAS" } },
            { "e203", new string[] { "Test Student14", "student","IAM" } },
            { "a100", new string[] { "Test Academic1", "academic","II" } },
            { "a101", new string[] { "Test Academic2", "academic","MUH" } },
            { "a102", new string[] { "Test Academic3", "academic","FEAS" } },
            { "a103", new string[] { "Test Academic4", "academic","IAM" } },
            { "o100", new string[] { "Test Officer1", "administrative","OIDB" } },
            { "o101", new string[] { "Test Approval Officer", "administrative","II" } },
            { "o102", new string[] { "Test Delivery Officer", "administrative","IHM" } },
            { "o103", new string[] { "Test Security Officer", "administrative","IHM" } },
            { "isbank", new string[] { "Test Affiliate1", "affiliate","ISBANK" } },
            { "burgerking", new string[] { "Test Affiliate2", "affiliate","BURGERKING" } },
            { "aselsan", new string[] { "Test Affiliate3", "affiliate","ASELSAN" } },
            { "cati", new string[] { "Test Affiliate4", "affiliate","CATI" } },
        };

        public LDAPResult Login(string username, string password)
        {
            if (password != username || !testLogins.ContainsKey(username))
            {
                return new LDAPResult
                {
                    UID = username,
                    Result = false
                };
            }
            else
            {
                var testLogin = testLogins[username];
                return new LDAPResult()
                {
                    UID = username,
                    CN = testLogin[0],
                    DC = testLogin[1],
                    OU = testLogin[2],
                    Result = true
                };
            }
        }
    }
}