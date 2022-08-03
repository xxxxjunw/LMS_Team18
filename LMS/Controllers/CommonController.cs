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
    public class CommonController : Controller
    {
        //If your context class is named differently, fix this
        //and the constructor parameter
        private readonly LMSContext db;

        public CommonController(LMSContext _db)
        {
            db = _db;
        }

        /*******Begin code to modify********/

        /// <summary>
        /// Retreive a JSON array of all departments from the database.
        /// Each object in the array should have a field called "name" and "subject",
        /// where "name" is the department name and "subject" is the subject abbreviation.
        /// </summary>
        /// <returns>The JSON array</returns>
        public IActionResult GetDepartments()
        {
            var query =
                from d in db.Departments
                select new { name = d.Name, subject = d.Subject };
            return Json(query.ToArray());
        }



        /// <summary>
        /// Returns a JSON array representing the course catalog.
        /// Each object in the array should have the following fields:
        /// "subject": The subject abbreviation, (e.g. "CS")
        /// "dname": The department name, as in "Computer Science"
        /// "courses": An array of JSON objects representing the courses in the department.
        ///            Each field in this inner-array should have the following fields:
        ///            "number": The course number (e.g. 5530)
        ///            "cname": The course name (e.g. "Database Systems")
        /// </summary>
        /// <returns>The JSON array</returns>
        public IActionResult GetCatalog()
        {
            Console.WriteLine("Now inside of student cat");
            var query =
                from d in db.Departments
                join c in db.Courses
                on d.Subject equals c.Subject
                select new
                {
                    subject = d.Subject,
                    dname = d.Name,
                    courses = new { number = c.CourseNum, cname = c.Name }
                };
            return Json(query.ToArray());
        }

        /// <summary>
        /// Returns a JSON array of all class offerings of a specific course.
        /// Each object in the array should have the following fields:
        /// "season": the season part of the semester, such as "Fall"
        /// "year": the year part of the semester
        /// "location": the location of the class
        /// "start": the start time in format "hh:mm:ss"
        /// "end": the end time in format "hh:mm:ss"
        /// "fname": the first name of the professor
        /// "lname": the last name of the professor
        /// </summary>
        /// <param name="subject">The subject abbreviation, as in "CS"</param>
        /// <param name="number">The course number, as in 5530</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetClassOfferings(string subject, int number)
        {
            var query =
                from co in db.Courses
                join cl in db.Classes on co.CourseNum equals cl.CourseNum
                join p in db.Professors on cl.ProfessorId equals p.UId
                where co.Subject == subject && co.CourseNum == number
                select new
                {
                    season = cl.Semester,
                    year = cl.Year,
                    location = cl.Location,
                    start = cl.StartDate.ToString(),
                    end = cl.EndDate.ToString(),
                    fname = p.FirstName,
                    lname = p.LastName
                };

            return Json(query.ToArray());
        }
        

        /// <summary>
        /// This method does NOT return JSON. It returns plain text (containing html).
        /// Use "return Content(...)" to return plain text.
        /// Returns the contents of an assignment.
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The name of the assignment in the category</param>
        /// <returns>The assignment contents</returns>
        public IActionResult GetAssignmentContents(string subject, int num, string season, int year, string category, string asgname)
        {
            var classID = getClassID(subject, num, season, year);

            var query =
                from ac in db.AssignmentCategories
                join a in db.Assignments on ac.CId equals a.CId
                where ac.CId == classID && ac.Name == category && a.Name == asgname
                select a.Contents;

            return Content(query.ToArray().Length == 0 ? null : query.ToArray()[0]);
        }
        public int getClassID(string subject, int num, string season, int year)
        {
            var query =
                from co in db.Courses
                join cl in db.Classes on co.CourseNum equals cl.CourseNum
                where subject == co.Subject && num == co.CourseNum
                    && season == cl.Semester && year == cl.Year
                select cl.CId;

            return query.ToArray()[0];
        }

        /// <summary>
        /// This method does NOT return JSON. It returns plain text (containing html).
        /// Use "return Content(...)" to return plain text.
        /// Returns the contents of an assignment submission.
        /// Returns the empty string ("") if there is no submission.
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The name of the assignment in the category</param>
        /// <param name="uid">The uid of the student who submitted it</param>
        /// <returns>The submission text</returns>
        public IActionResult GetSubmissionText(string subject, int num, string season, int year, string category, string asgname, string uid)
        {
            var classID = getClassID(subject, num, season, year);

            var query =
                from ac in db.AssignmentCategories
                join a in db.Assignments on ac.Id equals a.CId
                join s in db.Submissions on a.AId equals s.AId
                where ac.CId == classID && ac.Name == category
                    && a.Name == asgname && uid == s.UId
                select s.Contents;

            if (query.Count() == 0)
                return Content("");

            return Content(query.ToArray()[0]);
        }


        /// <summary>
        /// Gets information about a user as a single JSON object.
        /// The object should have the following fields:
        /// "fname": the user's first name
        /// "lname": the user's last name
        /// "uid": the user's uid
        /// "department": (professors and students only) the name (such as "Computer Science") of the department for the user. 
        ///               If the user is a Professor, this is the department they work in.
        ///               If the user is a Student, this is the department they major in.    
        ///               If the user is an Administrator, this field is not present in the returned JSON
        /// </summary>
        /// <param name="uid">The ID of the user</param>
        /// <returns>
        /// The user JSON object 
        /// or an object containing {success: false} if the user doesn't exist
        /// </returns>
        public IActionResult GetUser(string uid)
        {
            var query =
                from s in db.Students
                join d in db.Departments on s.Subject equals d.Subject
                where uid == s.UId
                select new { fname = s.FirstName, lname = s.LastName, uid, department = d.Name };
            if (query.Count() == 1)
                return Json(query.ToArray()[0]);

            // Check if the user is a professor
            query =
                from p in db.Professors
                join d in db.Departments
                on p.Department equals d.Subject
                where uid == p.UId
                select new { fname = p.FirstName, lname = p.LastName, uid, department = d.Name };
            if (query.Count() == 1)
                return Json(query.ToArray()[0]);

            // Check if the user is an administrator
            var query1 =
                from a in db.Administrators
                where uid == a.UId
                select new { fname = a.FirstName, lname = a.LastName, uid };
            if (query.Count() == 1)
            {
                return Json(query1.ToArray()[0]);
            }

            // If the uid doesn't show up in the three tables,
            // that means the user doesn't exist.
            return Json(new { success = false });
        }


        /*******End code to modify********/
    }
}

