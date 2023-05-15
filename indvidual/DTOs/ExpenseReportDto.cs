using System;
using System.Collections.Generic;

namespace indvidual.DTOs;

public class ExpenseReportDto
{
    public int Id { get; set; }
    public DateTime ReportDate { get; set; } = DateTime.UtcNow;
    public int EmployeeId { get; set; }
    public ICollection<ExpenseDto> Expenses { get; set; }
}