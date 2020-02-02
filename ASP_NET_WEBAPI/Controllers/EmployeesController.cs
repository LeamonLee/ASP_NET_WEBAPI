using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using EmployeeDataAccess;
using System.Threading;
using System.Web.Http.Cors;

namespace ASP_NET_WEBAPI.Controllers
{
    public class EmployeesController : ApiController
    {

        [EnableCorsAttribute("*", "*", "*")]
        [BasicAuthentication]
        public HttpResponseMessage Get(string gender = "All")
        {
            string username = Thread.CurrentPrincipal.Identity.Name;

            using (EmployeeDBEntities entities = new EmployeeDBEntities())
            {
                switch (username.ToLower())
                {
                    case "male":
                        return Request.CreateResponse(HttpStatusCode.OK,
                            entities.Employees.Where(e => e.Gender.ToLower() == "male").ToList());
                    case "female":
                        return Request.CreateResponse(HttpStatusCode.OK,
                            entities.Employees.Where(e => e.Gender.ToLower() == "female").ToList());
                    default:
                        return Request.CreateResponse(HttpStatusCode.BadRequest,
                            "Value for gender must be Male, or Female. " + gender + " is invalid.");
                }
            }
        }

        //public HttpResponseMessage Get(string gender = "All")
        //{
        //    using (EmployeeDBEntities entities = new EmployeeDBEntities())
        //    {
        //        switch (gender.ToLower())
        //        {
        //            case "all":
        //                return Request.CreateResponse(HttpStatusCode.OK,
        //                    entities.Employees.ToList());
        //            case "male":
        //                return Request.CreateResponse(HttpStatusCode.OK,
        //                    entities.Employees.Where(e => e.Gender.ToLower() == "male").ToList());
        //            case "female":
        //                return Request.CreateResponse(HttpStatusCode.OK,
        //                    entities.Employees.Where(e => e.Gender.ToLower() == "female").ToList());
        //            default:
        //                return Request.CreateErrorResponse(HttpStatusCode.BadRequest,
        //                    "Value for gender must be Male, Female or All. " + gender + " is invalid.");
        //        }
        //    }
        //}


        //[HttpGet]
        //public IEnumerable<Employee> LoadAllEmployees()
        //{
        //    using (EmployeeDBEntities entities = new EmployeeDBEntities())
        //    {
        //        return entities.Employees.ToList();
        //    }

        //}

        //public IEnumerable<Employee> Get()
        //{
        //    using(EmployeeDBEntities entities = new EmployeeDBEntities())
        //    {
        //        return entities.Employees.ToList();
        //    }

        //}

        public HttpResponseMessage Get(int id) // The variable name has to be 'id'. If you name it to other name, it would fail.
        {
            using (EmployeeDBEntities entities = new EmployeeDBEntities())
            {
                var entity = entities.Employees.FirstOrDefault(e => e.ID == id);
                if (entity != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, entity);
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound,
                        "Employee with Id " + id.ToString() + " not found");
                }
            }

        }

        public HttpResponseMessage Post([FromBody] Employee employee)
        {
            try
            {
                using (EmployeeDBEntities entities = new EmployeeDBEntities())
                {
                    entities.Employees.Add(employee);
                    entities.SaveChanges();

                    var message = Request.CreateResponse(HttpStatusCode.Created, employee);
                    message.Headers.Location = new Uri(Request.RequestUri + employee.ID.ToString());
                    System.Diagnostics.Debug.WriteLine(Request.RequestUri);
                    System.Diagnostics.Debug.WriteLine(message.Headers.Location);
                    return message;
                }
            }
            catch(Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
            
        }

        public HttpResponseMessage Delete(int id)
        {
            try
            {
                using (EmployeeDBEntities entities = new EmployeeDBEntities())
                {
                    var entity = entities.Employees.FirstOrDefault(e => e.ID == id);
                    if (entity == null)
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound,
                            "Employee with Id = " + id.ToString() + " not found to delete");
                    }
                    else
                    {
                        entities.Employees.Remove(entity);
                        entities.SaveChanges();
                        return Request.CreateResponse(HttpStatusCode.OK, new { message = "successfull"});
                    }
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        public HttpResponseMessage Put(int id, [FromBody]Employee employee)
        {
            try
            {
                using (EmployeeDBEntities entities = new EmployeeDBEntities())
                {
                    var entity = entities.Employees.FirstOrDefault(e => e.ID == id);
                    if (entity == null)
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound,
                            "Employee with Id " + id.ToString() + " not found to update");
                    }
                    else
                    {
                        entity.FirstName = employee.FirstName;
                        entity.LastName = employee.LastName;
                        entity.Gender = employee.Gender;
                        entity.Salary = employee.Salary;

                        entities.SaveChanges();

                        return Request.CreateResponse(HttpStatusCode.OK, entity);
                    }
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }
    }
}
