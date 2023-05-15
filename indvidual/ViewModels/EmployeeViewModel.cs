using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using indvidual.DTOs;
using indvidual.Models;
using Microsoft.EntityFrameworkCore;
using Second.ViewModels;

namespace indvidual.ViewModels;

public class EmployeeViewModel : Base, INotifyPropertyChanged
{
    private ObservableCollection<EmployeeDto> _employee = new ObservableCollection<EmployeeDto>();
    private EmployeeDto? _selectedEmployee = null;

    public EmployeeViewModel()
    {
    }
    public EmployeeDto? SelectedEmployee
    {
        get => _selectedEmployee;
        set
        {
            _selectedEmployee = value;
            OnPropertyChanged(nameof(SelectedEmployee));
        }
    }
    public ObservableCollection<EmployeeDto> Employees
    {
        get => _employee;
        set
        {
            _employee = value;
            OnPropertyChanged(nameof(Employees));
        }
    }



    public  void Load()
    {
        using (var context = ContextFactory.CreateDbContext(Array.Empty<string>()))
        {
            var employees =  context.Employees.ToList();
            Employees = new ObservableCollection<EmployeeDto>(Mapper.Map<List<EmployeeDto>>(employees));
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