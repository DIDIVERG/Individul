using System;
using System.Collections.Generic;

namespace indvidual.Models;

public class SubReport
{
    public int Id { get; set; }
    public Decimal Amount { get; set; }
    public string Purpose { get; set; }
    public DateTime RecievedDate { get; set; }
    
    public int EmployeeId { get; set; }
    public Employee Employee { get; set; }
}