using Microsoft.AspNetCore.Mvc;
using Inzynierka.DAL;
using Inzynierka.Models;
using Inzynierka.Helpers;

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
                if (String.IsNullOrEmpty(item.ToString()))
                {
                    CreateErrorMessage("Required fields left empty", false);
                    return RedirectToAction("Login", "Home");
                }
                   
            }
            string username = collection["username"];
            string password = collection["password"];
            //User foundUser = _sqlCommandsManager.CheckForUserLogin(collection["username"], collection["Password"]);
            User? foundUser = Inzynierka.Models.User.GetUserByUsernamePassword(_context, username, password);
            if (foundUser == null)
            {
                CreateErrorMessage("Wrong credencials entered", false);
                return RedirectToAction("Login", "Home");
            }

            //string token = TokenHelper.CreateToken();

            //_sqlCommandsManager.CreateAuthToken(token, foundUser.ID.ToString(), false);

            //if(foundUser.Email != null)
            //    TokenHelper.SendTokenViaMail(foundUser.Email, token);
            //else
            //    TokenHelper.SendTokenViaMail("sztucznawiedza@gmail.com", token);
            SetSessionPrivilages(foundUser.Username, foundUser.Privilage.ToString(), foundUser.ID.ToString());

            return RedirectToAction("Index", "Home");
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
                    CreateErrorMessage("Something went wrong!", true);
                    return RedirectToAction("Login", "Home");
                }
            }

            bool foundMatch;
            AuthToken auth = _sqlCommandsManager.FindLoginAuthToken(collection["Token"], out foundMatch);

            if(foundMatch && auth.UserID == GetSessionUserID())
            {
                TempData["Success"] = "Loged in succesfully!";
                return RedirectToAction("Index", "Home");
            } 
            else
            {
                CreateErrorMessage("Either couldn't find user or the entered token was wrong!", true);
                return RedirectToAction("Login", "Home");
            }
        }


        public IActionResult Register()
        {
            return View();
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
            TempData["Message"] = "Account created succesfully";
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
                TempData["Message"] = "Empty password - remember to fill password field!";
                return RedirectToAction("Login", "Home");
            }

            Password password = new Password(){ UserPassword = collection["Password"] };
            string refCode = collection["referalCode"];

            _sqlCommandsManager.CreateAccount(newUser, password.UserPassword, refCode);
            TempData["Message"] = "Account created succesfully";
            return RedirectToAction("Login", "Home");
        }

        public IActionResult Account()
        {
            User currentUser = Inzynierka.Models.User.GetUserById(_context, GetSessionUserID());
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
            CheckPrivilages(Privilages.Worker);

            List<Company> companies = Company.getCompaniesRelatedToWorker(_context, GetSessionUserID());
            ViewData["Companies"] = companies;

            return View();
        }

        public IActionResult ChangePassword(IFormCollection collection)
        {
            CheckPrivilages(Privilages.Worker);

            string oldPassword = collection["oldPassword"];
            string newPassword = collection["newPassword"];
            string newPassword2 = collection["newPassword2"];
            int userId = GetSessionUserID();

            if(newPassword == newPassword2)
            {
                if(Inzynierka.Models.User.CheckIfPasswordMatch(_context, userId, oldPassword))
                {
                    int success = Inzynierka.Models.User.UpdateUserPassword(_context, userId, newPassword);

                    if (success == 1)
                        TempData["Success"] = "Password changed successfully!";
                    else
                        TempData["Error"] = "Incorrect credentials...";

                    return RedirectToAction("Index", "Home");
                } else {
                    TempData["Error"] = "Incorrect credentials...";
                    return RedirectToAction("Index", "Home");
                }
            } else {
                TempData["Error"] = "Passwords don't match";
                return RedirectToAction("AccountSettings", "User");
            }
            
        }

        public IActionResult ChangePhoneNumber(IFormCollection collection)
        {
            CheckPrivilages(Privilages.Worker);

            if (collection.Count < 3)
            {
                TempData["Error"] = "Some fields were left empty";
                return RedirectToAction("AccountSettings", "User");
            }

            foreach (var value in collection)
            {
                if (String.IsNullOrEmpty(value.Value))
                {
                    TempData["Error"] = "Some fields were left empty";
                    return RedirectToAction("AccountSettings", "User");
                }
            }

            string newPhoneNumber = collection["phoneNumber"];
            string userPassword = collection["password"];
            string userEmail = collection["mail"];

            if (!Inzynierka.Models.User.CheckIfPasswordMatch(_context, GetSessionUserID(), userPassword))
                return RedirectToAction("AccountSettings", "User");

            if(!Inzynierka.Models.User.UpdatePhoneNumber(_context, GetSessionUserID(), newPhoneNumber))
            {
                TempData["Error"] = "Couldn't update phone number";
                return RedirectToAction("AccountSettings", "User");
            }

            TempData["Success"] = "Phone number changed successfully~";
            return RedirectToAction("Index", "Home");
        }

        public IActionResult RemoveSelfFromCompany(IFormCollection collection)
        {
            CheckPrivilages(Privilages.Worker);

            if (collection == null || collection.Count < 2)
            {
                TempData["Error"] = "Something went wrong...";
                return RedirectToAction("AccountSettings", "User");
            }

            foreach(var input in collection)
            {
                if (String.IsNullOrEmpty(input.Value))
                {
                    TempData["Error"] = "Some fields were left empty";
                    return RedirectToAction("AccountSettings", "User");
                }
            }

            int companyId = int.Parse(collection["company"].ToString());
            if (!Company.removeUserFromCompany(_context, GetSessionUserID(), companyId))
            {
                TempData["Error"] = "Couldn't remove record from database...";
                return RedirectToAction("AccountSettings", "User");
            }

            TempData["Success"] = "Removed self from company~";
            return RedirectToAction("Index", "Home");
        }

        public IActionResult StylingSettings()
        {
            
            return View();
        }
    }
}
