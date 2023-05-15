using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using indvidual.DTOs;
using indvidual.Models;
using indvidual.ViewModels;

namespace Second.ViewModels;

public class SubReportViewModel : Base, INotifyPropertyChanged
{

    private SubReportDto? _subReport = new SubReportDto();
    private ObservableCollection<SubReportDto> _subRecord = new ObservableCollection<SubReportDto>();
    private SubReportDto? _selectedSubReport = null;
    public AsyncRelayCommand AddSubReportAsyncCommand { get; private set; }
    private EmployeeViewModel EmployeeViewModel;
    public SubReportViewModel(EmployeeViewModel employeeViewModel)
    {
        EmployeeViewModel = employeeViewModel;
        AddSubReportAsyncCommand = new AsyncRelayCommand(AddSubReportAsync);
    }
    public SubReportDto? SelectedSubReport
    {
        get => _selectedSubReport;
        set
        {
            _selectedSubReport = value;
            OnPropertyChanged(nameof(SelectedSubReport));
        }
    }
    public ObservableCollection<SubReportDto> SubReports
    {
        get => _subRecord;
        set
        {
            _subRecord = value;
            OnPropertyChanged(nameof(SubReports));
        }
    }
    public async Task AddSubReportAsync()
    {
        using (var context = ContextFactory.CreateDbContext(Array.Empty<string>()))
        {
            this.SubReport.EmployeeId = EmployeeViewModel.SelectedEmployee.Id;
            this.SubReport.RecievedDate = this.SubReport.RecievedDate.ToUniversalTime();
             await context.SubReports.AddAsync(Mapper.Map<SubReport>(SubReport));
            await context.SaveChangesAsync();
            Load();
        }
    }

    public  void Load()
    {
        using (var context = ContextFactory.CreateDbContext(Array.Empty<string>()))
        {
            var subReports =  context.SubReports.ToList();
            SubReports = new ObservableCollection<SubReportDto>(Mapper.Map<List<SubReportDto>>(subReports));
        }
    }
    public SubReportDto? SubReport
    {
        get => _subReport;
        set
        {
            _subReport = value;
            OnPropertyChanged(nameof(SubReport));
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