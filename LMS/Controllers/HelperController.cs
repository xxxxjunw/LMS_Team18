using System;
using LMS.Models.LMSModels;
namespace LMS.Controllers
{
    public class HelperController
    {
        public HelperController()
        {
        }

        public static int getClassID(string subject, int num, string season, int year, LMSContext db)
        {
            var query =
                from co in db.Courses
                join cl in db.Classes on co.CourseNum equals cl.CourseNum
                where subject == co.Subject && num == co.CourseNum
                    && season == cl.Semester && year == cl.Year
                select cl.CId;

            return query.ToArray()[0];
        }

        public static void updateGrade(string uid, int classID, LMSContext db)
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

        private static string ScoreToGrade(double score)
        {
            if (score >= 92)
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
    }
}

