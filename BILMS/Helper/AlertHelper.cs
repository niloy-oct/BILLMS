using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BILMS.Helper
{
    public static class AlertHelper
    {
        public static string ShowAlert(string message, string alertType)
        {

            string alertClass = "";

            switch (alertType)
            {
                case "success":
                    alertClass = "alert-success";
                    break;
                case "info":
                    alertClass = "alert-info";
                    break;
                case "warning":
                    alertClass = "alert-warning";
                    break;
                case "danger":
                    alertClass = "alert-danger";
                    break;
            }

            return $"<div class=\"alert {alertClass} alert-dismissible fade show\" role=\"alert\">" +
                   $"<button type=\"button\" class=\"close\" data-dismiss=\"alert\" aria-label=\"Close\">" +
                   $"<span aria-hidden=\"true\">&times;</span></button>" +
                   $"{message}</div>";
        }
    }
}