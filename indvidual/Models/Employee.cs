using System.Collections.Generic;

namespace indvidual.Models;

public class Employee
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Department { get; set; }
    public  ICollection<ExpenseReport> ExpenseReports { get; set; }
    public  ICollection<SubReport> SubReports { get; set; }
}