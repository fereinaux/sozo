using Arquitetura.ViewModels;
using AutoMapper;
using Core.Business.Arquivos;
using Core.Business.ContaBancaria;
using Core.Business.Equipantes;
using Core.Business.Equipes;
using Core.Business.Eventos;
using Core.Business.Lancamento;
using Core.Business.MeioPagamento;
using Core.Business.Quartos;
using Core.Business.Reunioes;
using Core.Models.Equipantes;
using Core.Models.Lancamento;
using Data.Entities;
using SysIgreja.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Dynamic;
using System.Web.Mvc;
using Utils.Constants;
using Utils.Enums;
using Utils.Extensions;
using Utils.Services;

namespace SysIgreja.Controllers
{

    public class EquipanteController : Controller
    {
        private readonly IEquipantesBusiness equipantesBusiness;
        private readonly IQuartosBusiness quartosBusiness;
        private readonly IArquivosBusiness arquivoBusiness;
        private readonly IEventosBusiness eventosBusiness;
        private readonly IEquipesBusiness equipesBusiness;
        private readonly IReunioesBusiness reunioesBusiness;
        private readonly ILancamentoBusiness lancamentoBusiness;
        private readonly IMeioPagamentoBusiness meioPagamentoBusiness;
        private readonly IContaBancariaBusiness contaBancariaBusiness;
        private readonly IDatatableService datatableService;
        private readonly IMapper mapper;
        private readonly int qtdReunioes;


        public EquipanteController(IEquipantesBusiness equipantesBusiness, IQuartosBusiness quartosBusiness, IDatatableService datatableService, IEventosBusiness eventosBusiness, IEquipesBusiness equipesBusiness, ILancamentoBusiness lancamentoBusiness, IReunioesBusiness reunioesBusiness, IMeioPagamentoBusiness meioPagamentoBusiness, IContaBancariaBusiness contaBancariaBusiness, IArquivosBusiness arquivoBusiness)
        {
            this.equipantesBusiness = equipantesBusiness;
            this.quartosBusiness = quartosBusiness;
            this.eventosBusiness = eventosBusiness;
            this.equipesBusiness = equipesBusiness;
            this.arquivoBusiness = arquivoBusiness;
            this.lancamentoBusiness = lancamentoBusiness;
            this.contaBancariaBusiness = contaBancariaBusiness;
            this.meioPagamentoBusiness = meioPagamentoBusiness;
            this.reunioesBusiness = reunioesBusiness;
            this.datatableService = datatableService;
            qtdReunioes = reunioesBusiness.GetReunioes(eventosBusiness.GetEventoAtivo().Id).Where(x => x.DataReuniao < System.DateTime.Today).Count();
            mapper = new MapperRealidade(qtdReunioes).mapper;
        }



        public ActionResult Index()
        {
            ViewBag.Title = "Equipantes";
            ViewBag.Eventos = eventosBusiness
               .GetEventos()
               .OrderByDescending(x => x.DataEvento)
               .ToList()
               .Select(x => new EventoViewModel
               {
                   Id = x.Id,
                   DataEvento = x.DataEvento,
                   Numeracao = x.Numeracao,
                   TipoEvento = x.TipoEvento.GetNickname(),
                   Status = x.Status.GetDescription()
               });
            ViewBag.MeioPagamentos = meioPagamentoBusiness.GetAllMeioPagamentos().ToList();
            ViewBag.Valor = (int)ValoresPadraoEnum.TaxaEquipante;
            ViewBag.ContasBancarias = contaBancariaBusiness.GetContasBancarias().ToList()
                .Select(x => new ContaBancariaViewModel
                {
                    Banco = x.Banco.GetDescription(),
                    Id = x.Id
                });

            return View();
        }

        [HttpPost]
        public ActionResult getEquipantesExcel(int eventoid)
        {
            Guid g = Guid.NewGuid();

            Session[g.ToString()] = datatableService.GenerateExcel(mapper.Map<IEnumerable<EquipanteExcelModel>>(equipesBusiness.GetEquipantesByEvento(eventoid).OrderBy(x => x.Nome)));

            return Content(g.ToString());
        }

        [HttpPost]
        public ActionResult GetEquipantesDataTable(FilterModel model)
        {
            var extract = Request.QueryString["extract"];
            if (extract == "excel")
            {
                Guid g = Guid.NewGuid();

                Session[g.ToString()] = datatableService.GenerateExcel(mapper.Map<IEnumerable<EquipanteExcelModel>>(equipantesBusiness.GetEquipantes()));

                return Content(g.ToString());
            }
            else
            {
                var eventoId = eventosBusiness.GetEventoAtivo().Id;
                var result = equipantesBusiness.GetEquipantes();

                var totalResultsCount = result.Count();
                var filteredResultsCount = totalResultsCount;

                if (model.Equipe != null)
                {
                    result = result.Where(x => x.Equipes.Any() && x.Equipes.OrderByDescending(z => z.EventoId).FirstOrDefault().Equipe == model.Equipe);
                }

                if (model.search.value != null)
                {
                    result = result.Where(x => (x.Nome.Contains(model.search.value)));
                    filteredResultsCount = result.Count();
                }

                try
                {
                    if (model.columns[model.order[0].column].name == "HasOferta")
                    {
                        if (model.order[0].dir == "asc")
                        {
                            result = result.OrderBy(x => new
                            {
                                Order = x.Lancamentos.Where(y => y.EventoId == eventoId).Any()
                            });

                        }
                        else
                        {
                            result = result.OrderByDescending(x => new
                            {
                                Order = x.Lancamentos.Where(y => y.EventoId == eventoId).Any()
                            });
                        }

                    } else if (model.columns[model.order[0].column].name == "Faltas")
                    {
                        if (model.order[0].dir == "asc")
                        {
                            result = result.OrderBy(x => new
                            {
                                Order = qtdReunioes - x.Equipes.OrderByDescending(z => z.EventoId).FirstOrDefault().Presencas.Count()
                            });

                        }
                        else
                        {
                            result = result.OrderByDescending(x => new
                            {
                                Order = qtdReunioes - x.Equipes.OrderByDescending(z => z.EventoId).FirstOrDefault().Presencas.Count()
                            });
                        }

                    }
                    else if (model.columns[model.order[0].column].name == "Equipe")
                    {
                        if (model.order[0].dir == "asc")
                        {
                            result = result.OrderBy(x => new
                            {
                                Order = x.Equipes.OrderByDescending(z => z.EventoId).FirstOrDefault().Equipe
                            });

                        }
                        else
                        {
                            result = result.OrderByDescending(x => new
                            {
                                Order = x.Equipes.OrderByDescending(z => z.EventoId).FirstOrDefault().Equipe
                            });
                        }

                    }
                    else
                    {
                        result = result.OrderBy(model.columns[model.order[0].column].name + " " + model.order[0].dir);
                    }
                }
                catch (Exception)
                {
                }

                result = result.Skip(model.Start)
                .Take(model.Length);

                return Json(new
                {
                    data = mapper.Map<IEnumerable<EquipanteListModel>>(result),
                    recordsTotal = totalResultsCount,
                    recordsFiltered = filteredResultsCount,
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult VerificaCadastro(string Fone)
        {
            var equipante = equipantesBusiness.GetEquipantes().FirstOrDefault(x => x.Fone.Replace("+", "").Replace("(", "").Replace(")", "").Replace(".", "").Replace("-", "") == Fone.Replace("+", "").Replace("(", "").Replace(")", "").Replace(".", "").Replace("-", ""));

            if (equipante != null)
                return Json(Url.Action("InscricaoConcluida", new { Id = equipante.Id }));
            else
                return new HttpStatusCodeResult(200);
        }

        public ActionResult InscricaoConcluida(int Id)
        {
            Equipante equipante = equipantesBusiness.GetEquipanteById(Id);
            var eventoAtual = eventosBusiness.GetEventoAtivo();

            ViewBag.Participante = new InscricaoConcluidaViewModel
            {
                Id = equipante.Id,
                Apelido = equipante.Nome,
                Logo = eventoAtual.TipoEvento.GetNickname() + ".png",
                Evento = $"{eventoAtual.TipoEvento.GetDescription()}",
                Valor = eventoAtual.Valor.ToString("C", CultureInfo.CreateSpecificCulture("pt-BR")),
                DataEvento = eventoAtual.DataEvento.ToString("dd/MM/yyyy"),
            };

            return View("InscricaoConcluida");

        }

        [HttpPost]
        public ActionResult GetEquipantes()
        {

            var result = equipantesBusiness.GetEquipantes();

            return Json(new { data = mapper.Map<IEnumerable<EquipanteListModel>>(result) }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetEquipante(int Id)
        {
            var result = equipantesBusiness.GetEquipanteById(Id);
            int eventoId = (eventosBusiness.GetEventoAtivo() ?? eventosBusiness.GetEventos().OrderByDescending(x => x.DataEvento).First()).Id;

            result.Nome = UtilServices.CapitalizarNome(result.Nome);
            result.Apelido = UtilServices.CapitalizarNome(result.Apelido);
            result.Equipe = equipesBusiness.GetEquipeAtual(eventoId, result.Id)?.Equipe.GetDescription() ?? "";

            var equipante = mapper.Map<PostEquipanteModel>(result);

            var dadosAdicionais = new
            {
                Status = result.Status.GetDescription(),
                Quarto = quartosBusiness.GetQuartosComParticipantes(eventoId, TipoPessoaEnum.Equipante).Where(x => x.EquipanteId == Id).FirstOrDefault()?.Quarto?.Titulo ?? ""               
            };

            return Json(new { Equipante = equipante, DadosAdicionais = dadosAdicionais }, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult PostEquipante(PostEquipanteModel model)
        {
            var equipante = equipantesBusiness.PostEquipante(model);

            if (model.Inscricao)
            {
                return Json(Url.Action("InscricaoConcluida", new { Id = equipante.Id }));
            }
            else
            {
                return new HttpStatusCodeResult(200);
            }

        }

        [HttpPost]
        public ActionResult DeleteEquipante(int Id)
        {
            equipantesBusiness.DeleteEquipante(Id);

            return new HttpStatusCodeResult(200);
        }

        [HttpPost]
        public ActionResult ToggleSexo(int Id)
        {
            equipantesBusiness.ToggleSexo(Id);

            return new HttpStatusCodeResult(200);
        }

        [HttpPost]
        public ActionResult ToggleVacina(int Id)
        {
            equipantesBusiness.ToggleVacina(Id);

            return new HttpStatusCodeResult(200);
        }


        [HttpPost]
        public ActionResult Ativar(int Id)
        {
            equipantesBusiness.Ativar(Id);

            return new HttpStatusCodeResult(200);
        }

        [HttpPost]
        public ActionResult ToggleCheckin(int Id)
        {
            equipantesBusiness.ToggleCheckin(Id);

            return new HttpStatusCodeResult(200);
        }
    }
}