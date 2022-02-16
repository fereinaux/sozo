using Arquitetura.ViewModels;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Core.Business.Arquivos;
using Core.Business.ContaBancaria;
using Core.Business.Equipantes;
using Core.Business.Equipes;
using Core.Business.Eventos;
using Core.Business.Lancamento;
using Core.Business.MeioPagamento;
using Core.Business.Reunioes;
using Core.Models.Equipantes;
using Core.Models.Eventos;
using Core.Models.Lancamento;
using Core.Models.Participantes;
using Core.Models.Quartos;
using Data.Context;
using Data.Entities;
using SysIgreja.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Utils.Constants;
using Utils.Enums;
using Utils.Extensions;
using Utils.Services;

namespace SysIgreja.Controllers
{

    public class MapperRealidade
    {

        public IMapper mapper;

        public MapperRealidade(int? qtdReunioes = null, int? eventoId = null)
        {
            var configuration = new MapperConfiguration(cfg =>
            {                
                cfg.CreateMap<Equipante, PostEquipanteModel>();
                cfg.CreateMap<ApplicationUser, UsuarioViewModel>();
                cfg.CreateMap<Quarto, PostQuartoModel>();
                cfg.CreateMap<Quarto, PostQuartoModel>();
                cfg.CreateMap<Evento, PostEventoModel>();
                cfg.CreateMap<Participante, ParticipanteSelectModel>().ForMember(dest => dest.Nome, opt => opt.MapFrom(x => UtilServices.CapitalizarNome(x.Nome)));
                cfg.CreateMap<Participante, ParticipanteListModel>()
                    .ForMember(dest => dest.Idade, opt => opt.MapFrom(x => UtilServices.GetAge(x.DataNascimento)))
                    .ForMember(dest => dest.QtdAnexos, opt => opt.MapFrom(x => x.Arquivos.Count()))
                    .ForMember(dest => dest.Sexo, opt => opt.MapFrom(x => x.Sexo.GetDescription()))
                    .ForMember(dest => dest.Padrinho, opt => opt.MapFrom(x => x.PadrinhoId.HasValue ? x.Padrinho.Nome : null))
                    .ForMember(dest => dest.HasContact, opt => opt.MapFrom(x => x.MsgGeral || x.MsgVacina || x.MsgPagamento || !string.IsNullOrEmpty(x.Observacao)))
                    .ForMember(dest => dest.Status, opt => opt.MapFrom(x => x.Status.GetDescription()));
                cfg.CreateMap<Equipante, EquipanteListModel>()
                    .ForMember(dest => dest.Idade, opt => opt.MapFrom(x => UtilServices.GetAge(x.DataNascimento)))
                            .ForMember(dest => dest.Sexo, opt => opt.MapFrom(x => x.Sexo.GetDescription()))
                            .ForMember(dest => dest.Sexo, opt => opt.MapFrom(x => x.Sexo.GetDescription()))
                            .ForMember(dest => dest.QtdAnexos, opt => opt.MapFrom(x => x.Arquivos.Count()))
                            .ForMember(dest => dest.HasOferta, opt => opt.MapFrom(x => x.Lancamentos.Any(y => y.EventoId == (eventoId ?? x.Equipes.LastOrDefault().EventoId))))
                            .ForMember(dest => dest.Faltas, opt => opt.MapFrom(x => qtdReunioes - x.Equipes.LastOrDefault().Presencas.Count()))
                            .ForMember(dest => dest.Status, opt => opt.MapFrom(x => x.Status.GetDescription()))
                            .ForMember(dest => dest.Equipe, opt => opt.MapFrom(x => x.Equipes.Any() ? x.Equipes.LastOrDefault().Equipe.GetDescription() : null));               
                cfg.CreateMap<Equipante, EquipanteExcelModel>()
                  .ForMember(dest => dest.Nome, opt => opt.MapFrom(x => UtilServices.CapitalizarNome(x.Nome)))
                 .ForMember(dest => dest.Apelido, opt => opt.MapFrom(x => UtilServices.CapitalizarNome(x.Apelido)))
                   .ForMember(dest => dest.Idade, opt => opt.MapFrom(x => UtilServices.GetAge(x.DataNascimento)))
                   .ForMember(dest => dest.Sexo, opt => opt.MapFrom(x => x.Sexo.GetDescription()))
                   .ForMember(dest => dest.Status, opt => opt.MapFrom(x => x.Status.GetDescription()))
                   .ForMember(dest => dest.HasVacina, opt => opt.MapFrom(x => x.HasVacina ? "Sim" : "Não"))
                   .ForMember(dest => dest.HasOferta, opt => opt.MapFrom(x => x.Lancamentos.Any(y => y.EventoId == (eventoId ?? x.Equipes.LastOrDefault().EventoId)) ? "Sim" : "Não"))
                   .ForMember(dest => dest.Equipe, opt => opt.MapFrom(x => x.Equipes.Any() ? x.Equipes.LastOrDefault().Equipe.GetDescription() : null));


            });
            mapper = configuration.CreateMapper();
        }
    }

}