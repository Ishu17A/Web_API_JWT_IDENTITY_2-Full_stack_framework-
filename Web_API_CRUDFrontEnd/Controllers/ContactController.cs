using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Web_API_CRUD.Models;

namespace Web_API_CRUDFrontEnd.Controllers
{
    public class ContactController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            IEnumerable<Contact> employees = null;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7238/");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", HttpContext.Session.GetString("Token"));

                var responseTask = client.GetAsync("api/Contacts/GetContacts");
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsStringAsync();
                    var deserialized = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Contact>>(readTask.Result);
                    readTask.Wait();
                    employees = deserialized;
                }
                else
                {
                    employees = Enumerable.Empty<Contact>();
                    ModelState.AddModelError(string.Empty, "Employees not found.");
                }
            }
            return View(employees);
        }
        [HttpGet]
        public IActionResult AddEmployee()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddEmployee(Contact employee)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7238/");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", HttpContext.Session.GetString("Token"));

                var postTask = client.PostAsJsonAsync<Contact>("api/Contacts/AdddContact/Add", employee);
                postTask.Wait();
                var result = postTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
            }
            ModelState.AddModelError(string.Empty, "Failed Try again.");
            return View(employee);
        }

        /*[HttpGet]
        public ActionResult UpdateEmployee(Guid id)
        {
            using (var client = new HttpClient())
            {

                return View(contact);
            }
        }


        [HttpPost]
        public ActionResult UpdateEmployee(Contact employee)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7238/");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", HttpContext.Session.GetString("Token"));

                var postTask = client.GetAsync($"api/Contacts/UpdateContact/update/{employee.Id}");
                postTask.Wait();
                var result = postTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
            }
            ModelState.AddModelError(string.Empty, "Failed Try again.");
            return View(employee);
        }*/

        [HttpGet]
        public async Task<IActionResult> UpdateEmployee(Guid id)
        {
            Contact contact = null;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7238/");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));

                var response = await client.GetAsync($"api/Contacts/Getcontact/GetById/{id}");
      
                if (response.IsSuccessStatusCode)
                {
                    var content =  response.Content.ReadAsStringAsync().Result;
                    contact = JsonConvert.DeserializeObject<Contact>(content);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Contact not found.");
                }
            }
            return View(contact);
        }

        [HttpPost]
        public async Task<ActionResult> UpdateEmployee(Contact contact)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7238/");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));

                var json = JsonConvert.SerializeObject(contact);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PutAsync($"api/Contacts/UpdateContact/update/{contact.Id}", content);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
            }

            ModelState.AddModelError(string.Empty, "Failed to update contact. Please try again.");
            return View(contact);
        }







        [HttpGet]
        public IActionResult DeleteEmployee()
        {
            return View();
        }

        [HttpPost, ActionName("DeleteEmployee")]
            public async Task<ActionResult> DeletData(Guid id)
            {
                try
                {
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri("https://localhost:7238/");
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", HttpContext.Session.GetString("Token"));

                        var postTask = await client.DeleteAsync($"api/Contacts/Deletecontact/Delete/{id}");
                    var result = await postTask.Content.ReadAsStringAsync();
                        if (postTask.IsSuccessStatusCode)
                        {
                            return RedirectToAction("Index");
                        }
                    }
                    return Json("\"Failed Try again.\"");
                }
                catch (Exception ex) { throw ex; }
            }

        /*[HttpGet]
        public IActionResult AddFeedback()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AddFeedback(Feedback feedback)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7130/");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", HttpContext.Session.GetString("Token"));

                var postTask = client.PostAsJsonAsync<Feedback>("api/Employee/addFeedback", feedback);
                postTask.Wait();
                var result = postTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
            }
            ModelState.AddModelError(string.Empty, "Failed Try again.");
            return View(feedback);
        }

        [HttpGet]
        public ActionResult GetFeedbacks(int id)
        {
            IEnumerable<Feedback> feedback = null;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7130/");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", HttpContext.Session.GetString("Token"));

                var postTask = client.GetAsync($"api/Employee/feedbacks/{id}");
                postTask.Wait();
                var result = postTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsStringAsync();
                    var deserialized = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Feedback>>(readTask.Result);
                    readTask.Wait();
                    feedback = deserialized;
                }
            }
            return View(feedback);
        }
        
        
        [HttpGet]
        public IActionResult GetEmployeeById(Guid id)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7238/");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", HttpContext.Session.GetString("Token"));

                var postTask = client.GetAsync($"api/Employee/GetEmployeeById/{id}");
                postTask.Wait();
                var result = postTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsStringAsync();
                    var deserialized = Newtonsoft.Json.JsonConvert.DeserializeObject<Contact>(readTask.Result);
                    readTask.Wait();

                    return Json(deserialized);
                }
            }
            return RedirectToAction("Index");
        }*/
    }
}
