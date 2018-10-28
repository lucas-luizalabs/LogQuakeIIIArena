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
    /// Classe para efeutar a conversão de ViewModel para Domain
    /// </summary>
    public class ViewModelToDomainMappingProfile : Profile
    {
        /// <summary>
        /// Define um nome para o conversor
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

            //CreateMap<Player, PlayerViewModel>();
        }
    }
}
