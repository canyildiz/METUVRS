﻿using METU.VRS.Services.Abstract;
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

        public LDAPResult Login(string loginId, string password)
        {
            if (password != loginId || !testLogins.ContainsKey(loginId))
            {
                return new LDAPResult
                {
                    UID = loginId,
                    Result = false
                };
            }
            else
            {
                var testLogin = testLogins[loginId];
                return new LDAPResult()
                {
                    UID = loginId,
                    CN = testLogin[0],
                    DC = testLogin[1],
                    OU = testLogin[2],
                    Result = true
                };
            }
        }
    }
}