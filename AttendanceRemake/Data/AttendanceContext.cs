using System;
using System.Collections.Generic;
using AttendanceRemake.Models;
using Microsoft.EntityFrameworkCore;

namespace AttendanceRemake.Data;

public partial class AttendanceContext : DbContext
{
    public AttendanceContext()
    {
    }

    public AttendanceContext(DbContextOptions<AttendanceContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Attendance> Attendances { get; set; }

    public virtual DbSet<EmpAllow> EmpAllows { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<Floor> Floors { get; set; }

    public virtual DbSet<Location> Locations { get; set; }

    public virtual DbSet<Node> Nodes { get; set; }

    public virtual DbSet<TimingPlan> TimingPlans { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Password=12345;Persist Security Info=True;User ID=timeuser;Initial Catalog=TimeAttndProd;Data Source=FPSQL; TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("SQL_Latin1_General_CP1256_CI_AS");

        modelBuilder.Entity<Attendance>(entity =>
        {
            entity.HasKey(e => e.AttCode);

            entity.ToTable("Attendance");

            entity.HasIndex(e => new { e.IodateTime, e.TrType, e.FingerCode }, "IODateTimeTrTypeFingerCode_IX");

            entity.HasIndex(e => new { e.FingerCode, e.Status, e.TrType, e.IodateTime }, "PF_FCODE_STATUS_TYPE_IOTIME");

            entity.Property(e => e.IodateTime)
                .HasColumnType("datetime")
                .HasColumnName("IODateTime");
            entity.Property(e => e.NodeSerialNo).HasMaxLength(20);
            entity.Property(e => e.Photo).HasColumnType("image");
        });

        modelBuilder.Entity<EmpAllow>(entity =>
        {
            entity.HasKey(e => e.Serial);

            entity.ToTable("EmpAllow");

            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.RealStartDate).HasColumnType("datetime");
            entity.Property(e => e.StartDate).HasColumnType("datetime");

            entity.HasOne(d => d.EmpNoNavigation).WithMany(p => p.EmpAllows)
                .HasForeignKey(d => d.EmpNo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EmpAllow_Employee");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.EmpNo).HasName("PK_Employee_1");

            entity.ToTable("Employee");

            entity.HasIndex(e => e.FingerCode, "_dta_index_Employee_6_2110630562__K2");

            entity.Property(e => e.CheckLate).HasDefaultValue((short)1);
            entity.Property(e => e.HasPass).HasDefaultValue(false);
            entity.Property(e => e.NameA).HasMaxLength(100);
            entity.Property(e => e.NameE).HasMaxLength(100);

            entity.HasOne(d => d.TimingCodeNavigation).WithMany(p => p.Employees)
                .HasForeignKey(d => d.TimingCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Employee_TimingPlan");
        });

        modelBuilder.Entity<Floor>(entity =>
        {
            entity.HasKey(e => e.Floor1);         
            entity.ToTable("Floors");

            entity.Property(e => e.DescA).HasMaxLength(50);
            entity.Property(e => e.DescB).HasMaxLength(50);
            entity.Property(e => e.Floor1)
                .HasMaxLength(10)
                .HasColumnName("Floor");
        });

        modelBuilder.Entity<Location>(entity =>
        {
            entity.HasKey(e => e.Code);

            entity.ToTable("Location");

            entity.Property(e => e.Code).ValueGeneratedNever();
            entity.Property(e => e.DescA).HasMaxLength(50);
            entity.Property(e => e.DescE).HasMaxLength(50);
        });

        modelBuilder.Entity<Node>(entity =>
        {
            entity.HasKey(e => e.SerialNo);

            entity.Property(e => e.SerialNo).HasMaxLength(20);
            entity.Property(e => e.DescA).HasMaxLength(250);
            entity.Property(e => e.DescE).HasMaxLength(250);
            entity.Property(e => e.Floor).HasMaxLength(10);
        });

        modelBuilder.Entity<TimingPlan>(entity =>
        {
            entity.HasKey(e => e.Code);

            entity.ToTable("TimingPlan");

            entity.Property(e => e.Code).ValueGeneratedNever();
            entity.Property(e => e.Activity).HasDefaultValue(true);
            entity.Property(e => e.DescA).HasMaxLength(50);
            entity.Property(e => e.DescE).HasMaxLength(50);
            entity.Property(e => e.FromTime).HasMaxLength(5);
            entity.Property(e => e.RmdFromTime).HasMaxLength(5);
            entity.Property(e => e.RmdToTime).HasMaxLength(5);
            entity.Property(e => e.ToTime).HasMaxLength(5);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
