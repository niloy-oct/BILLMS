using System;
using System.Collections.Generic;
using System.Configuration;

using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BILMS.Code;
using static BILMS.Controllers.AccountController;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Web.UI;
using Newtonsoft.Json;
using System.Web.Services.Description;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Web.Helpers;
using System.Linq.Dynamic;
using BILMS.Code;
using BILMS.ViewModels;

namespace BILMS.Controllers
{
    public class PendingController : Controller
    {
        public ActionResult Index()
        {
            TempData["AlertMessage"] = "Welcome to Bill Management System 👋";
            TempData["AlertType"] = "success";
            return View();
        }

    }
}