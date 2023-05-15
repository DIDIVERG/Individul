using AutoMapper;
using indvidual.DTOs;
using indvidual.Models;


namespace Second;


    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Employee, EmployeeDto>().ReverseMap();
            CreateMap<Expense, ExpenseDto>().ReverseMap();
            CreateMap<ExpenseReport, ExpenseReportDto>().ReverseMap();
            CreateMap<SubReport, SubReportDto>().ReverseMap();
        }
    }
