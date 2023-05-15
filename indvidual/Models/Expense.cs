namespace indvidual.Models;

public class Expense
{
    public int Id { get; set; }
    public string Item { get; set; }
    public int Quantity { get; set; }
    public decimal Amount { get; set; }
    public bool IsTaxed { get; set; } = false;

    public int ExpenseReportId { get; set; }
    public ExpenseReport ExpenseReport { get; set; }
}