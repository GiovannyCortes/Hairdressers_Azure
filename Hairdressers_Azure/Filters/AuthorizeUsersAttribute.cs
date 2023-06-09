﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Hairdressers_Azure.Filters {
    public class AuthorizeUsersAttribute : AuthorizeAttribute, IAuthorizationFilter {

        public void OnAuthorization(AuthorizationFilterContext context) {
            var user = context.HttpContext.User;

            string controller = context.RouteData.Values["controller"].ToString();
            string action = context.RouteData.Values["action"].ToString();

            ITempDataProvider provider = context.HttpContext.RequestServices.GetService<ITempDataProvider>();

            var TempData = provider.LoadTempData(context.HttpContext);
            TempData["controller"] = controller;
            TempData["action"] = action;

            // Obtención de parámetros para la aprobación de una cita
            if (context.HttpContext.Request.Query.ContainsKey("token")) {
                var token = context.HttpContext.Request.Query["token"].FirstOrDefault();
                TempData["token"] = token;
            }

            if (context.HttpContext.Request.Query.ContainsKey("hid")) {
                var hid = context.HttpContext.Request.Query["hid"].FirstOrDefault();
                TempData["hid"] = hid;
            }

            if (context.HttpContext.Request.Query.ContainsKey("apid")) {
                var apid = context.HttpContext.Request.Query["apid"].FirstOrDefault();
                TempData["apid"] = apid;
            }

            provider.SaveTempData(context.HttpContext, TempData);

            if (user.Identity.IsAuthenticated == false) { // Usuario no identificado
                context.Result = GetRoute("Managed", "LogIn");
            }
        }

        private RedirectToRouteResult GetRoute(string controller, string action) {
            RouteValueDictionary ruta =
                new RouteValueDictionary(new {
                    controller,
                    action
                }
            );
            RedirectToRouteResult result = new RedirectToRouteResult(ruta);
            return result;
        }

    }
}
