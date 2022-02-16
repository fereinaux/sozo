using Arquitetura.Controller;
using Arquitetura.ViewModels;
using AutoMapper;
using Core.Business.Account;
using Core.Business.Arquivos;
using Core.Business.Circulos;
using Core.Business.ContaBancaria;
using Core.Business.Equipes;
using Core.Business.Eventos;
using Core.Business.Lancamento;
using Core.Business.MeioPagamento;
using Core.Business.Participantes;
using Core.Business.Quartos;
using Core.Models.Lancamento;
using Core.Models.Participantes;
using Core.Models.Quartos;
using SysIgreja.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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
    [Authorize(Roles = Usuario.Master + "," + Usuario.Admin + "," + Usuario.Secretaria)]
    public class ParticipanteController : SysIgrejaControllerBase
    {
        private readonly IParticipantesBusiness participantesBusiness;
        private readonly IEquipesBusiness equipesBusiness;
        private readonly ICirculosBusiness circulosBusiness;
        private readonly IArquivosBusiness arquivoBusiness;
        private readonly IQuartosBusiness quartosBusiness;
        private readonly ILancamentoBusiness lancamentoBusiness;
        private readonly IMeioPagamentoBusiness meioPagamentoBusiness;
        private readonly IContaBancariaBusiness contaBancariaBusiness;
        private readonly IDatatableService datatableService;
        private readonly IMapper mapper;

        public ParticipanteController(ILancamentoBusiness lancamentoBusiness, IQuartosBusiness quartosBusiness, IEquipesBusiness equipesBusiness, IArquivosBusiness arquivoBusiness, ICirculosBusiness circulosBusiness, IParticipantesBusiness participantesBusiness, IContaBancariaBusiness contaBancariaBusiness, IEventosBusiness eventosBusiness, IAccountBusiness accountBusiness, IDatatableService datatableService, IMeioPagamentoBusiness meioPagamentoBusiness) : base(eventosBusiness, accountBusiness)
        {
            this.participantesBusiness = participantesBusiness;
            this.arquivoBusiness = arquivoBusiness;
            this.quartosBusiness = quartosBusiness;
            this.equipesBusiness = equipesBusiness;
            this.circulosBusiness = circulosBusiness;
            this.lancamentoBusiness = lancamentoBusiness;
            this.meioPagamentoBusiness = meioPagamentoBusiness;
            this.contaBancariaBusiness = contaBancariaBusiness;
            this.datatableService = datatableService;
            mapper = new MapperRealidade().mapper;
        }

        public ActionResult ListaTelefonica()
        {
            ViewBag.Title = "Lista Telefônica";
            GetEventos();

            return View();
        }

        public ActionResult Checkin()
        {
            ViewBag.Title = "Check-in";
            GetEventos();
            ViewBag.MeioPagamentos = meioPagamentoBusiness.GetAllMeioPagamentos().ToList();
            ViewBag.ValorRealista = (int)ValoresPadraoEnum.Inscricoes;
            ViewBag.ValorEquipante = (int)ValoresPadraoEnum.TaxaEquipante;
            ViewBag.ContasBancarias = contaBancariaBusiness.GetContasBancarias().ToList()
                .Select(x => new ContaBancariaViewModel
                {
                    Banco = x.Banco.GetDescription(),
                    Id = x.Id
                });


            return View();
        }

        public ActionResult Etiquetas()
        {
            ViewBag.Title = "Impressão de Etiquetas";
            GetEventos();

            return View();
        }

        public ActionResult Boletos()
        {
            ViewBag.Title = "Boletos Solicitados";
            GetEventos();

            return View();
        }

        public ActionResult Index()
        {
            ViewBag.Title = "Participantes";
            GetEventos();
            ViewBag.MeioPagamentos = meioPagamentoBusiness.GetAllMeioPagamentos().ToList();
            ViewBag.Valor = (int)ValoresPadraoEnum.Inscricoes;
            ViewBag.ContasBancarias = contaBancariaBusiness.GetContasBancarias().ToList()
                .Select(x => new ContaBancariaViewModel
                {
                    Banco = x.Banco.GetDescription(),
                    Id = x.Id
                });

            return View();
        }

        private PostInscricaoModel mapParticipante(Data.Entities.Participante x)
        {
            return new PostInscricaoModel
            {
                Alergia = x.Alergia,
                Apelido = x.Apelido,
                Bairro = x.Bairro,
                CancelarCheckin = false,
                Checkin = x.Checkin,
                Complemento = x.Complemento,
                Congregacao = x.Congregacao,
                DataNascimento = x.DataNascimento,
                Email = x.Email,
                EventoId = x.EventoId,
                Fone = x.Fone,
                FoneConvite = x.FoneConvite,
                FoneMae = x.FoneMae,
                FonePai = x.FonePai,
                HasAlergia = x.HasAlergia,
                HasMedicacao = x.HasMedicacao,
                HasParente = false,
                HasRestricaoAlimentar = x.HasRestricaoAlimentar,
                Id = x.Id,
                Logradouro = x.Logradouro,
                Medicacao = x.Medicacao,
                Nome = x.Nome,
                HasVacina = x.HasVacina,
                NomeConvite = x.NomeConvite,
                NomeMae = x.NomeMae,
                NomePai = x.NomePai,
                Parente = x.Parente,
                RestricaoAlimentar = x.RestricaoAlimentar,
                Sexo = x.Sexo,
                Status = x.Status.GetDescription(),
                MsgGeral = x.MsgGeral,
                MsgPagamento = x.MsgPagamento,
                MsgVacina = x.MsgVacina,
                PendenciaBoleto = x.PendenciaBoleto,
                PendenciaContato = x.PendenciaContato,
                Observacao = x.Observacao
            };
        }

        [HttpGet]
        public ActionResult GetParticipante(int Id)
        {
            var result = mapParticipante(participantesBusiness.GetParticipanteById(Id));

            result.Nome = UtilServices.CapitalizarNome(result.Nome);
            result.Apelido = UtilServices.CapitalizarNome(result.Apelido);

            var quartoAtual = quartosBusiness.GetNextQuarto(result.EventoId, result.Sexo);

            var dadosAdicionais = new
            {
                Circulo = circulosBusiness.GetCirculosComParticipantes(result.EventoId).Where(x => x.ParticipanteId == Id)?.FirstOrDefault()?.Circulo?.Cor.GetDescription() ?? "",
                Status = result.Status.GetDescription(),
                Quarto = quartosBusiness.GetQuartosComParticipantes(result.EventoId).Where(x => x.ParticipanteId == Id).FirstOrDefault()?.Quarto?.Titulo ?? "",
                QuartoAtual = new
                {
                    Quarto = mapper.Map<PostQuartoModel>(quartoAtual),
                    Participantes = quartoAtual != null ? quartosBusiness.GetParticipantesByQuartos(quartoAtual.Id).Count() : 0
                }
            };

            return Json(new { Participante = result, DadosAdicionais = dadosAdicionais }, JsonRequestBehavior.AllowGet);
        }



        [HttpGet]
        public ActionResult GetParticipantesByCirculo(int CirculoId)
        {
            var result = circulosBusiness.GetParticipantesByCirculos(CirculoId).ToList().OrderBy(x => x.Participante.Nome).Select(x => new
            {
                Circulo = x.Circulo.Cor.GetDescription(),
                Nome = UtilServices.CapitalizarNome(x.Participante.Nome),
                Apelido = UtilServices.CapitalizarNome(x.Participante.Apelido),
                Fone = x.Participante.Fone,
            });

            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetParticipantesByQuarto(int QuartoId)
        {
            var result = quartosBusiness.GetParticipantesByQuartos(QuartoId).ToList().Select(x => new
            {
                Nome = UtilServices.CapitalizarNome(x.Participante.Nome),
                Medicacao = (x.Participante.Medicacao ?? "-") + "/" + (x.Participante.Alergia ?? "-")
            });

            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetAniversariantesByEvento(int EventoId)
        {
            var result = participantesBusiness.GetAniversariantesByEvento(EventoId).ToList().Select(x => new
            {
                Nome = UtilServices.CapitalizarNome(x.Nome),
                Apelido = UtilServices.CapitalizarNome(x.Apelido),
                Dia = x.DataNascimento.ToString("dd"),
                Idade = UtilServices.GetAge(x.DataNascimento).ToString()
            }).ToList();

            result.AddRange(equipesBusiness.GetEquipantesAniversariantesByEvento(EventoId).ToList().Select(x => new
            {
                Nome = UtilServices.CapitalizarNome(x.Nome),
                Apelido = UtilServices.CapitalizarNome(x.Apelido),
                Dia = x.DataNascimento.ToString("dd"),
                Idade = UtilServices.GetAge(x.DataNascimento).ToString()
            }));

            return Json(new { data = result.OrderBy(x => x.Dia) }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetParentesByEvento(int EventoId)
        {
            var result = participantesBusiness.GetParentesByEvento(EventoId).ToList().Select(x => new
            {
                Nome = UtilServices.CapitalizarNome(x.Nome),
                Circulo = circulosBusiness.GetCirculosComParticipantes(EventoId).FirstOrDefault(y => y.ParticipanteId == x.Id)?.Circulo.Cor.GetDescription(),
                Parente = UtilServices.CapitalizarNome(x.Parente)
            }).ToList();

            return Json(new { data = result.OrderBy(x => x.Nome) }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult PostInfo(PostInfoModel model)
        {
            participantesBusiness.PostInfo(model);
            return new HttpStatusCodeResult(200);

        }

        [HttpGet]
        public ActionResult GetTotaisCheckin(int EventoId)
        {
            var result = new
            {
                Confirmados = participantesBusiness.GetParticipantesByEvento(EventoId).Count(x => x.Status == StatusEnum.Confirmado),
                Presentes = participantesBusiness.GetParticipantesByEvento(EventoId).Count(x => x.Checkin),
                ConfirmadosEquipantes = equipesBusiness.GetEquipantesByEvento(EventoId).Count(),
                PresentesEquipantes = equipesBusiness.GetEquipantesByEvento(EventoId).Count(x => x.Checkin),
            };

            return Json(new { result }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetRestricoesByEvento(int EventoId)
        {
            var result = participantesBusiness.GetRestricoesByEvento(EventoId).ToList().Select(x => new
            {
                Nome = UtilServices.CapitalizarNome(x.Nome),
                Apelido = UtilServices.CapitalizarNome(x.Apelido),
                Restricao = x.RestricaoAlimentar
            }).ToList();

            result.AddRange(equipesBusiness.GetEquipantesRestricoesByEvento(EventoId).ToList().Select(x => new
            {
                Nome = UtilServices.CapitalizarNome(x.Nome),
                Apelido = UtilServices.CapitalizarNome(x.Apelido),
                Restricao = x.RestricaoAlimentar
            }));

            return Json(new { data = result.OrderBy(x => x.Nome) }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult GetParticipantesSelect(int EventoId)
        {

            var result = participantesBusiness
            .GetParticipantesByEvento(EventoId)
              .Where(x => x.Status == StatusEnum.Confirmado)
               .OrderBy(x => x.Nome);

            return Json(new { data = mapper.Map<IEnumerable<ParticipanteSelectModel>>(result) }, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public ActionResult GetParticipantesDatatable(FilterModel model)
        {
            var extract = Request.QueryString["extract"];
            if (extract == "excel")
            {
                Guid g = Guid.NewGuid();

                Session[g.ToString()] = datatableService.GenerateExcel(participantesBusiness
                .GetParticipantesByEvento(model.EventoId).ToList().Select(x => new ParticipanteExcelViewModel
                {
                    Nome = UtilServices.CapitalizarNome(x.Nome),

                    DataNascimento = x.DataNascimento.ToString("dd/MM/yyyy"),
                    Idade = UtilServices.GetAge(x.DataNascimento),
                    Sexo = x.Sexo.GetDescription(),
                    Fone = x.Fone,
                    Email = x.Email,
                    Congregacao = x.Congregacao,
                    Situacao = x.Status.GetDescription(),
                    DataCadastro = x.DataCadastro?.ToString("dd/MM/yyyy")

                }).ToList());

                return Content(g.ToString());
            }
            else
            {

                var result = participantesBusiness
                .GetParticipantesByEvento(model.EventoId);

                var totalResultsCount = result.Count();
                var filteredResultsCount = totalResultsCount;

                if (model.PadrinhoId > 0)
                {
                    result = result.Where(x => (x.PadrinhoId == model.PadrinhoId));
                    filteredResultsCount = result.Count();
                }

                if (model.search.value != null)
                {
                    result = result.Where(x => (x.Nome.Contains(model.search.value)));
                    filteredResultsCount = result.Count();
                }


                try
                {
                    result = result.OrderBy(model.columns[model.order[0].column].name + " " + model.order[0].dir);
                }
                catch (Exception)
                {
                }

                result = result.Skip(model.Start)
                .Take(model.Length);

                return Json(new
                {
                    data = mapper.Map<IEnumerable<ParticipanteListModel>>(result),
                    recordsTotal = totalResultsCount,
                    recordsFiltered = filteredResultsCount,
                }, JsonRequestBehavior.AllowGet);

            }
        }

        [HttpPost]
        public ActionResult GetParticipantes(int EventoId)
        {
            var list = participantesBusiness
                .GetParticipantesByEvento(EventoId)
                .ToList();

            var extract = Request.QueryString["extract"];
            if (extract == "excel")
            {
                Guid g = Guid.NewGuid();

                Session[g.ToString()] = datatableService.GenerateExcel(participantesBusiness
                .GetParticipantesByEvento(EventoId).ToList().Select(x => new ParticipanteExcelViewModel
                {
                    Nome = UtilServices.CapitalizarNome(x.Nome),
                    Apelido = UtilServices.CapitalizarNome(x.Apelido),
                    DataNascimento = x.DataNascimento.ToString("dd/MM/yyyy"),
                    DataCadastro = x.DataCadastro?.ToString("dd/MM/yyyy"),
                    Idade = UtilServices.GetAge(x.DataNascimento),
                    Sexo = x.Sexo.GetDescription(),
                    Fone = x.Fone,
                    Email = x.Email,
                    Endereco = $"{x.Logradouro} {x.Complemento}",
                    Bairro = x.Bairro,
                    NomeParente = x.HasParente ? UtilServices.CapitalizarNome(x.Parente) : "",
                    NomeConvite = UtilServices.CapitalizarNome(x.NomeConvite),
                    FoneConvite = x.FoneConvite,
                    Congregacao = x.Congregacao,
                    Alergia = x.Alergia,
                    Medicacao = x.Medicacao,
                    RestricaoAlimentar = x.RestricaoAlimentar,
                    Situacao = x.Status.GetDescription()

                }).ToList());

                return Content(g.ToString());
            }
            else
            {
                var result = participantesBusiness
                .GetParticipantesByEvento(EventoId)
                   .OrderBy(x => x.Nome);

                return Json(new { data = mapper.Map<IEnumerable<ParticipanteListModel>>(result) }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult GetParticipantesConfirmados(int EventoId)
        {
            var list = participantesBusiness
                .GetParticipantesByEvento(EventoId)
                .Where(x => x.Status == StatusEnum.Confirmado)
                .ToList();


            var result = list
               .OrderBy(x => x.Nome)
               .Select(x => new
               {
                   Id = x.Id,
                   Nome = UtilServices.CapitalizarNome(x.Nome),
                   Apelido = UtilServices.CapitalizarNome(x.Apelido),
                   Sexo = x.Sexo.GetDescription(),
                   Fone = x.Fone,
                   Status = x.Status.GetDescription(),
                   Checkin = x.Checkin,
                   Idade = UtilServices.GetAge(x.DataNascimento),
                   Circulo = x.Circulos.LastOrDefault().Circulo.Cor.GetDescription()
               });

            return Json(new { data = result }, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public ActionResult GetBoletos(int EventoId)
        {
            var list = participantesBusiness
                .GetParticipantesByEvento(EventoId)
                .Where(x => x.Boleto)
                .ToList();

            var result = list
           .Select(x => new ParticipanteViewModel
           {
               Id = x.Id,
               Nome = UtilServices.CapitalizarNome(x.Nome),
               Sexo = x.Sexo.GetDescription(),
               Fone = x.Fone,
               Status = x.Status.GetDescription(),
               Idade = UtilServices.GetAge(x.DataNascimento),
               PendenciaBoleto = x.PendenciaBoleto,
               DataCadastro = x.DataCadastro.Value.ToString("dd/MM/yyyy hh:mm")
           }); ;

            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ToggleBoleto(int ParticipanteId)
        {
            participantesBusiness.TogglePendenciaBoleto(ParticipanteId);

            return new HttpStatusCodeResult(200);
        }

        [HttpPost]
        public ActionResult ToggleContato(int ParticipanteId)
        {
            participantesBusiness.TogglePendenciaContato(ParticipanteId);

            return new HttpStatusCodeResult(200);
        }


        [HttpPost]
        public ActionResult CancelarInscricao(int Id)
        {
            participantesBusiness.CancelarInscricao(Id);

            return new HttpStatusCodeResult(200);
        }

        [HttpPost]
        public ActionResult ToggleSexo(int Id)
        {
            participantesBusiness.ToggleSexo(Id);

            return new HttpStatusCodeResult(200);
        }


        [HttpPost]
        public ActionResult ToggleVacina(int Id)
        {
            participantesBusiness.ToggleVacina(Id);

            return new HttpStatusCodeResult(200);
        }

        [HttpGet]
        public ActionResult GetPadrinhos(int eventoId)
        {
            return Json(new { Padrinhos = participantesBusiness.GetParticipantesByEvento(eventoId).Select(x => new { Id = x.PadrinhoId, Nome = x.Padrinho.Nome }).Distinct().ToList() }, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public ActionResult ToggleCheckin(int Id)
        {
            participantesBusiness.ToggleCheckin(Id);

            return new HttpStatusCodeResult(200);
        }

    }
}