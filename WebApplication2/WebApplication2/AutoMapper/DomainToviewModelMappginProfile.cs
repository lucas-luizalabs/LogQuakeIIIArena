using AutoMapper;
using LogQuake.API.ViewModels;
using LogQuake.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogQuake.API.AutoMapper
{
    /// <summary>
    /// Seleciona todos os valores cadastrados.
    /// </summary>
    public class DomainToviewModelMappginProfile : Profile
    {
        /// <summary>
        /// Seleciona todos os valores cadastrados.
        /// </summary>
        public override string ProfileName
        {
            get { return "DomainToViewModelMapping"; }
        }

        /// <summary>
        /// Seleciona todos os valores cadastrados.
        /// </summary>
        public DomainToviewModelMappginProfile()
        {

            //CreateMap<PlayerViewModel, Player>();
        }
    }
}
