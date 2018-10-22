using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogQuake.API.AutoMapper
{
    public class AutoMapperConfig
    {
        public static void RegisterMappgings()
        {
            Mapper.Initialize(x =>
            {
                x.AddProfile<DomainToviewModelMappginProfile>();
                x.AddProfile<ViewModelToDomainMappingProfile>();
            }
            );
        }
    }
}
