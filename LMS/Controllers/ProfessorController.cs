using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using LMS.Models.LMSModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LMS_CustomIdentity.Controllers
{
    [Authorize(Roles = "Professor")]
    public class ProfessorController : Controller
    {

        //If your context is named something else, fix this
        //and the constructor param
        private readonly LMSContext db;

        public ProfessorController(LMSContext _db)
        {
            db = _db;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Students(string subject, string num, string season, string year)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
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

        public IActionResult Categories(string subject, string num, string season, string year)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            return View();
        }

        public IActionResult CatAssignments(string subject, string num, string season, string year, string cat)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            ViewData["cat"] = cat;
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

        public IActionResult Submissions(string subject, string num, string season, string year, string cat, string aname)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            ViewData["cat"] = cat;
            ViewData["aname"] = aname;
            return View();
        }

        public IActionResult Grade(string subject, string num, string season, string year, string cat, string aname, string uid)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            ViewData["cat"] = cat;
            ViewData["aname"] = aname;
            ViewData["uid"] = uid;
            return View();
        }

        /*******Begin code to modify********/


        /// <summary>
        /// Returns a JSON array of all the students in a class.
        /// Each object in the array should have the following fields:
        /// "fname" - first name
        /// "lname" - last name
        /// "uid" - user ID
        /// "dob" - date of birth
        /// "grade" - the student's grade in this class
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetStudentsInClass(string subject, int num, string season, int year)
        {
            var classID = getClassID(subject, num, season, year);

            var query =
                from e in db.EnrollmentGrades
                join s in db.Students on e.UId equals s.UId
                where classID == e.CId
                select new
                {
                    fname = s.FirstName,
                    lname = s.LastName,
                    uid = s.UId,
                    dob = s.UIdNavigation.Dob,
                    grade = e.Grade
                };

            return Json(query.ToArray());
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
        /// Returns a JSON array with all the assignments in an assignment category for a class.
        /// If the "category" parameter is null, return all assignments in the class.
        /// Each object in the array should have the following fields:
        /// "aname" - The assignment name
        /// "cname" - The assignment category name.
        /// "due" - The due DateTime
        /// "submissions" - The number of submissions to the assignment
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class, 
        /// or null to return assignments from all categories</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetAssignmentsInCategory(string subject, int num, string season, int year, string category)
        {
            var classID = getClassID(subject, num, season, year);

            var query =
                from ac in db.AssignmentCategories
                join a in db.Assignments on ac.CId equals a.CId
                where classID == ac.CId
                select new { ac, a };

            if (category == null)
            {
                var query1 =
                    from q in query
                    select new
                    {
                        aname = q.a.Name,
                        cname = q.ac.Name,
                        due = q.a.Due,
                        submissions = (from s in db.Submissions where s.AId == q.a.AId select s).Count()
                    };
                return Json(query1.ToArray());
            }
            else
            {
                var query1 =
                    from q in query
                    where q.ac.Name == category
                    select new
                    {
                        aname = q.a.Name,
                        cname = q.ac.Name,
                        due = q.a.Due,
                        submissions = (from s in db.Submissions where s.AId == q.a.AId select s).Count()
                    };
                return Json(query1.ToArray());
            }
        }


        /// <summary>
        /// Returns a JSON array of the assignment categories for a certain class.
        /// Each object in the array should have the folling fields:
        /// "name" - The category name
        /// "weight" - The category weight
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetAssignmentCategories(string subject, int num, string season, int year)
        {
            var classID = getClassID(subject, num, season, year);

            var query =
                from ac in db.AssignmentCategories
                where classID == ac.CId
                select new { name = ac.Name, weight = ac.GradeWeight };

            return Json(query.ToArray());
        }

        /// <summary>
        /// Creates a new assignment category for the specified class.
        /// If a category of the given class with the given name already exists, return success = false.
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The new category name</param>
        /// <param name="catweight">The new category weight</param>
        /// <returns>A JSON object containing {success = true/false} </returns>
        public IActionResult CreateAssignmentCategory(string subject, int num, string season, int year, string category, int catweight)
        {
            var classID = getClassID(subject, num, season, year);

            // To see if the category of the given class already exists
            var query1 =
                from ac in db.AssignmentCategories
                where ac.CId == classID && ac.Name == category
                select ac;
            if (query1.Count() > 0)
                return Json(new { success = false });

            AssignmentCategory a = new AssignmentCategory();
            a.CId = classID;
            a.GradeWeight = catweight;
            a.Name = category;
            db.AssignmentCategories.Add(a);
            db.SaveChanges();

            return Json(new { success = true });
        }

        /// <summary>
        /// Creates a new assignment for the given class and category.
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The new assignment name</param>
        /// <param name="asgpoints">The max point value for the new assignment</param>
        /// <param name="asgdue">The due DateTime for the new assignment</param>
        /// <param name="asgcontents">The contents of the new assignment</param>
        /// <returns>A JSON object containing success = true/false</returns>
        public IActionResult CreateAssignment(string subject, int num, string season, int year, string category, string asgname, int asgpoints, DateTime asgdue, string asgcontents)
        {
            var classID = getClassID(subject, num, season, year);

            // To get the categoryID and create a new assignment
            var categoryID = (from ac in db.AssignmentCategories
                              where classID == ac.CId && category == ac.Name
                              select ac.Id).ToArray()[0];

            // If the assignment name already exists, then return false
            var count = (from a in db.Assignments
                         where categoryID == a.CId && asgname == a.Name
                         select a).Count();
            if (count > 0)
                return Json(new { success = false });

            Assignment new_asg = new Assignment();
            new_asg.CId = categoryID;
            new_asg.Name = asgname;
            new_asg.Points = asgpoints;
            new_asg.Due = asgdue;
            new_asg.Contents = asgcontents;
            db.Assignments.Add(new_asg);
            db.SaveChanges();

            // Get all the studentsID in this class
            var all_uids = (from e in db.EnrollmentGrades where classID == e.CId select e.UId).ToArray();

            foreach (var uid in all_uids)
                updateGrade(uid, classID);

            return Json(new { success = true });
        }

        public void updateGrade(string uid, int classID)
        {
            // Filter the categories of the class that has assignments in it
            var query =
                from ac in db.AssignmentCategories
                join a in db.Assignments on ac.Id equals a.CId
                where classID == ac.CId
                group ac by ac.Id into group1
                select new { categoryID = group1.First().Id, weight = (int)group1.First().GradeWeight };

            int totalWeight = query.Sum(p => p.weight);

            System.Diagnostics.Debug.WriteLine("weight " + totalWeight);


            double totalGrade = 0.0;
            foreach (var q in query)
            {
                var assignments = (from a in db.Assignments
                                   where a.CId == q.categoryID
                                   select a).ToArray();

                int totalMaxPoint = 0;
                int totalScore = 0;
                foreach (var a in assignments)
                {
                    totalMaxPoint += (int)a.Points;

                    var query1 =
                        from s in db.Submissions
                        where s.AId == a.AId && s.UId == uid
                        select s;
                    // The student hasn't submitted the assignment
                    if (query1.Count() == 0)
                    {
                        totalScore += 0;
                    }
                    else
                    {
                        totalScore += (int)query1.ToArray()[0].Score;
                    }

                }

                System.Diagnostics.Debug.WriteLine(q.weight.ToString() + " "
                     + totalScore.ToString() + " " + totalMaxPoint.ToString());

                totalGrade += q.weight * (double)totalScore / totalMaxPoint;
                System.Diagnostics.Debug.WriteLine("totalgrade " + totalGrade);

            }

            System.Diagnostics.Debug.WriteLine("totalgrade " + totalGrade);


            double normalizedGrade = totalGrade * 100 / totalWeight;



            var query2 = from e in db.EnrollmentGrades where classID == e.CId && uid == e.UId select e;

            query2.ToArray()[0].Grade = ScoreToGrade(normalizedGrade);

            db.SaveChanges();
        }
        private string ScoreToGrade(double score)
        {
            if (score >= 93)
                return "A";
            else if (score >= 90)
                return "A-";
            else if (score >= 87)
                return "B+";
            else if (score >= 83)
                return "B";
            else if (score >= 80)
                return "B-";
            else if (score >= 77)
                return "C+";
            else if (score >= 73)
                return "C";
            else if (score >= 70)
                return "C-";
            else if (score >= 67)
                return "D+";
            else if (score >= 63)
                return "D";
            else if (score >= 60)
                return "D-";
            else
                return "E";
        }
        /// <summary>
        /// Gets a JSON array of all the submissions to a certain assignment.
        /// Each object in the array should have the following fields:
        /// "fname" - first name
        /// "lname" - last name
        /// "uid" - user ID
        /// "time" - DateTime of the submission
        /// "score" - The score given to the submission
        /// 
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The name of the assignment</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetSubmissionsToAssignment(string subject, int num, string season, int year, string category, string asgname)
        {
            var classID = getClassID(subject, num, season, year);

            var query =
                from ac in db.AssignmentCategories
                join a in db.Assignments on ac.Id equals a.CId
                join s in db.Submissions on a.AId equals s.AId
                join st in db.Students on s.UId equals st.UId
                where category == ac.Name && asgname == a.Name
                select new
                {
                    fname = st.FirstName,
                    lname = st.LastName,
                    uid = st.UId,
                    //time = s.Time,
                    score = s.Score
                };

            return Json(query.ToArray());
        }


        /// <summary>
        /// Set the score of an assignment submission
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The name of the assignment</param>
        /// <param name="uid">The uid of the student who's submission is being graded</param>
        /// <param name="score">The new score for the submission</param>
        /// <returns>A JSON object containing success = true/false</returns>
        public IActionResult GradeSubmission(string subject, int num, string season, int year, string category, string asgname, string uid, int score)
        {
            return Json(new { success = false });
        }


        /// <summary>
        /// Returns a JSON array of the classes taught by the specified professor
        /// Each object in the array should have the following fields:
        /// "subject" - The subject abbreviation of the class (such as "CS")
        /// "number" - The course number (such as 5530)
        /// "name" - The course name
        /// "season" - The season part of the semester in which the class is taught
        /// "year" - The year part of the semester in which the class is taught
        /// </summary>
        /// <param name="uid">The professor's uid</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetMyClasses(string uid)
        {            
            return Json(null);
        }


        
        /*******End code to modify********/
    }
}

