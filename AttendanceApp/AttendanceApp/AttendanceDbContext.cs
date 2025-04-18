using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace AttendanceApp;

public partial class AttendanceDbContext : DbContext
{
    public AttendanceDbContext()
    {
    }

    public AttendanceDbContext(DbContextOptions<AttendanceDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Discipline> Disciplines { get; set; }

    public virtual DbSet<DisciplineType> DisciplineTypes { get; set; }

    public virtual DbSet<Group> Groups { get; set; }

    public virtual DbSet<Person> People { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Schedule> Schedules { get; set; }

    public virtual DbSet<Specialization> Specializations { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    public virtual DbSet<StudyPlan> StudyPlans { get; set; }

    public virtual DbSet<Teacher> Teachers { get; set; }

    public virtual DbSet<Truancy> Truancies { get; set; }

    public virtual DbSet<TruancyReason> TruancyReasons { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Discipline>(entity =>
        {
            entity.HasKey(e => e.IdDiscipline);

            entity.ToTable("Discipline");

            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.NameId).HasMaxLength(50);

            entity.HasOne(d => d.IdDisciplineTypeNavigation).WithMany(p => p.Disciplines)
                .HasForeignKey(d => d.IdDisciplineType)
                .HasConstraintName("FK_Discipline_DisciplineType");
        });

        modelBuilder.Entity<DisciplineType>(entity =>
        {
            entity.HasKey(e => e.IdDisciplineType);

            entity.ToTable("DisciplineType");

            entity.Property(e => e.Name).HasMaxLength(70);
            entity.Property(e => e.NameId).HasMaxLength(50);
        });

        modelBuilder.Entity<Group>(entity =>
        {
            entity.HasKey(e => e.IdGroup);

            entity.ToTable("Group");

            entity.Property(e => e.Name).HasMaxLength(50);

            entity.HasOne(d => d.IdElderNavigation).WithMany(p => p.Groups)
                .HasForeignKey(d => d.IdElder)
                .HasConstraintName("FK_Group_Student");

            entity.HasOne(d => d.IdSpecNavigation).WithMany(p => p.Groups)
                .HasForeignKey(d => d.IdSpec)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Group_Specialization");

            entity.HasOne(d => d.IdTeacherCuratorNavigation).WithMany(p => p.Groups)
                .HasForeignKey(d => d.IdTeacherCurator)
                .HasConstraintName("FK_Group_Teacher");
        });

        modelBuilder.Entity<Person>(entity =>
        {
            entity.HasKey(e => e.IdPerson);

            entity.ToTable("Person");

            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.MiddleName).HasMaxLength(50);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.IdRole);

            entity.ToTable("Role");

            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<Schedule>(entity =>
        {
            entity.HasKey(e => e.IdSchedule);

            entity.ToTable("Schedule");

            entity.HasOne(d => d.IdDisciplineNavigation).WithMany(p => p.Schedules)
                .HasForeignKey(d => d.IdDiscipline)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Schedule_Discipline");

            entity.HasOne(d => d.IdGroupNavigation).WithMany(p => p.Schedules)
                .HasForeignKey(d => d.IdGroup)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Schedule_Group");

            entity.HasOne(d => d.IdTeacherNavigation).WithMany(p => p.Schedules)
                .HasForeignKey(d => d.IdTeacher)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Schedule_Teacher");
        });

        modelBuilder.Entity<Specialization>(entity =>
        {
            entity.HasKey(e => e.IdSpec).HasName("PK_Speciality");

            entity.ToTable("Specialization");

            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.NameId).HasMaxLength(50);
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.IdStudent);

            entity.ToTable("Student");

            entity.HasOne(d => d.IdGroupNavigation).WithMany(p => p.Students)
                .HasForeignKey(d => d.IdGroup)
                .HasConstraintName("FK_Student_Group");

            entity.HasOne(d => d.IdPersonNavigation).WithMany(p => p.Students)
                .HasForeignKey(d => d.IdPerson)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Student_Person");
        });

        modelBuilder.Entity<StudyPlan>(entity =>
        {
            entity.HasKey(e => e.IdStudyPlan);

            entity.ToTable("StudyPlan");

            entity.HasOne(d => d.IdDisciplineNavigation).WithMany(p => p.StudyPlans)
                .HasForeignKey(d => d.IdDiscipline)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_StudyPlan_Discipline");

            entity.HasOne(d => d.IdSpecNavigation).WithMany(p => p.StudyPlans)
                .HasForeignKey(d => d.IdSpec)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_StudyPlan_Speciality");
        });

        modelBuilder.Entity<Teacher>(entity =>
        {
            entity.HasKey(e => e.IdTeacher);

            entity.ToTable("Teacher");

            entity.HasOne(d => d.IdPersonNavigation).WithMany(p => p.Teachers)
                .HasForeignKey(d => d.IdPerson)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Teacher_Person");
        });

        modelBuilder.Entity<Truancy>(entity =>
        {
            entity.HasKey(e => e.IdTruancy);

            entity.ToTable("Truancy");

            entity.HasOne(d => d.IdReasonNavigation).WithMany(p => p.Truancies)
                .HasForeignKey(d => d.IdReason)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Truancy_TruancyReason");

            entity.HasOne(d => d.IdScheduleNavigation).WithMany(p => p.Truancies)
                .HasForeignKey(d => d.IdSchedule)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Truancy_Schedule");

            entity.HasOne(d => d.IdStudentNavigation).WithMany(p => p.Truancies)
                .HasForeignKey(d => d.IdStudent)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Truancy_Student");
        });

        modelBuilder.Entity<TruancyReason>(entity =>
        {
            entity.HasKey(e => e.IdReason);

            entity.ToTable("TruancyReason");

            entity.Property(e => e.IdReason).ValueGeneratedNever();
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.IdUser);

            entity.ToTable("User");

            entity.Property(e => e.Login)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Password)
                .HasMaxLength(64)
                .IsUnicode(false)
                .IsFixedLength();

            entity.HasOne(d => d.IdPersonNavigation).WithMany(p => p.Users)
                .HasForeignKey(d => d.IdPerson)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_User_Person");

            entity.HasOne(d => d.IdRoleNavigation).WithMany(p => p.Users)
                .HasForeignKey(d => d.IdRole)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_User_Role");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
