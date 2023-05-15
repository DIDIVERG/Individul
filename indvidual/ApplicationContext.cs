using System.CodeDom;
using indvidual.Models;
using Microsoft.EntityFrameworkCore;

namespace indvidual;

public class ApplicationContext : DbContext
{


    public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
    {
            
    }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Expense> Expenses { get; set; }
    public DbSet<ExpenseReport> ExpenseReports { get; set; }
    public DbSet<SubReport> SubReports { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(50);

            entity.HasMany(e => e.SubReports)
                .WithOne(sr => sr.Employee)
                .HasForeignKey(sr => sr.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Expense>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Item).IsRequired().HasMaxLength(50);

            entity.HasOne(e => e.ExpenseReport)
                .WithMany(er => er.Expenses)
                .HasForeignKey(e => e.ExpenseReportId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ExpenseReport>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ReportDate).IsRequired();
            

            entity.HasOne(e => e.Employee)
                .WithMany(e => e.ExpenseReports)
                .HasForeignKey(e => e.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}