using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LMS.Models.LMSModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LMS.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentController : Controller
    {
        //If your context is named something else, fix this and the
        //constructor param
        private LMSContext db;
        public StudentController(LMSContext _db)
        {
            db = _db;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Catalog()
        {
            return View();
        }

        public IActionResult Class(string subject, string num, string season, string year)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            return View();
        }

        public IActionResult Assignment(string subject, string num, string season, string year, string cat, string aname)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            ViewData["cat"] = cat;
            ViewData["aname"] = aname;
            return View();
        }


        public IActionResult ClassListings(string subject, string num)
        {
            System.Diagnostics.Debug.WriteLine(subject + num);
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            return View();
        }


        /*******Begin code to modify********/

        /// <summary>
        /// Returns a JSON array of the classes the given student is enrolled in.
        /// Each object in the array should have the following fields:
        /// "subject" - The subject abbreviation of the class (such as "CS")
        /// "number" - The course number (such as 5530)
        /// "name" - The course name
        /// "season" - The season part of the semester
        /// "year" - The year part of the semester
        /// "grade" - The grade earned in the class, or "--" if one hasn't been assigned
        /// </summary>
        /// <param name="uid">The uid of the student</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetMyClasses(string uid)
        {
            var query =
                from co in db.Courses
                join cl in db.Classes on co.CourseNum equals cl.CourseNum
                join e in db.EnrollmentGrades on cl.CId equals e.CId
                join s in db.Students on e.UId equals s.UId
                where s.UId == uid
                select new
                {
                    subject = co.Subject,
                    number = co.CourseNum,
                    name = co.Name,
                    season = cl.Semester,
                    year = cl.Year,
                    grade = e.Grade == null ? "--" : e.Grade
                };

            return Json(query.ToArray());
        }

        /// <summary>
        /// Returns a JSON array of all the assignments in the given class that the given student is enrolled in.
        /// Each object in the array should have the following fields:
        /// "aname" - The assignment name
        /// "cname" - The category name that the assignment belongs to
        /// "due" - The due Date/Time
        /// "score" - The score earned by the student, or null if the student has not submitted to this assignment.
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="uid"></param>
        /// <returns>The JSON array</returns>
        public IActionResult GetAssignmentsInClass(string subject, int num, string season, int year, string uid)
        {
            var classID = HelperController.getClassID(subject, num, season, year, db);

            var query =
                from ac in db.AssignmentCategories
                join a in db.Assignments on ac.Id equals a.CId
                where classID == ac.CId
                select a;

            var query1 =
                from q in query
                join s in db.Submissions
                on new { A = q.AId, B = uid } equals new { A = s.AId, B = s.UId }
                into join1
                from j in join1.DefaultIfEmpty()
                select new
                {
                    aname = q.Name,
                    cname = q.CIdNavigation.Name,
                    due = q.Due.ToString(),
                    score = j == null ? null : (int?)j.Score
                };

            return Json(query1.ToArray());
        }



        /// <summary>
        /// Adds a submission to the given assignment for the given student
        /// The submission should use the current time as its DateTime
        /// You can get the current time with DateTime.Now
        /// The score of the submission should start as 0 until a Professor grades it
        /// If a Student submits to an assignment again, it should replace the submission contents
        /// and the submission time (the score should remain the same).
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The new assignment name</param>
        /// <param name="uid">The student submitting the assignment</param>
        /// <param name="contents">The text contents of the student's submission</param>
        /// <returns>A JSON object containing {success = true/false}</returns>
        public IActionResult SubmitAssignmentText(string subject, int num, string season, int year,
          string category, string asgname, string uid, string contents)
        {
            var classID = HelperController.getClassID(subject, num, season, year, db);

            // To get the AssignmentID
            var query =
                from ac in db.AssignmentCategories
                join a in db.Assignments on ac.Id equals a.CId
                where classID == ac.CId && category == ac.Name && asgname == a.Name
                select a;
            int assignmentID = query.ToArray()[0].AId;

            // To see if the student has already submitted for the assignment
            var query1 =
                from s in db.Submissions
                where s.AId == assignmentID && uid == s.UId
                select s;
            // if he has, update the row
            if (query1.Count() == 1)
            {
                query1.ToArray()[0].Contents = contents;
                query1.ToArray()[0].Score = 0;
            }
            else
            // if he hasn't, create a new row
            {
                Submission s = new Submission();
                s.UId = uid;
                s.AId = assignmentID;
                s.Score = 0;
                s.Contents = contents;
                db.Submissions.Add(s);
            }

            db.SaveChanges();

            return Json(new { success = true });
        }


        /// <summary>
        /// Enrolls a student in a class.
        /// </summary>
        /// <param name="subject">The department subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester</param>
        /// <param name="year">The year part of the semester</param>
        /// <param name="uid">The uid of the student</param>
        /// <returns>A JSON object containing {success = {true/false}. 
        /// false if the student is already enrolled in the class, true otherwise.</returns>
        public IActionResult Enroll(string subject, int num, string season, int year, string uid)
        {
            var classID = HelperController.getClassID(subject, num, season, year,db);

            // To see if the student has already enrolled in that class
            var query1 =
                from e in db.EnrollmentGrades
                where e.CId == classID && e.UId == uid
                select e;
            // if he has already enrolled in the class, return false
            if (query1.Count() > 0)
            {
                return Json(new { success = false });
            }
            else
            // otherwise, create a new row
            {
                EnrollmentGrade e = new EnrollmentGrade();
                e.UId = uid;
                e.CId = classID;
                e.Grade = "--";
                db.EnrollmentGrades.Add(e);
                db.SaveChanges();
                return Json(new { success = true });
            }
        }



        /// <summary>
        /// Calculates a student's GPA
        /// A student's GPA is determined by the grade-point representation of the average grade in all their classes.
        /// Assume all classes are 4 credit hours.
        /// If a student does not have a grade in a class ("--"), that class is not counted in the average.
        /// If a student is not enrolled in any classes, they have a GPA of 0.0.
        /// Otherwise, the point-value of a letter grade is determined by the table on this page:
        /// https://advising.utah.edu/academic-standards/gpa-calculator-new.php
        /// </summary>
        /// <param name="uid">The uid of the student</param>
        /// <returns>A JSON object containing a single field called "gpa" with the number value</returns>
        public IActionResult GetGPA(string uid)
        {
            var map = new Dictionary<string, double>
            {
                { "A", 4.0 },
                { "A-", 3.7 },
                { "B+", 3.3 },
                { "B", 3.0 },
                { "B-", 2.7 },
                { "C+", 2.3 },
                { "C", 2.0 },
                { "C-", 1.7 },
                { "D+", 1.3 },
                { "D", 1.0 },
                { "D-", 0.7 },
                { "E", 0.0 }
            };

            var allGrades = (from e in db.EnrollmentGrades where uid == e.UId select e.Grade).ToArray();

            double totalGPA = 0.0;
            int count = 0;
            foreach (var grade in allGrades)
            {
                if (grade != "--")
                {
                    totalGPA += map[grade];
                    count++;
                }
            }

            // count == 0 means the student hasn't enrolled in any class
            //            or he hasn't received grades from any class he enrolled in
            if (count == 0)
                return Json(new { gpa = 0.0 });

            return Json(new { gpa = totalGPA / count });
        }

        /*******End code to modify********/

        

    }
}

