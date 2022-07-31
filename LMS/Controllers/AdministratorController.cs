using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using LMS.Models.LMSModels;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LMS.Controllers
{
    public class AdministratorController : Controller
    {

        //If your context class is named something different,
        //fix this member var and the constructor param
        private readonly LMSContext db;

        public AdministratorController(LMSContext _db)
        {
            db = _db;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Department(string subject)
        {
            ViewData["subject"] = subject;
            return View();
        }

        public IActionResult Course(string subject, string num)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            return View();
        }

        /*******Begin code to modify********/

        /// <summary>
        /// Create a department which is uniquely identified by it's subject code
        /// </summary>
        /// <param name="subject">the subject code</param>
        /// <param name="name">the full name of the department</param>
        /// <returns>A JSON object containing {success = true/false}.
        /// false if the department already exists, true otherwise.</returns>
        public IActionResult CreateDepartment(string subject, string name)
        {

            var query =
                from d in db.Departments
                where subject == d.Subject && name == d.Name 
                select d;
            // if it exists, return false
            if (query.Count() > 0)
                return Json(new { success = false });

            // if it doesn't exist, create a new course
            Department dept = new Department();
            dept.Subject = subject;
            dept.Name = name;
            db.Departments.Add(dept);
            db.SaveChanges();

            return Json(new { success = true });
        }


        /// <summary>
        /// Returns a JSON array of all the courses in the given department.
        /// Each object in the array should have the following fields:
        /// "number" - The course number (as in 5530)
        /// "name" - The course name (as in "Database Systems")
        /// </summary>
        /// <param name="subjCode">The department subject abbreviation (as in "CS")</param>
        /// <returns>The JSON result</returns>
        public IActionResult GetCourses(string subject)
        {

            var query =
                from c in db.Courses
                where c.Subject == subject
                select new
                {
                    number = c.CourseNum,
                    name = c.Name
                };

            return Json(query.ToArray());
        }

        /// <summary>
        /// Returns a JSON array of all the professors working in a given department.
        /// Each object in the array should have the following fields:
        /// "lname" - The professor's last name
        /// "fname" - The professor's first name
        /// "uid" - The professor's uid
        /// </summary>
        /// <param name="subject">The department subject abbreviation</param>
        /// <returns>The JSON result</returns>
        public IActionResult GetProfessors(string subject)
        {

            var query =
                from p in db.Professors
                where p.Department == subject
                select new
                {
                    lname = p.FirstName,
                    fname = p.LastName,
                    uid = p.UId
                };

            return Json(query.ToArray());

        }



        /// <summary>
        /// Creates a course.
        /// A course is uniquely identified by its number + the subject to which it belongs
        /// </summary>
        /// <param name="subject">The subject abbreviation for the department in which the course will be added</param>
        /// <param name="number">The course number</param>
        /// <param name="name">The course name</param>
        /// <returns>A JSON object containing {success = true/false}.
        /// false if the course already exists, true otherwise.</returns>
        public IActionResult CreateCourse(string subject, int number, string name)
        {
            var query =
                from co in db.Courses
                where subject == co.Subject && number == co.CourseNum && name == co.Name
                select co;
            // if it exists, return false
            if (query.Count() > 0)
                return Json(new { success = false });

            // if it doesn't exist, create a new course
            Course c = new Course();
            c.Subject = subject;
            c.Name = name;
            c.CourseNum = (uint)number;
            db.Courses.Add(c);
            db.SaveChanges();

            return Json(new { success = true });
        }



        /// <summary>
        /// Creates a class offering of a given course.
        /// </summary>
        /// <param name="subject">The department subject abbreviation</param>
        /// <param name="number">The course number</param>
        /// <param name="season">The season part of the semester</param>
        /// <param name="year">The year part of the semester</param>
        /// <param name="start">The start time</param>
        /// <param name="end">The end time</param>
        /// <param name="location">The location</param>
        /// <param name="instructor">The uid of the professor</param>
        /// <returns>A JSON object containing {success = true/false}. 
        /// false if another class occupies the same location during any time 
        /// within the start-end range in the same semester, or if there is already
        /// a Class offering of the same Course in the same Semester,
        /// true otherwise.</returns>
        public IActionResult CreateClass(string subject, int number, string season, int year, DateTime start, DateTime end, string location, string instructor)
        {
            var query =
                from co in db.Courses
                where subject == co.Subject && number == co.CourseNum
                select co;
            uint courseID = query.ToArray()[0].CourseNum;

            // To see if there is already a class offering of the same course in the same semester
            var query1 =
                from cl in db.Classes
                where cl.CourseNum == courseID && cl.Year == year && cl.Semester == season
                select cl;
            if (query1.Count() > 0)
                return Json(new { success = false });

            // To see if the time clashes with another class
            var query2 =
                from cl in db.Classes
                where cl.Year == year && cl.Semester == season && cl.Location == location
                select new { start = cl.StartDate, end = cl.EndDate };
            //var intervals = query2.ToArray();
            //foreach (var interval in intervals)
            //{
            //    if (interval.start > DateOnly.FromDateTime(end) || interval.end > DateOnly.FromDateTime(start))
            //    {
            //        return Json(new { success = false });
            //    }
            //}

            Class c = new Class();
            c.Year = (uint)year;
            c.Semester = season;
            c.Location = location;
            c.CatalogId = 1111;
            c.StartDate = DateOnly.FromDateTime(start);
            c.EndDate = DateOnly.FromDateTime(end);
            c.ProfessorId = instructor;
            c.CourseNum = courseID;
            db.Classes.Add(c);
            db.SaveChanges();

            return Json(new { success = true });
        }


        /*******End code to modify********/

    }
}

