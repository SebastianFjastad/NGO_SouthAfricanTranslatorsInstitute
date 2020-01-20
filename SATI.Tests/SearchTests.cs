using Microsoft.VisualStudio.TestTools.UnitTesting;
using SATI.Entities;
using SATI.Models;
using System.Collections.Generic;
using System.Linq;

namespace SATI.Tests
{

    [TestClass]
    public class SearchTests
    {
        private List<User> _users = new List<User>
        {
            new User
            {
                FirstName = "Test First Name",
                LastName = "Test Last Name",
                AlternateEmail = "Test Email",
                Capabilities = new List<Capability>
                {
                    new Capability
                    {
                        From = new Language
                        {
                            LanguageId = 1,
                            Name = "German"
                        },
                        To = new Language
                        {
                            LanguageId = 2,
                            Name = "French"
                        },
                        Skill = new Skill
                        {
                            SkillId = 1,
                            Name = "Translating",
                            IsTransitive  = true,
                        },
                        Specialisations = new List<Specialisation> {
                            new Specialisation
                            {
                                SpecialisationId = 1,
                                Name = "Engineering",
                            }
                        },
                        Accreditations = new List<Accreditation>
                        {
                            new Accreditation
                            {
                                AccreditationId = 1,
                                Name = "Diploma",
                            }
                        }
                    }
                }
            }
        };


    public void TestSearchWithNoTerm()
    {
        //var model = new SearchParamsViewModel();
        //var filteredUsers = _repo.SearchUsers(model);
        //var allUsersCount = _repo.Where<User>(u => !u.IsDeleted).Count();
        //Assert.AreEqual(allUsersCount, filteredUsers.Count);
    }
    }
}
