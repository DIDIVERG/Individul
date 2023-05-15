using System;
using System.Runtime.InteropServices.JavaScript;
using System.Security.Cryptography;
using System.Threading.Tasks;
using indvidual;
using indvidual.Models;
using indvidual.ViewModels;

namespace Second.ViewModels;

public class MainViewModel : Base
{
    public ReportViewModel ReportViewModel { get; set; }
    public EmployeeViewModel EmployeeViewModel { get; set; }
    public SubReportViewModel SubReportViewModel { get; set; }
    public ExpenseViewModel ExpenseViewModel { get; set; }
    public MainViewModel()
    {
        DatabaseHelper.EnsureCreatedAsync(ContextFactory.CreateDbContext(Array.Empty<string>()));
        /*
        DatabaseHelper.DeleteDatabase(ContextFactory.CreateDbContext(Array.Empty<string>()));
        */
        ReportViewModel = new ReportViewModel();
        EmployeeViewModel = new EmployeeViewModel();
        SubReportViewModel = new SubReportViewModel(this.EmployeeViewModel);
        ExpenseViewModel = new ExpenseViewModel(SubReportViewModel);
        Load();
    }

  
    private void Load()
    {
         EmployeeViewModel.Load();
         SubReportViewModel.Load();
         ExpenseViewModel.Load();
    }
}