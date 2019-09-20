using AyurvedOnCall.Models;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace AyurvedOnCall.Helpers
{
    public static class SendgridEmailHelpers
    {
        public static bool SendTempleteEmail(long templeteId, string to, string username, Dictionary<string, string> replacements)
        {
            var response = false;
            try
            {
                using (var db = new DBEntities())
                {
                    var templete = db.EmailTempletes.FirstOrDefault(s => s.EmailTempleteId == templeteId);

                    if (templete != null)
                    {
                        var body = replacements.Aggregate(templete.Body, (result, s) => result.Replace(s.Key, s.Value));

                        var apiKey = ConfigurationManager.AppSettings["SendGrid"];
                        var client = new SendGridClient(apiKey);
                        var from = new EmailAddress("noreply@ayurvedoncall.com", ConfigurationManager.AppSettings["DefaultEmailUser"]);
                        var toAddress = new EmailAddress(to, username);
                        var plainTextContent = string.Empty;
                        var htmlContent = body;
                        var msg = MailHelper.CreateSingleEmail(from, toAddress, templete.Subject, plainTextContent, htmlContent);
                        var sendEmailAsync = client.SendEmailAsync(msg);
                        response = true;
                    }
                }
            }
            catch (Exception)
            {
                response = false;
            }
            return response;
        }

        public static bool SendEmail(string subject, string body, string to, string username)
        {
            bool response;
            try
            {
                var apiKey = ConfigurationManager.AppSettings["SendGrid"]; //Set you're sendgri key in websconfig
                var client = new SendGridClient(apiKey);
                var from = new EmailAddress(ConfigurationManager.AppSettings["DefaultFromEmail"], ConfigurationManager.AppSettings["DefaultEmailUser"]);
                var toAddress = new EmailAddress(to, username);
                var plainTextContent = string.Empty;
                var htmlContent = body;
                var msg = MailHelper.CreateSingleEmail(from, toAddress, subject, plainTextContent, htmlContent);
                var sendEmailAsync = client.SendEmailAsync(msg);
                response = true;
            }
            catch (Exception)
            {
                response = false;
            }
            return response;
        }
    }
}