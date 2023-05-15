using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using indvidual.DTOs;
using indvidual.Models;
using Second.ViewModels;

namespace indvidual.ViewModels;

public class ExpenseViewModel:Base, INotifyPropertyChanged
{

    private ExpenseDto? _expense;
    private ExpenseDto? _selectedExpense;
    private int _employeeId;
    private ObservableCollection<ExpenseDto> _expenses = new ObservableCollection<ExpenseDto>();
    private ObservableCollection<ExpenseDto> _currentExpenses = new ObservableCollection<ExpenseDto>();
    
    private ObservableCollection<ExpenseDto> _selectedItems;

    public ObservableCollection<ExpenseDto> SelectedItems
    {
        get { return _selectedItems; }
        set
        {
            if (_selectedItems != value)
            {
                _selectedItems = value;
                OnPropertyChanged(nameof(SelectedItems));
            }
        }
    }
    public int EmployeeId
    {
        get { return _employeeId; }
        set
        {
            _employeeId = value;
            OnPropertyChanged(nameof(EmployeeId));
        }
    }
    private SubReportViewModel _subReportViewModel;
    public AsyncRelayCommand AddExpenseAsyncCommand { get; private set; }
    public AsyncRelayCommand DeleteExpenseAsyncCommand { get; private set; }
    public AsyncRelayCommand CreateExpenseReportAsyncCommand { get; private set; }
    public AsyncRelayCommand DeleteAsyncCommand { get; private set; }

    public ExpenseViewModel(SubReportViewModel subReportViewModel)
    {
        this._subReportViewModel = subReportViewModel;
        AddExpenseAsyncCommand = new AsyncRelayCommand(AddExpenseAsync);
        DeleteExpenseAsyncCommand = new AsyncRelayCommand(DeleteExpenseAsync);
        CreateExpenseReportAsyncCommand = new AsyncRelayCommand(CreateExpenseReportAsync);
        DeleteAsyncCommand = new AsyncRelayCommand(DeleteAsync);
        Expense = new ExpenseDto();
    }

    public  Task DeleteAsync()
    {
        if (SelectedExpense != null)
        {
            Expenses.Remove(SelectedExpense);
            CurrentExpenses.Remove(SelectedExpense);
        }
        return Task.CompletedTask;
    }
    public async Task CreateExpenseReportAsync()
    {
        using (var context = ContextFactory.CreateDbContext(Array.Empty<string>()))
        {
            var lastReport = context.ExpenseReports.OrderByDescending(er => er.Id)
                .FirstOrDefault() ?? new ExpenseReport();
            if (_subReportViewModel.SelectedSubReport is not null)
            {
                await context.ExpenseReports.AddAsync(new ExpenseReport()
                {
                
                    EmployeeId = _subReportViewModel.SelectedSubReport.EmployeeId ,
                    Id = lastReport.Id + 1,
                }); 
            }
            else
            {
                await context.ExpenseReports.AddAsync(new ExpenseReport()
                {
                    EmployeeId = this.EmployeeId ,
                    Id = lastReport.Id + 1,
                });
  
            }
           
            foreach (var item in CurrentExpenses)
            {
                item.ExpenseReportId = lastReport.Id + 1;
                if (_subReportViewModel.SelectedSubReport is null)
                {
                    item.Amount = item.Amount - item.Amount * (decimal)0.13;
                    item.IsTaxed = true;
                }
            }
            await context.Expenses.AddRangeAsync(Mapper.Map<List<Expense>>(CurrentExpenses));
            await context.SaveChangesAsync();
            CurrentExpenses.Clear();
            Load();
        }
    }
    public ObservableCollection<ExpenseDto> Expenses
    {
        get => _expenses;
        set
        {
            _expenses = value;
            OnPropertyChanged(nameof(Expenses));
        }
    }
    public ObservableCollection<ExpenseDto> CurrentExpenses
    {
        get => _currentExpenses;
        set
        {
            _currentExpenses = value;
            OnPropertyChanged(nameof(CurrentExpenses));
        }
    }
    
    public  Task AddExpenseAsync()
    {
        var max = Expenses.MaxBy(e => e.Id);
        var expenseTemp = new ExpenseDto()
        {
            Id = max is null ? 1 : max.Id + 1,
            Quantity = this.Expense.Quantity,
            Amount = this.Expense.Amount,
            Item = this.Expense.Item,
        };
        Expenses.Add(expenseTemp);
        CurrentExpenses.Add(expenseTemp);
        return Task.CompletedTask;
    }

    public async Task DeleteExpenseAsync()
    {
        if (SelectedExpense is not null)
        {
            _expenses.Remove(SelectedExpense);
        }
    }

    public ExpenseDto? Expense
    {
        get => _expense;
        set
        {
            _expense = value;
            OnPropertyChanged(nameof(_expense));
        }
    } 
    
    public ExpenseDto? SelectedExpense
    {
        get => _selectedExpense;
        set
        {
            _selectedExpense = value;
            OnPropertyChanged(nameof(SelectedExpense));
        }
    }

    public void Load()
    {
        using (var context = ContextFactory.CreateDbContext(Array.Empty<string>()))
        {
            var expenses =  context.Expenses.ToList();
            Expenses = new ObservableCollection<ExpenseDto>(Mapper.Map<List<ExpenseDto>>(expenses));
        }
    }
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}