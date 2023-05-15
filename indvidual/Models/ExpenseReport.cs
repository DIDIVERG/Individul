using System;
using System.Collections.Generic;

namespace indvidual.Models;

public class ExpenseReport
{
    public int Id { get; set; }
    public DateTime ReportDate { get; set; } = DateTime.UtcNow;
    public int EmployeeId { get; set; }
    public Employee Employee { get; set; }
    public  ICollection<Expense> Expenses { get; set; }
}