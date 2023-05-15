using System;
using AutoMapper;
using indvidual;
using Microsoft.EntityFrameworkCore;

namespace Second.ViewModels;

public  class Base
{
    protected ApplicationContextFactory ContextFactory;
    protected Mapper Mapper;
    public Base()
    {
        ContextFactory = new ApplicationContextFactory();
        var config = new MapperConfiguration(cfg => {
            cfg.AddProfile<MappingProfile>();
        });
        Mapper = new Mapper(config);
    }
}