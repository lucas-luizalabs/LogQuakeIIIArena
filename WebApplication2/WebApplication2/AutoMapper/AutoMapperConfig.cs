using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogQuake.API.AutoMapper
{
    /// <summary>
    /// Conversor de Domain x ViewModel e ViewModel x Domain
    /// </summary>
    public class AutoMapperConfig
    {
        /// <summary>
        /// Registrar converso
        /// </summary>
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
