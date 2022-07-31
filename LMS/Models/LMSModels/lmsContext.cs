using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace LMS.Models.LMSModels
{
    public partial class LMSContext : DbContext
    {
        public LMSContext()
        {
        }

        public LMSContext(DbContextOptions<LMSContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Administrator> Administrators { get; set; } = null!;
        public virtual DbSet<Assignment> Assignments { get; set; } = null!;
        public virtual DbSet<AssignmentCategory> AssignmentCategories { get; set; } = null!;
        public virtual DbSet<Class> Classes { get; set; } = null!;
        public virtual DbSet<Course> Courses { get; set; } = null!;
        public virtual DbSet<Department> Departments { get; set; } = null!;
        public virtual DbSet<EnrollmentGrade> EnrollmentGrades { get; set; } = null!;
        public virtual DbSet<Professor> Professors { get; set; } = null!;
        public virtual DbSet<Student> Students { get; set; } = null!;
        public virtual DbSet<Submission> Submissions { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseMySql("database=Team18Library;uid=u1364466;password=newpassword;server=atr.eng.utah.edu", Microsoft.EntityFrameworkCore.ServerVersion.Parse("10.1.48-mariadb"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseCollation("latin1_swedish_ci")
                .HasCharSet("latin1");

            modelBuilder.Entity<Administrator>(entity =>
            {
                entity.HasKey(e => e.UId)
                    .HasName("PRIMARY");

                entity.Property(e => e.UId)
                    .HasMaxLength(8)
                    .HasColumnName("uID");

                entity.Property(e => e.FirstName).HasMaxLength(100);

                entity.Property(e => e.LastName).HasMaxLength(100);

                entity.Property(e => e.Role).HasMaxLength(100);

                entity.HasOne(d => d.UIdNavigation)
                    .WithOne(p => p.Administrator)
                    .HasForeignKey<Administrator>(d => d.UId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Administrators_ibfk_1");
            });

            modelBuilder.Entity<Assignment>(entity =>
            {
                entity.HasKey(e => e.AId)
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.CId, "cID");

                entity.Property(e => e.AId)
                    .HasColumnType("int(11)")
                    .ValueGeneratedNever()
                    .HasColumnName("aID");

                entity.Property(e => e.CId)
                    .HasColumnType("int(11)")
                    .HasColumnName("cID");

                entity.Property(e => e.Contents).HasColumnType("text");

                entity.Property(e => e.Due).HasColumnType("datetime");

                entity.Property(e => e.Name).HasMaxLength(100);

                entity.Property(e => e.Points).HasColumnType("int(11)");

                entity.HasOne(d => d.CIdNavigation)
                    .WithMany(p => p.Assignments)
                    .HasForeignKey(d => d.CId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Assignments_ibfk_2");
            });

            modelBuilder.Entity<AssignmentCategory>(entity =>
            {
                entity.HasIndex(e => e.CId, "cID");

                entity.HasIndex(e => new { e.Name, e.CId }, "identifier")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("ID");

                entity.Property(e => e.CId)
                    .HasColumnType("int(11)")
                    .HasColumnName("cID");

                entity.Property(e => e.GradeWeight).HasColumnType("int(11)");

                entity.Property(e => e.Name).HasMaxLength(100);

                entity.HasOne(d => d.CIdNavigation)
                    .WithMany(p => p.AssignmentCategories)
                    .HasForeignKey(d => d.CId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("AssignmentCategories_ibfk_1");
            });

            modelBuilder.Entity<Class>(entity =>
            {
                entity.HasKey(e => e.CId)
                    .HasName("PRIMARY");

                entity.HasIndex(e => new { e.CatalogId, e.CourseNum }, "CatalogID");

                entity.HasIndex(e => e.ProfessorId, "ProfessorID");

                entity.HasIndex(e => new { e.Semester, e.Year, e.CourseNum }, "Semester")
                    .IsUnique();

                entity.Property(e => e.CId)
                    .HasColumnType("int(11)")
                    .HasColumnName("cID");

                entity.Property(e => e.CatalogId)
                    .HasColumnType("int(5) unsigned zerofill")
                    .HasColumnName("CatalogID");

                entity.Property(e => e.CourseNum).HasColumnType("int(4) unsigned zerofill");

                entity.Property(e => e.Location).HasMaxLength(100);

                entity.Property(e => e.ProfessorId)
                    .HasMaxLength(8)
                    .HasColumnName("ProfessorID");

                entity.Property(e => e.Semester).HasColumnType("enum('Spring','Fall','Summer')");

                entity.Property(e => e.Year).HasColumnType("int(10) unsigned");

                entity.HasOne(d => d.Professor)
                    .WithMany(p => p.Classes)
                    .HasForeignKey(d => d.ProfessorId)
                    .HasConstraintName("Classes_ibfk_2");
            });

            modelBuilder.Entity<Course>(entity =>
            {
                entity.HasKey(e => new { e.CatalogId, e.CourseNum })
                    .HasName("PRIMARY")
                    .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

                entity.HasIndex(e => new { e.Subject, e.CourseNum }, "Subject")
                    .IsUnique();

                entity.Property(e => e.CatalogId)
                    .HasColumnType("int(5)")
                    .HasColumnName("CatalogID");

                entity.Property(e => e.CourseNum).HasColumnType("int(4) unsigned zerofill");

                entity.Property(e => e.Name).HasMaxLength(100);

                entity.Property(e => e.Subject).HasMaxLength(4);

                entity.HasOne(d => d.SubjectNavigation)
                    .WithMany(p => p.Courses)
                    .HasForeignKey(d => d.Subject)
                    .HasConstraintName("Courses_ibfk_1");
            });

            modelBuilder.Entity<Department>(entity =>
            {
                entity.HasKey(e => e.Subject)
                    .HasName("PRIMARY");

                entity.Property(e => e.Subject).HasMaxLength(4);

                entity.Property(e => e.Name).HasMaxLength(100);
            });

            modelBuilder.Entity<EnrollmentGrade>(entity =>
            {
                entity.HasKey(e => new { e.UId, e.CId })
                    .HasName("PRIMARY")
                    .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

                entity.HasIndex(e => e.CId, "cID");

                entity.Property(e => e.UId)
                    .HasMaxLength(8)
                    .HasColumnName("uID");

                entity.Property(e => e.CId)
                    .HasColumnType("int(11)")
                    .HasColumnName("cID");

                entity.Property(e => e.Grade).HasMaxLength(2);

                entity.HasOne(d => d.CIdNavigation)
                    .WithMany(p => p.EnrollmentGrades)
                    .HasForeignKey(d => d.CId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("EnrollmentGrades_ibfk_2");

                entity.HasOne(d => d.UIdNavigation)
                    .WithMany(p => p.EnrollmentGrades)
                    .HasForeignKey(d => d.UId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("EnrollmentGrades_ibfk_1");
            });

            modelBuilder.Entity<Professor>(entity =>
            {
                entity.HasKey(e => e.UId)
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.Department, "Department");

                entity.Property(e => e.UId)
                    .HasMaxLength(8)
                    .HasColumnName("uID");

                entity.Property(e => e.Department).HasMaxLength(4);

                entity.Property(e => e.FirstName).HasMaxLength(100);

                entity.Property(e => e.LastName).HasMaxLength(100);

                entity.HasOne(d => d.DepartmentNavigation)
                    .WithMany(p => p.Professors)
                    .HasForeignKey(d => d.Department)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Professors_ibfk_2");

                entity.HasOne(d => d.UIdNavigation)
                    .WithOne(p => p.Professor)
                    .HasForeignKey<Professor>(d => d.UId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Professors_ibfk_1");
            });

            modelBuilder.Entity<Student>(entity =>
            {
                entity.HasKey(e => e.UId)
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.Subject, "Subject");

                entity.Property(e => e.UId)
                    .HasMaxLength(8)
                    .HasColumnName("uID");

                entity.Property(e => e.FirstName).HasMaxLength(100);

                entity.Property(e => e.LastName).HasMaxLength(100);

                entity.Property(e => e.Subject).HasMaxLength(4);

                entity.HasOne(d => d.SubjectNavigation)
                    .WithMany(p => p.Students)
                    .HasForeignKey(d => d.Subject)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Students_ibfk_2");

                entity.HasOne(d => d.UIdNavigation)
                    .WithOne(p => p.Student)
                    .HasForeignKey<Student>(d => d.UId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Students_ibfk_1");
            });

            modelBuilder.Entity<Submission>(entity =>
            {
                entity.ToTable("Submission");

                entity.HasIndex(e => e.AId, "aID");

                entity.HasIndex(e => e.UId, "uID");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("ID");

                entity.Property(e => e.AId)
                    .HasColumnType("int(11)")
                    .HasColumnName("aID");

                entity.Property(e => e.Contents).HasColumnType("text");

                entity.Property(e => e.Score).HasColumnType("int(11)");

                entity.Property(e => e.UId)
                    .HasMaxLength(8)
                    .HasColumnName("uID");

                entity.HasOne(d => d.AIdNavigation)
                    .WithMany(p => p.Submissions)
                    .HasForeignKey(d => d.AId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Submission_ibfk_1");

                entity.HasOne(d => d.UIdNavigation)
                    .WithMany(p => p.Submissions)
                    .HasForeignKey(d => d.UId)
                    .HasConstraintName("Submission_ibfk_2");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UId)
                    .HasName("PRIMARY");

                entity.Property(e => e.UId)
                    .HasMaxLength(8)
                    .HasColumnName("uID");

                entity.Property(e => e.Dob).HasColumnName("DOB");

                entity.Property(e => e.FirstName).HasMaxLength(100);

                entity.Property(e => e.LastName).HasMaxLength(100);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
