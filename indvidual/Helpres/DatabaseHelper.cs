using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Web;
using indvidual;
using indvidual.Models;
using Microsoft.EntityFrameworkCore;

namespace Second;

public  class DatabaseHelper
{
    private static void FillInitialData(ApplicationContext context)
    {
        var employees = new List<Employee>
        {
            new Employee { Name = "John Smith", Department = "Sales" },
            new Employee { Name = "Anna Johnson", Department = "Marketing" },
            new Employee { Name = "Peter Brown", Department = "IT" },
            new Employee { Name = "Lisa Davis", Department = "HR" },
            new Employee { Name = "Michael Wilson", Department = "Finance" }
        };

         context.Employees.AddRange(employees);
         context.SaveChanges();
    }
    
    public static  void EnsureCreatedAsync(ApplicationContext context)
    {
        if ( context.Database.EnsureCreated())
        {
             FillInitialData(context);
        }
    }

    public static  void RecreateDatabase(ApplicationContext context)
    {
        if ( context.Database.EnsureDeleted())
        {
            EnsureCreatedAsync(context);
        }
    }

    public static void DeleteDatabase(ApplicationContext context) => context.Database.EnsureDeleted();
}