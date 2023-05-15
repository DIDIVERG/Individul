using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using indvidual.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using OfficeOpenXml;

namespace indvidual
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ApplicationContextFactory contextFactory;
        public MainWindow()
        {
            InitializeComponent();
            contextFactory = new ApplicationContextFactory();
        }

        
        private void GenerateReport_Click(object sender, RoutedEventArgs e)
        {
            DateTime startDate = DatePicker.SelectedDate ?? DateTime.MinValue;
            DateTime endDate = DatePicker2.SelectedDate ?? DateTime.MaxValue;
            DateTime utcDateFrom = startDate.ToUniversalTime();
            DateTime utcDateBefore = endDate.ToUniversalTime();
            using (var context = contextFactory.CreateDbContext(Array.Empty<string>()))
            {
                var reportData = context.Expenses
                    .Include(e => e.ExpenseReport)
                    .ThenInclude(r => r.Employee)
                    .Where(e => e.ExpenseReport.ReportDate >= utcDateFrom && e.ExpenseReport.ReportDate <= utcDateBefore)
                    .GroupBy(e => e.ExpenseReport.Employee)
                    .Select(g => new
                    {
                        EmployeeId = g.Key.Id,
                        EmployeeName = g.Key.Name,
                        Department = g.Key.Department,
                        Amount = g.First().IsTaxed ? g.Sum(e => e.Amount) / (decimal)0.87 : g.Sum(e => e.Amount),
                        TaxSum =  (g.First().IsTaxed ? g.Sum(e => e.Amount) / (decimal)0.87 : g.Sum(e => e.Amount)) - g.Sum(e => e.Amount) ,
                    })
                    .ToList<dynamic>();

                SaveReportToExcel(reportData);
            }
            
        }
        private void SaveReportToExcel(List<dynamic> reportData)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Excel files (*.xlsx)|*.xlsx";
            saveFileDialog.Title = "Save Report";
            if (saveFileDialog.ShowDialog() == true)
            {
                using (var package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("Report");
                    worksheet.Cells[1, 1].Value = "Employee Id";
                    worksheet.Cells[1, 2].Value = "Employee Name";
                    worksheet.Cells[1, 3].Value = "Department";
                    worksheet.Cells[1, 4].Value = "Amount";
                    worksheet.Cells[1, 5].Value = "Tax Amount";

                    int row = 2;
                    foreach (var data in reportData)
                    {
                        worksheet.Cells[row, 1].Value = data.EmployeeId;
                        worksheet.Cells[row, 2].Value = data.EmployeeName;
                        worksheet.Cells[row, 3].Value = data.Department;
                        worksheet.Cells[row, 4].Value = data.Amount;
                        worksheet.Cells[row, 5].Value = data.TaxSum;
                        row++;
                    }

                    worksheet.Cells.AutoFitColumns();

                    var filePath = saveFileDialog.FileName;
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        package.SaveAs(fileStream);
                    }
                }

                MessageBox.Show("Report saved successfully.", "Report", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        private async void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var articles =  DataGridArticles.SelectedItems.Cast<ExpenseDto>();
            DateTime selectedDateFrom = DatePicker.SelectedDate ?? DateTime.Now;
            DateTime selectedDateBefore = DatePicker2.SelectedDate ?? DateTime.Now;
            DateTime utcDateFrom = selectedDateFrom.ToUniversalTime();
            DateTime utcDateBefore = selectedDateBefore.ToUniversalTime();
            using (var context = contextFactory.CreateDbContext(Array.Empty<string>()))
            {
                var articleItems = articles.Select(a => a.Item).ToList();

                var expensesByEmployee = await context.ExpenseReports
                    .Include(r => r.Expenses)
                    .Where(r => r.Expenses.Any(e => articleItems.Contains(e.Item)))
                    .SelectMany(r => r.Expenses)
                    .Where(e => articleItems.Contains(e.Item))
                    .GroupBy(e => e.ExpenseReport.EmployeeId)
                    .Select(g => new
                    {
                        EmployeeId = g.Key,
                        TotalAmount = g.Sum(e => e.Amount)
                    })
                    .ToListAsync();

                var employeeIds = expensesByEmployee.Select(e => e.EmployeeId).ToList();

                var employees = await context.Employees
                    .Where(emp => employeeIds.Contains(emp.Id))
                    .ToListAsync();

                var reportData = employees
                    .Join(expensesByEmployee.AsEnumerable(),
                        emp => emp.Id,
                        e => e.EmployeeId,
                        (emp, e) => new
                        {
                            EmployeeName = emp.Name,
                            e.TotalAmount
                        })
                    .ToList();
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using var package = new ExcelPackage();

// Add a new worksheet to the workbook
                var worksheet = package.Workbook.Worksheets.Add("Report");

// Set the column headers
                worksheet.Cells[1, 1].Value = "Employee Name";
                worksheet.Cells[1, 2].Value = "Total Amount";

// Fill the data in the worksheet
                int row = 2;
                foreach (var data in reportData)
                {
                    worksheet.Cells[row, 1].Value = data.EmployeeName;
                    worksheet.Cells[row, 2].Value = data.TotalAmount;
                    row++;
                }

// Auto-fit the columns for better readability
                worksheet.Cells.AutoFitColumns();

// Prompt the user to select the destination for saving the file
                var saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Excel files (*.xlsx)|*.xlsx";
                saveFileDialog.Title = "Save Report";
                if (saveFileDialog.ShowDialog() == true)
                {
                    // Save the Excel file
                    var filePath = saveFileDialog.FileName;
                    using var fileStream = new FileStream(filePath, FileMode.Create);
                    package.SaveAs(fileStream);
                }
            }
        }
    }
}