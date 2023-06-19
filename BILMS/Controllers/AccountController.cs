using BILMS.Code;

using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.ServiceModel.Channels;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using BILMS.Helper;
using BILMS;
using System.Configuration;

namespace BILMS.Controllers
{

    public class AccountController : Controller
    {

        private DatabaseContext context;

        public AccountController(DatabaseContext databaseContext)
        {
            this.context = databaseContext;
        }


        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }



        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> Login(FormCollection formCollection)
        {

            var UserName = formCollection["UserName"];
            var Password = formCollection["Password"];

            LoginModel login = new LoginModel();
            login.UserName = UserName;
            login.Password = Password;
            if (login != null && !string.IsNullOrEmpty(login.UserName) && !string.IsNullOrEmpty(login.Password))
            {


                // Code here

                return RedirectToAction("Index", "Pending");
            }

            else
            {
                TempData["AlertMessage"] = "The username or password you entered is incorrect. Please try again.";
                TempData["AlertType"] = "danger";
            }


            return View();

        }

        public ActionResult Logout()
        {
            return RedirectToAction("Login");

        }

    }
}
