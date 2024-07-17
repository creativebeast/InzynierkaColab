using Microsoft.AspNetCore.Mvc;
using Inzynierka.DAL;
using Inzynierka.Models;
using Inzynierka.Helpers;
using System.Collections.Generic;

namespace Inzynierka.Controllers
{
    public class UserController : BaseController
    {
        public UserController(ProjectContext context) : base(context)
        {
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login(IFormCollection collection)
        {
            foreach (var item in collection)
            {
                if (String.IsNullOrEmpty(item.Value.ToString()))
                {
                    CreateErrorMessage("Niektóre wymagane pola zostały puste", false);
                    return RedirectToAction("Login", "Home");
                }
            }

            string username = collection["username"];
            string password = collection["password"];

            //User foundUser = _sqlCommandsManager.CheckForUserLogin(collection["username"], collection["Password"]);
            User? foundUser = Inzynierka.Models.User.GetUserByUsernamePassword(_context, username, password);
            if (foundUser == null)
            {
                CreateErrorMessage("Wprowadzono nieprawidłowe dane logowania", false);
                return RedirectToAction("Login", "Home");
            }

            //string token = TokenHelper.CreateToken();

            //_sqlCommandsManager.CreateAuthToken(token, foundUser.ID.ToString(), false);

            //if(foundUser.Email != null)
            //    TokenHelper.SendTokenViaMail(foundUser.Email, token);
            //else
            //    TokenHelper.SendTokenViaMail("sztucznawiedza@gmail.com", token);
            SetSessionPrivilages(foundUser.Username, foundUser.Privilage.ToString(), foundUser.ID.ToString());

            return RedirectToAction("ChangeCompany", "Home");
            //return RedirectToAction("LoginAuth", "User");
        }

        public IActionResult LoginAuth()
        {
            return View();
        }

        public IActionResult AuthUser(IFormCollection collection)
        {
            foreach (var item in collection)
            {
                if (String.IsNullOrEmpty(item.ToString()))
                {
                    CreateErrorMessage("Coś poszło nie tak!", false);
                    return RedirectToAction("Login", "Home");
                }
            }

            bool foundMatch;
            AuthToken auth = _sqlCommandsManager.FindLoginAuthToken(collection["Token"], out foundMatch);

            if (foundMatch && auth.UserID == GetSessionUserID())
            {
                TempData["Success"] = "Zalogowano pomyślnie!";
                return RedirectToAction("Index", "Home");
            }
            else
            {
                CreateErrorMessage("Nie udało się znaleźć użytkownika lub wprowadzony token był nieprawidłowy!", false);
                return RedirectToAction("Login", "Home");
            }
        }


        public IActionResult Register()
        {
            return View("Register");
        }
        public IActionResult RegisterComp()
        {
            return View("RegisterCompany");
        }
        public IActionResult RegisterCompany(IFormCollection collection)
        {
            User testUser = new User()
            {
                Username = collection["Username"],
                Email = collection["Email"],
                Phone = null
            };

            Password password = new Password() { UserPassword = collection["Password"] };

            Company testCompany = new Company()
            {
                Name = collection["CompanyName"],
                PostalCode = collection["PostalCode"],
                City = collection["CompanyCity"],
                Province = collection["CompanyProvince"],
                Street = collection["CompanyStreet"],
                LocalNumber = collection["CompanyLocalNumber"],
                NIP = collection["CompanyNIP"]
            };

            _sqlCommandsManager.CreateAccount(testUser, password.UserPassword, testCompany);
            TempData["Success"] = "Konto utworzone pomyślnie";
            return RedirectToAction("Login", "Home");
        }

        public IActionResult RegisterWorker(IFormCollection collection)
        {
            User newUser = new User()
            {
                Username = collection["username"],
                Email = collection["Email"]
            };

            if (!String.IsNullOrEmpty(collection["Phone"]))
                newUser.Phone = String.Empty;

            if (String.IsNullOrWhiteSpace(collection["password"]))
            {
                TempData["error"] = "Puste hasło - pamiętaj, aby wypełnić pole hasła!";
                return RedirectToAction("Login", "Home");
            }

            Password password = new Password() { UserPassword = collection["Password"] };
            string refCode = collection["referalCode"];

            _sqlCommandsManager.CreateAccount(newUser, password.UserPassword, refCode);
            TempData["Success"] = "Konto utworzone pomyślnie";
            return RedirectToAction("Login", "Home");
        }

        public IActionResult Account()
        {
            User currentUser = Inzynierka.Models.User.GetUserById(_context, GetSessionUserID());
            if (currentUser == null)
            {
                TempData["error"] = "Niewystarczające uprawnienia...";
                return RedirectToAction("Login", "Home");
            }
            ViewData["User"] = currentUser;

            List<Company> companies;
            if (currentUser.Privilage == 0)
                companies = Company.getCompaniesRelatedToOwner(_context, GetSessionUserID());
            else
                companies = Company.getCompaniesRelatedToWorker(_context, GetSessionUserID());

            ViewData["Companies"] = companies;

            return View();
        }

        public IActionResult AccountSettings()
        {
            if (!CheckPrivilages(Privilages.Worker))
            {
                return RedirectToAction("Login", "Home");
            }

            List<Company> companies = Company.getCompaniesRelatedToWorker(_context, GetSessionUserID());
            ViewData["Companies"] = companies;

            return View();
        }

        public IActionResult ChangePassword(IFormCollection collection)
        {
            if (!CheckPrivilages(Privilages.Worker))
            {
                return RedirectToAction("Login", "Home");
            }

            string oldPassword = collection["oldPassword"];
            string newPassword = collection["newPassword"];
            string newPassword2 = collection["newPassword2"];
            int userId = GetSessionUserID();

            if (newPassword == newPassword2)
            {
                if (Inzynierka.Models.User.CheckIfPasswordMatch(_context, userId, oldPassword))
                {
                    int success = Inzynierka.Models.User.UpdateUserPassword(_context, userId, newPassword);

                    if (success == 1)
                        TempData["Success"] = "Hasło zmienione pomyślnie!";
                    else
                        TempData["Error"] = "Nieprawidłowe dane logowania...";

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    TempData["Error"] = "Nieprawidłowe dane logowania...";
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                TempData["Error"] = "Hasła nie pasują do siebie";
                return RedirectToAction("AccountSettings", "User");
            }

        }

        public IActionResult ChangePhoneNumber(IFormCollection collection)
        {
            if (!CheckPrivilages(Privilages.Worker))
            {
                return RedirectToAction("Login", "Home");
            }

            if (collection.Count < 3)
            {
                TempData["Error"] = "Niektóre pola zostały puste";
                return RedirectToAction("AccountSettings", "User");
            }

            foreach (var value in collection)
            {
                if (String.IsNullOrEmpty(value.Value))
                {
                    TempData["Error"] = "Niektóre pola zostały puste";
                    return RedirectToAction("AccountSettings", "User");
                }
            }

            string newPhoneNumber = collection["phoneNumber"];
            string userPassword = collection["password"];
            string userEmail = collection["mail"];

            if (!Inzynierka.Models.User.CheckIfPasswordMatch(_context, GetSessionUserID(), userPassword))
                return RedirectToAction("AccountSettings", "User");

            if (!Inzynierka.Models.User.UpdatePhoneNumber(_context, GetSessionUserID(), newPhoneNumber))
            {
                TempData["Error"] = "Nie udało się zaktualizować numeru telefonu";
                return RedirectToAction("AccountSettings", "User");
            }

            TempData["Success"] = "Numer telefonu został pomyślnie zmieniony~";
            return RedirectToAction("Index", "Home");
        }

        public IActionResult RemoveSelfFromCompany(IFormCollection collection)
        {
            if (!CheckPrivilages(Privilages.Worker))
            {
                return RedirectToAction("Login", "Home");
            }

            if (collection == null || collection.Count < 2)
            {
                TempData["Error"] = "Coś poszło nie tak...";
                return RedirectToAction("AccountSettings", "User");
            }

            foreach (var input in collection)
            {
                if (String.IsNullOrEmpty(input.Value))
                {
                    //TempData["Error"] = "Niektóre pola zostały puste";
                    //return RedirectToAction("AccountSettings", "User");
                }
            }

            int companyId = int.Parse(collection["company"].ToString());
            if (!Company.removeUserFromCompany(_context, GetSessionUserID(), companyId))
            {
                TempData["Error"] = "Nie udało się usunąć rekordu z bazy danych...";
                return RedirectToAction("AccountSettings", "User");
            }

            TempData["Success"] = "Usunięto siebie z firmy~";
            return RedirectToAction("Index", "Home");
        }

        public IActionResult StylingSettings()
        {
            if (!CheckPrivilages(Privilages.Worker))
            {
                return RedirectToAction("Login", "Home");
            }
            return View();
        }
    }
}
