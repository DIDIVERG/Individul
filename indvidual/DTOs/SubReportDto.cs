using System;

namespace indvidual.DTOs;

public class SubReportDto
{
    public int Id { get; set; }
    public Decimal Amount { get; set; }
    public string Purpose { get; set; }
    public DateTime RecievedDate { get; set; } = DateTime.Now;
    public int EmployeeId { get; set; }

}