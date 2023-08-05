using Microsoft.AspNetCore.Mvc;
using Inzynierka.DAL;
using Inzynierka.Models;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
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
                    return RedirectToAction("LoginTest", "Home");
                }
                   
            }

            User foundUser = _sqlCommandsManager.CheckForUserLogin(collection["username"], collection["Password"]);

            if (String.IsNullOrEmpty(foundUser.Username))
            {
                CreateErrorMessage("Wrong credencials entered", false);
                return RedirectToAction("LoginTest", "Home");
            }

            
            string token = TokenHelper.CreateToken();

            _sqlCommandsManager.CreateAuthToken(token, foundUser.ID.ToString(), false);
            TokenHelper.SendTokenViaMail("sztucznawiedza@gmail.com", token);
            SetSessionPrivilages(foundUser.Username, foundUser.Privilage.ToString(), foundUser.ID.ToString());

            return RedirectToAction("LoginAuth", "User");

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
                    return RedirectToAction("LoginTest", "Home");
                }
            }

            bool foundMatch;
            AuthToken auth = _sqlCommandsManager.FindLoginAuthToken(collection["Token"], out foundMatch);

            if(foundMatch && auth.UserID.ToString() == GetSessionUserID())
            {
                TempData["Success"] = "Loged in succesfully!";
                return RedirectToAction("Index", "Home");
            } 
            else
            {
                CreateErrorMessage("Either couldn't find user or the entered token was wrong!", true);
                return RedirectToAction("LoginTest", "Home");
            }
        }


        public IActionResult RegisterTest()
        {
            return View();
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
            return RedirectToAction("LoginTest", "Home");
        }

        public IActionResult RegisterWorker(IFormCollection collection)
        {
            User newUser = new User()
            {
                Username = collection["Username"],
                Email = collection["Email"]
            };

            if (!String.IsNullOrEmpty(collection["Phone"]))
                newUser.Phone = collection["Phone"];

            Password password = new Password(){ UserPassword = collection["Password"] };
            string refCode = collection["ReferalCode"];

            _sqlCommandsManager.CreateAccount(newUser, password.UserPassword, refCode);
            TempData["Message"] = "Account created succesfully";
            return RedirectToAction("LoginTest", "Home");
        }
    }
}
