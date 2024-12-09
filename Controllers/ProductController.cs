using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using cafe_management_system.Models;

namespace cafe_management_system.Controllers
{
    [RoutePrefix("api/product")]
    public class ProductController : ApiController
    {
        CafeVideoEntities db = new CafeVideoEntities();
        Response response = new Response();

        [HttpPost, Route("addNewProduct")]
        [CustomAuthenticationFilter]
        public HttpResponseMessage AddNewProduct([FromBody] Product product)
        {
            try
            {
                var token = Request.Headers.GetValues("authorization").First();
                TokenClaim tokenClaim = TokenManager.ValidateToken(token);
                if (tokenClaim.Role != "admin")
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized);
                }

                product.status = "true";
                db.Products.Add(product);
                db.SaveChanges();

                response.message = "Product Added Successfully";
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpGet, Route("getAllProduct")]
        [CustomAuthenticationFilter]
        public HttpResponseMessage GetAllProduct()
        {
            try
            {
                var result = from Products in db.Products
                             join category in db.categories
                             on Products.categoryId equals category.id
                             select new
                             {
                                 Products.id,
                                 Products.NAME,
                                 Products.description,
                                 Products.price,
                                 Products.status,
                                 categoryId = category.id,
                                 categoryName = category.name
                             };

                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpGet, Route("getProductByCategory/{id}")]
        [CustomAuthenticationFilter]


        public HttpResponseMessage getProductByCategory(int id)
        {
            try
            {
                var result = db.Products
                    .Where(x => x.categoryId == id && x.status == "true")
                    .Select(x => new { x.id, x.NAME }).ToList();
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }


        [HttpGet, Route("getProductById/{id}")]
        [CustomAuthenticationFilter]


        public HttpResponseMessage GetProductById(int id) {
            try
            {
                Product productObj = db.Products.Find(id);
                return Request.CreateResponse(HttpStatusCode.OK, productObj);


            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);


            }
        }

        [HttpPost, Route("updateProduct")]
        [CustomAuthenticationFilter]

        public HttpResponseMessage UpdateProduct([FromBody] Product product) {

            try
            {
                var token = Request.Headers.GetValues("authorization").First();
                TokenClaim tokenClaim = TokenManager.ValidateToken(token);
                if (tokenClaim.Role != "admin")
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized);
                }

                Product productObj = db.Products.Find(product.id);
                if (productObj == null)
                {
                    response.message = "Product id does not found";
                    return Request.CreateResponse(HttpStatusCode.OK, response);
                }

                productObj.NAME = product.NAME;
                productObj.categoryId = product.categoryId;
                productObj.description = product.description;
                productObj.price = product.price;
                db.Entry(productObj).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                response.message = " Product Updated Successfully";
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }


        [HttpPost, Route("deleteProduct/{id}")]
        [CustomAuthenticationFilter]

        public HttpResponseMessage DeleteProduct(int id)
        {
            try
            {
                var token = Request.Headers.GetValues("authorization").First();
                TokenClaim tokenClaim = TokenManager.ValidateToken(token);
                if (tokenClaim.Role != "admin")
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized);
                }
                Product productObj = db.Products.Find(id);
                if (productObj == null)
                {
                    response.message = "Product id does not found";
                    return Request.CreateResponse(HttpStatusCode.OK, response);

                }

                db.Products.Remove(productObj);
                db.SaveChanges();
                response.message = "Product deleted successfully";
                return Request.CreateResponse(HttpStatusCode.OK,response);

            }

            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }


        [HttpPost, Route("updateProductStatus")]
        [CustomAuthenticationFilter]

        public HttpResponseMessage UpdateProductStatus([FromBody] Product product)
        {
            try
            {
                var token = Request.Headers.GetValues("authorization").First();
                TokenClaim tokenClaim = TokenManager.ValidateToken(token);
                if (tokenClaim.Role != "admin")
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized);
                }
                Product productObj = db.Products.Find(product.id);
                if (productObj == null)
                {
                    response.message = "Product id does not found";
                    return Request.CreateResponse(HttpStatusCode.OK, response);

                }
                productObj.status = product.status;
                db.Entry(productObj).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                response.message = "Product status updated successfully";
                return Request.CreateResponse(HttpStatusCode.OK, response);

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }

        }

     }
  
}
