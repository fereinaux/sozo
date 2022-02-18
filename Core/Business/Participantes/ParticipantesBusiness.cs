using Core.Business.Circulos;
using Core.Business.Eventos;
using Core.Business.Quartos;
using Core.Models.Participantes;
using Data.Entities;
using Data.Repository;
using System.Linq;
using System.Data.Entity;
using Utils.Enums;
using System;

namespace Core.Business.Participantes
{
    public class ParticipantesBusiness : IParticipantesBusiness
    {
        private readonly IGenericRepository<Participante> participanteRepository;
        private readonly IEventosBusiness eventosBusiness;
        private readonly ICirculosBusiness circulosBusiness;
        private readonly IQuartosBusiness quartosBusiness;
        private readonly IGenericRepository<EquipanteEvento> equipanteEventoRepository;

        public ParticipantesBusiness(IGenericRepository<Participante> participanteRepository, IGenericRepository<EquipanteEvento> equipanteEventoRepository, IQuartosBusiness quartosBusiness, IEventosBusiness eventosBusiness, ICirculosBusiness circulosBusiness)
        {
            this.participanteRepository = participanteRepository;
            this.equipanteEventoRepository = equipanteEventoRepository;
            this.eventosBusiness = eventosBusiness;
            this.quartosBusiness = quartosBusiness;
            this.circulosBusiness = circulosBusiness;
        }

        public void CancelarInscricao(int id)
        {
            Participante participante = participanteRepository.GetById(id);
            participante.Status = StatusEnum.Cancelado;
            circulosBusiness.ChangeCirculo(id, null);
            quartosBusiness.ChangeQuarto(id, null,null);

            var emEspera = participanteRepository.GetAll().Where(x => x.Status == StatusEnum.Espera).OrderBy(x => x.Id).FirstOrDefault();

            if (emEspera != null && participanteRepository.GetAll().Where(x => x.Status == StatusEnum.Confirmado || x.Status == StatusEnum.Inscrito).Count() - 1 < eventosBusiness.GetEventoById(participante.EventoId).Capacidade)
            {
                emEspera.Status = StatusEnum.Inscrito;
                participanteRepository.Update(emEspera);
            }

            participanteRepository.Update(participante);
            participanteRepository.Save();
        }

        public Participante GetParticipanteById(int id)
        {
            return participanteRepository.GetAll(x => x.Id == id).Include(x => x.Evento).Include(x => x.Padrinho).SingleOrDefault();
        }

        public Participante GetParticipanteByReference(string reference)
        {
            return participanteRepository.GetAll(x => x.ReferenciaPagSeguro == reference).Include(x => x.Evento).SingleOrDefault();
        }

        public IQueryable<Participante> GetParticipantes()
        {
            return participanteRepository.GetAll().Include(x => x.Evento);
        }

        public int PostInscricao(PostInscricaoModel model)
        {
            Participante participante = null;
            if (model.Id > 0)
            {
                participante = MapUpdateParticipante(model);
                participanteRepository.Update(participante);
            }
            else
            {
                participante = MapCreateParticipante(model);
                participanteRepository.Insert(participante);
            }

            participanteRepository.Save();

            CheckIn(participante, model.CancelarCheckin);

            return participante.Id;
        }

        private void CheckIn(Participante participante, bool cancelarCheckin)
        {
            if (participante.Checkin)
            {
                ManageCirculo(participante);
                ManageQuarto(participante);
            }
            else
            {
                if (cancelarCheckin)
                {
                    circulosBusiness.ChangeCirculo(participante.Id, null);
                    quartosBusiness.ChangeQuarto(participante.Id, null,null);
                }
            }
        }

        private void ManageQuarto(Participante participante)
        {
            if (!quartosBusiness
                              .GetQuartosComParticipantes(participante.EventoId,TipoPessoaEnum.Participante)
                              .Where(x => x.ParticipanteId == participante.Id)
                              .Any())
            {
                var quarto = quartosBusiness.GetNextQuarto(participante.EventoId, participante.Sexo, TipoPessoaEnum.Participante);
                if (quarto != null)
                    quartosBusiness.ChangeQuarto(participante.Id, quarto.Id,null);
            }
        }

        private void ManageCirculo(Participante participante)
        {
            if (!circulosBusiness
                                .GetCirculosComParticipantes(participante.EventoId)
                                .Where(x => x.ParticipanteId == participante.Id)
                                .Any())
            {
                var circulo = circulosBusiness.GetNextCirculo(participante.EventoId);
                if (circulo != null)
                    circulosBusiness.ChangeCirculo(participante.Id, circulo.Id);
            }
        }

        private Participante MapUpdateParticipante(PostInscricaoModel model)
        {
            Participante participante = participanteRepository.GetById(model.Id);
            participante.Nome = model.Nome;
            participante.Apelido = model.Apelido;
            participante.DataNascimento = model.DataNascimento.AddHours(5);
            participante.Fone = model.Fone;
            participante.Email = model.Email;
            participante.Logradouro = model.Logradouro;
            participante.Complemento = model.Complemento;
            participante.Bairro = model.Bairro;
            participante.NomePai = model.NomePai;
            participante.FonePai = model.FonePai;
            participante.NomeMae = model.NomeMae;
            participante.FoneMae = model.FoneMae;
            participante.NomeConvite = model.NomeConvite;
            participante.FoneConvite = model.FoneConvite;
            participante.Sexo = model.Sexo;
            participante.HasAlergia = model.HasAlergia;
            participante.Congregacao = model.Congregacao;
            participante.Alergia = model.HasAlergia ? model.Alergia : null;
            participante.HasParente = model.HasParente;
            participante.Parente = model.HasParente ? model.Parente : null;
            participante.HasMedicacao = model.HasMedicacao;
            participante.Medicacao = model.HasMedicacao ? model.Medicacao : null;
            participante.HasRestricaoAlimentar = model.HasRestricaoAlimentar;
            participante.RestricaoAlimentar = model.HasRestricaoAlimentar ? model.RestricaoAlimentar : null;
            participante.Checkin = model.Checkin;
            return participante;
        }

        private Participante MapCreateParticipante(PostInscricaoModel model)
        {
            return new Participante
            {
                Nome = model.Nome,
                Apelido = model.Apelido,
                DataNascimento = model.DataNascimento.AddHours(5),
                Fone = model.Fone,
                Email = model.Email,
                Logradouro = model.Logradouro,
                Complemento = model.Complemento,
                Bairro = model.Bairro,
                NomePai = model.NomePai,
                FonePai = model.FonePai,
                NomeMae = model.NomeMae,
                FoneMae = model.FoneMae,
                NomeConvite = model.NomeConvite,
                FoneConvite = model.FoneConvite,
                ReferenciaPagSeguro = Guid.NewGuid().ToString(),
                Sexo = model.Sexo,
                Status = model.Status == "Espera" ? StatusEnum.Espera : StatusEnum.Inscrito,
                HasAlergia = model.HasAlergia,
                Congregacao = model.Congregacao,
                Alergia = model.HasAlergia ? model.Alergia : null,
                HasParente = model.HasParente,
                Parente = model.HasParente ? model.Parente : null,
                HasMedicacao = model.HasMedicacao,
                Medicacao = model.HasMedicacao ? model.Medicacao : null,
                HasRestricaoAlimentar = model.HasRestricaoAlimentar,
                RestricaoAlimentar = model.HasRestricaoAlimentar ? model.RestricaoAlimentar : null,
                EventoId = model.EventoId,
                PendenciaContato = false,
                Boleto = false,
                PendenciaBoleto = false,
                Checkin = model.Checkin,
                PadrinhoId = getNextPadrinho(model.EventoId)?.Id
            };
        }

        public IQueryable<Participante> GetParticipantesByEvento(int eventoId)
        {
            return participanteRepository.GetAll(x => x.EventoId == eventoId).Include(x => x.Evento).Include(x => x.Padrinho).Include(x => x.Circulos).Include(x => x.Circulos.Select(y => y.Circulo));
        }

        private Equipante getNextPadrinho(int eventoid)
        {
            var query = equipanteEventoRepository
                 .GetAll(x => x.EventoId == eventoid && x.Equipe == EquipesEnum.Secretaria)
                 .Include(x => x.Equipante)
                 .ToList()
                 .Select(x => new
                 {
                     Equipante = x,
                     Qtd = participanteRepository.GetAll(y => y.PadrinhoId == x.EquipanteId && (y.Status == StatusEnum.Confirmado || y.Status == StatusEnum.Inscrito)).Count()
                 })
                 .ToList();

            return query.Any() ? query.OrderBy(x => x.Qtd).FirstOrDefault().Equipante.Equipante : null;

        }

        public void TogglePendenciaContato(int id)
        {
            var participante = GetParticipanteById(id);
            participante.PendenciaContato = !participante.PendenciaContato;
            participanteRepository.Update(participante);
            participanteRepository.Save();
        }

        public void TogglePendenciaBoleto(int id)
        {
            var participante = GetParticipanteById(id);
            participante.PendenciaBoleto = !participante.PendenciaBoleto;
            participanteRepository.Update(participante);
            participanteRepository.Save();
        }

        public void SolicitarBoleto(int id)
        {
            var participante = GetParticipanteById(id);
            participante.PendenciaBoleto = false;
            participante.Boleto = true;
            participanteRepository.Update(participante);
            participanteRepository.Save();
        }

        public IQueryable<Participante> GetAniversariantesByEvento(int eventoId)
        {
            var data = eventosBusiness.GetEventoById(eventoId).DataEvento;

            return participanteRepository.GetAll(x => x.Status != StatusEnum.Cancelado && x.EventoId == eventoId && x.DataNascimento.Month == data.Month);
        }

        public IQueryable<Participante> GetRestricoesByEvento(int eventoId)
        {
            return participanteRepository.GetAll(x => x.Status != StatusEnum.Cancelado && x.EventoId == eventoId && x.HasRestricaoAlimentar);
        }

        public void ToggleSexo(int id)
        {
            var participante = GetParticipanteById(id);
            participante.Sexo = participante.Sexo == SexoEnum.Feminino ? SexoEnum.Masculino : SexoEnum.Feminino;
            participanteRepository.Update(participante);
            participanteRepository.Save();
        }

        public IQueryable<Participante> GetParentesByEvento(int eventoId)
        {
            return participanteRepository.GetAll(x => x.Status != StatusEnum.Cancelado && x.EventoId == eventoId && x.HasParente);
        }

        public void ToggleVacina(int id)
        {
            var participante = GetParticipanteById(id);
            participante.HasVacina = !participante.HasVacina;
            participanteRepository.Update(participante);
            participanteRepository.Save();
        }

        public void PostInfo(PostInfoModel model)
        {
            var participante = GetParticipanteById(model.Id);
            participante.Observacao = model.Observacao;
            participante.MsgGeral = model.MsgGeral;
            participante.MsgVacina = model.MsgVacina;
            participante.MsgPagamento = model.MsgPagamento;
            participanteRepository.Update(participante);
            participanteRepository.Save();
        }

        public void ToggleCheckin(int id)
        {
            var participante = GetParticipanteById(id);
            participante.Checkin = !participante.Checkin;
            participanteRepository.Update(participante);
            participanteRepository.Save();

            if (participante.Checkin)
            {
                ManageCirculo(participante);
                ManageQuarto(participante);
            }

        }
    }
}
