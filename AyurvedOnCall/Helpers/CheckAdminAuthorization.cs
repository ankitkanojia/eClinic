using System;
using System.Web;
using System.Web.Mvc;

namespace AyurvedOnCall.Helpers
{
    public class CheckAdminAuthorization : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (string.IsNullOrWhiteSpace(CookieHelper.Get(StaticValues.SessionUserId)) || (string.IsNullOrWhiteSpace(CookieHelper.Get(StaticValues.SessionRoleId)) || !HttpContext.Current.Request.IsAuthenticated))
            {
                if (filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    filterContext.HttpContext.Response.StatusCode = 302; //Found Redirection to another page. Here- login page. Check Layout ajaxError() script.
                    filterContext.HttpContext.Response.End();
                }
                else
                {
                    filterContext.Result = new RedirectResult(System.Web.Security.FormsAuthentication.LoginUrl + "?ReturnUrl=" + filterContext.HttpContext.Server.UrlEncode(filterContext.HttpContext.Request.RawUrl));
                }
            }
            else
            {
                var roleId = Convert.ToInt32(CookieHelper.Get(StaticValues.SessionRoleId));

                if (roleId != (int)EnumList.Roles.Admin)
                {
                    if (filterContext.HttpContext.Request.IsAjaxRequest())
                    {
                        filterContext.HttpContext.Response.StatusCode = 302; //Found Redirection to another page. Here- login page. Check Layout ajaxError() script.
                        filterContext.HttpContext.Response.End();
                    }
                    else
                    {
                        filterContext.Result = new RedirectResult(System.Web.Security.FormsAuthentication.LoginUrl + "?ReturnUrl=" + filterContext.HttpContext.Server.UrlEncode(filterContext.HttpContext.Request.RawUrl));
                    }
                }
            }
        }
    }

    public class CheckDoctorAuthorization : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (string.IsNullOrWhiteSpace(CookieHelper.Get(StaticValues.SessionUserId)) || (string.IsNullOrWhiteSpace(CookieHelper.Get(StaticValues.SessionRoleId)) || !HttpContext.Current.Request.IsAuthenticated))
            {
                if (filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    filterContext.HttpContext.Response.StatusCode = 302; //Found Redirection to another page. Here- login page. Check Layout ajaxError() script.
                    filterContext.HttpContext.Response.End();
                }
                else
                {
                    filterContext.Result = new RedirectResult(System.Web.Security.FormsAuthentication.LoginUrl + "?ReturnUrl=" + filterContext.HttpContext.Server.UrlEncode(filterContext.HttpContext.Request.RawUrl));
                }
            }
            else
            {
                var roleId = Convert.ToInt32(CookieHelper.Get(StaticValues.SessionRoleId));

                if (roleId != (int)EnumList.Roles.Doctor)
                {
                    if (filterContext.HttpContext.Request.IsAjaxRequest())
                    {
                        filterContext.HttpContext.Response.StatusCode = 302; //Found Redirection to another page. Here- login page. Check Layout ajaxError() script.
                        filterContext.HttpContext.Response.End();
                    }
                    else
                    {
                        filterContext.Result = new RedirectResult(System.Web.Security.FormsAuthentication.LoginUrl + "?ReturnUrl=" + filterContext.HttpContext.Server.UrlEncode(filterContext.HttpContext.Request.RawUrl));
                    }
                }
            }
        }
    }

    public class CheckPatientAuthorization : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (string.IsNullOrWhiteSpace(CookieHelper.Get(StaticValues.SessionUserId)) || (string.IsNullOrWhiteSpace(CookieHelper.Get(StaticValues.SessionRoleId)) || !HttpContext.Current.Request.IsAuthenticated))
            {
                if (filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    filterContext.HttpContext.Response.StatusCode = 302; //Found Redirection to another page. Here- login page. Check Layout ajaxError() script.
                    filterContext.HttpContext.Response.End();
                }
                else
                {
                    filterContext.Result = new RedirectResult(System.Web.Security.FormsAuthentication.LoginUrl + "?ReturnUrl=" + filterContext.HttpContext.Server.UrlEncode(filterContext.HttpContext.Request.RawUrl));
                }
            }
            else
            {
                var roleId = Convert.ToInt32(CookieHelper.Get(StaticValues.SessionRoleId));

                if (roleId != (int)EnumList.Roles.Patient)
                {
                    if (filterContext.HttpContext.Request.IsAjaxRequest())
                    {
                        filterContext.HttpContext.Response.StatusCode = 302; //Found Redirection to another page. Here- login page. Check Layout ajaxError() script.
                        filterContext.HttpContext.Response.End();
                    }
                    else
                    {
                        filterContext.Result = new RedirectResult(System.Web.Security.FormsAuthentication.LoginUrl + "?ReturnUrl=" + filterContext.HttpContext.Server.UrlEncode(filterContext.HttpContext.Request.RawUrl));
                    }
                }
            }
        }
    }
}