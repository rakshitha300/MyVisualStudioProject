using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using cafe_management_system.Models;

namespace cafe_management_system.Controllers
{
    [RoutePrefix("api/category")]
    public class CategoryController : ApiController
    {
        CafeVideoEntities db = new CafeVideoEntities();
        Response response = new Response();

        [HttpPost, Route("addNewCategory")]
        [CustomAuthenticationFilter]
        public HttpResponseMessage AddNewCategory([FromBody] category category)
        {
            try
            {
                var token = Request.Headers.GetValues("authorization").First();
                TokenClaim tokenClaim = TokenManager.ValidateToken(token);
                if (tokenClaim.Role != "admin")
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized);
                }

                db.categories.Add(category);
                db.SaveChanges();
                response.message = "Category Added Successfully";
                return Request.CreateResponse(HttpStatusCode.OK, response);

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);

            }
        }

        [HttpGet, Route("getAllCategory")]
        [CustomAuthenticationFilter]

        public HttpResponseMessage GetAllCategory()
        {
            try
            { 
            return Request.CreateResponse(HttpStatusCode.OK, db.categories.ToList());
            
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError,ex);
            }

        }

        [HttpPost, Route("updateCategory")]
        [CustomAuthenticationFilter]

        public HttpResponseMessage UpdateCategory(category category)
        {
            try
            {
                var token = Request.Headers.GetValues("authorization").First();
                TokenClaim tokenClaim = TokenManager.ValidateToken(token);
                if (tokenClaim.Role != "admin")
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized);
                }
                category categoryObj = db.categories.Find(category.id);
                if (categoryObj == null)
                {
                    response.message = "Category id does not found";
                    return Request.CreateResponse(HttpStatusCode.OK, response);
                }
                categoryObj.name = category.name;
                db.Entry(categoryObj).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                response.message = "Category updated successfully";
                return Request.CreateResponse(HttpStatusCode.OK, response);

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);

            }
        }
    }
}
