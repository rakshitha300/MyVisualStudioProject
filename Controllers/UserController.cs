using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using cafe_management_system.Models;
using System.IO;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Security.Cryptography.X509Certificates;
using System.Web.Helpers;


namespace cafe_management_system.Controllers
{
    [RoutePrefix("api/user")]
    public class UserController : ApiController
    {
    CafeVideoEntities db = new CafeVideoEntities();
        Response response = new Response();

        [HttpPost, Route("signup")]

        public HttpResponseMessage Signup([FromBody] user user)
        {
            try
            {
                user userobj = db.users
                    .Where(u => u.email == user.email).FirstOrDefault();
                if (userobj == null)
                {
                    user.role = "user";
                    user.status = "false";
                    db.users.Add(user);
                    db.SaveChanges();
                    return Request.CreateResponse(HttpStatusCode.OK, new { message = "Successfully Registered" });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = " Email already exist" });
                }
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, e);
            }

        }

        [HttpPost, Route("login")]
        public HttpResponseMessage Login([Frombody] user user)
        {
            try
            {
                user userobj = db.users
                    .Where(u => (u.email == user.email && u.password == user.password)).FirstOrDefault();
                if (userobj != null)
                {
                    if (userobj.status == "true")
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, new { token = TokenManager.GenerateToken(userobj.email, userobj.role) });
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.Unauthorized, new { message = "wait for admin approval" });
                    }


                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, new { message = "Incorrect username or password" });
                }
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, e);
            }

        }

        [HttpGet,Route("checkToken")]
        [CustomAuthenticationFilter]

        public HttpResponseMessage checkToken()
        {
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "true" });
        }

        [HttpGet, Route("getAllUser")]
        [CustomAuthenticationFilter]

        public HttpResponseMessage GetAllUser()
        {
            try
            {
                var token = Request.Headers.GetValues("authorization").First();
                TokenClaim tokenClaim = TokenManager.ValidateToken(token);
                if (tokenClaim.Role != "admin")
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized);
                }
                var result = db.users
                    .Select(u => new { u.id, u.name, u.contactnumber, u.email, u.status, u.role })
                    .Where(x => (x.role == "user"))
                    .ToList();
                return Request.CreateResponse(HttpStatusCode.OK, result);

            }

            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }

        }


        [HttpPost, Route("updateUserStatus")]
        [CustomAuthenticationFilter]

        public HttpResponseMessage UpdateUserStatus(user user)
        { 
            try
            {
                var token = Request.Headers.GetValues("authorization").First();
                TokenClaim tokenClaim = TokenManager.ValidateToken(token);
                if (tokenClaim.Role != "admin")
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized);
                }
                user userObj = db.users.Find(user.id);
                if (userObj == null)
                {
                    response.message = "User id does not found";
                    return Request.CreateResponse(HttpStatusCode.OK, response);
                }
                userObj.status = user.status;
                db.Entry(userObj).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                response.message = "User Status Updated Successfully";
                return Request.CreateResponse(HttpStatusCode.OK, response);

            }


            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost, Route("changePassword")]
        [CustomAuthenticationFilter]

        public HttpResponseMessage ChangePassword(ChangePassword changePassword)
        {
            try
            {
                var token = Request.Headers.GetValues("authorization").First();
                TokenClaim tokenClaim = TokenManager.ValidateToken(token);

                user userObj = db.users
                    .Where(x => (x.email == tokenClaim.email && x.password == changePassword.OldPassword)).FirstOrDefault();
                if(userObj != null)
                {
                    userObj.password = changePassword.NewPassword;
                    db.Entry(userObj).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    response.message = "Password Updated Successfully";
                    return Request.CreateResponse(HttpStatusCode.OK, response);
                 }
                else {
                    response.message = "Incorrect old password";
                    return Request.CreateResponse(HttpStatusCode.BadRequest,response);

                 }

                }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }
        private string createEmailBody(string email, string password)
        {
            try
            {
                string body = string.Empty;
                using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~/Template/forgot-password.html")))
                {
                    body = reader.ReadToEnd();
                }

                body = body.Replace("{email}", email);
                body = body.Replace("{password}", password);
                body = body.Replace("{frontendUrl}", "http://localhost:4200/");
                return body;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        [HttpPost, Route("forgotPassword")]
        public async Task<HttpResponseMessage> ForgotPassword([FromBody] user user)
        {
            user userObj = db.users
                .Where(x => x.email == user.email).FirstOrDefault();
            response.message = "Password sent successfully to your email.";
            if (userObj == null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            var message = new MailMessage();
            message.To.Add(new MailAddress(user.email));
            message.Subject = "Password by Cafe Management System";
            message.Body = createEmailBody(user.email, userObj.password);
            message.IsBodyHtml = true;
            using (var smtp = new SmtpClient())
            {
                await smtp.SendMailAsync(message);
                await Task.FromResult(0);
            }
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }
    }
}

