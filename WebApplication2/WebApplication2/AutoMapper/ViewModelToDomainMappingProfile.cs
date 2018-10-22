using AutoMapper;
using LogQuake.API.ViewModels;
using LogQuake.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogQuake.API.AutoMapper
{
    public class ViewModelToDomainMappingProfile : Profile
    {
        /// <summary>
        /// Seleciona todos os valores cadastrados.
        /// </summary>
        public override string ProfileName
        {
            get { return "ViewModelToDomainMapping"; }
        }

        /// <summary>
        /// Seleciona todos os valores cadastrados.
        /// </summary>
        public ViewModelToDomainMappingProfile()
        {

            CreateMap<Player, PlayerViewModel>();
        }
    }
}
