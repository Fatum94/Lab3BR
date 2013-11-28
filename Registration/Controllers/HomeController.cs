using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.Configuration;
using Registration.Models;
using System.Data;
using System.Text.RegularExpressions;
using System.IO;
using System.Text;
using System.Security;


namespace System.Web.Security
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index(User user)
        {
            if (Request.Cookies["auth_test"] == null || Request.Cookies["auth_test"].Value == null)
            {
                return RedirectToAction("Register");
            }

            return View();
        }
        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase FileUpload)
        {
            DataTable dt = new DataTable();
            if (FileUpload != null && FileUpload.ContentLength > 0)
            {
                string fileName = Path.GetFileName(FileUpload.FileName);
                string path = Path.Combine(Server.MapPath("~/App_Data/uploads"), fileName);
                try
                {
                    FileUpload.SaveAs(path);
                    ProcessCSV(path);
                }
                catch (Exception ex)
                {
                    ViewData["Feedback"] = ex.Message;
                }
            }
            dt.Dispose();
            return RedirectToAction("Index");
        }

        public ActionResult Registration()
        {
            ViewData["Message"] = "Register Here!";
            var model = new User();
            return View("Registration", model);
        }

        public ActionResult WatchDB(User user)
        {
            ViewData = getFromTable(user);
            return RedirectToAction("Index");
        }

        public ActionResult Register(User user)
        {
            return View();
        }
        public ActionResult LogOut()
        {
            HttpCookie myCookie = new HttpCookie("auth_test");
            myCookie.Expires = DateTime.Now.AddDays(-1d);
            Response.Cookies.Add(myCookie);
            return RedirectToAction("Register");
        }

        public ViewDataDictionary getFromTable(User user)
        {
            var database = new Database();
            var userLine = database.Users.Where(u => u.Name == user.Name).FirstOrDefault();
            if (userLine.Password == user.Password) {
                var hash = Convert.ToBase64String(
                      System.Security.Cryptography.MD5.Create()
                      .ComputeHash(Encoding.UTF8.GetBytes(userLine.Password))
                    );


                var AuthCookie = new HttpCookie("auth_test")
                {
                    Value = hash,
                    Expires = DateTime.Now.Add(FormsAuthentication.Timeout)
                };
                user.isAuth = true;
                HttpContext.Response.Cookies.Set(AuthCookie);
            }
            
            return null;
        }

        static private string GetConnectionString(string DBname)
        {
            // To avoid storing the connection string in your code, 
            // you can retrieve it from a configuration file.
            return "Data Source=.\\SQLEXPRESS;AttachDbFilename=D:\\obrylTest\\netTest\\Wtf\\Registration\\App_Data\\" + DBname + ";Integrated Security=True;User Instance=True";
        }

        public ActionResult InsertCompressorCharacter(Kompressor compr)
        {
            if (ModelState.IsValid)
            {
                string connectionString = GetConnectionString("Compressor.mdf");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand cmd = new SqlCommand("insert into Compressor values('" + compr.PressIn + "','" + compr.PressOut + "','" + compr.Performance + "','" + compr.Rodo + "')", connection);
                cmd.ExecuteNonQuery();
                connection.Close();
                return RedirectToAction("Index");
            }
            return View("Index");
        }

        public ActionResult ConvertDataToCSV()
        {
            var database = new Database();
            var arr = database.Compressor.ToArray();
            string connectionString = GetConnectionString("Compressor.mdf");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand cmd = new SqlCommand("select * from Compressor", connection);
            SqlDataReader reader = cmd.ExecuteReader();
            string line;
            var sw = new StreamWriter("D:\\obrylTest\\netTest\\Wtf\\Registration\\App_Data\\uploads\\history.csv");
            var j = 0;
            //while (reader.Read())
            //{
            //    line = string.Empty;
            //    for (var i = 0; i < reader.FieldCount; i++)
            //    {
            //        line += reader.GetString(i) + ",";

            //    }
            //    sw.WriteLine(line);
            //}


            foreach (var rows in arr)
            {
                sw.WriteLine(rows.PressIn);
            }
            sw.Dispose();
            return RedirectToAction("Index");
        }

        private void CreateCookie(string userName, bool isPersistent = false)
        {
            var ticket = new FormsAuthenticationTicket(
                  1,
                  userName,
                  DateTime.Now,
                  DateTime.Now.Add(FormsAuthentication.Timeout),
                  isPersistent,
                  string.Empty,
                  FormsAuthentication.FormsCookiePath);

            // Encrypt the ticket.
            var encTicket = FormsAuthentication.Encrypt(ticket);

            // Create the cookie.
            var AuthCookie = new HttpCookie("auth_test")
            {
                Value = encTicket,
                Expires = DateTime.Now.Add(FormsAuthentication.Timeout)
            };
            HttpContext.Response.Cookies.Set(AuthCookie);
        }

        private void ProcessCSV(string fileName)
        {
            var database = new Database();

            //Set up our variables
            string Feedback = string.Empty;
            string line = string.Empty;
            string[] strArray = { "", "", "", "" };
            // work out where we should split on comma, but not in a sentence
            Regex r = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
            //Set the filename in to our stream
            var sr = new StreamReader(fileName);
            while ((line = sr.ReadLine()) != null)
            {

                //add our current value to our data row
                strArray = r.Split(line);

                database.Compressor.Add(new Kompressor { PressIn = strArray[0], PressOut = strArray[1], Performance = strArray[2], Rodo = strArray[3] });
                database.SaveChanges();
            }

        }
    }

}

