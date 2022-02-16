using Core.Models.Eventos;
using Data.Entities;
using Data.Repository;
using System.Linq;
using System.Data.Entity;
using Utils.Enums;

namespace Core.Business.Eventos
{
    public class EventosBusiness : IEventosBusiness
    {
        private readonly IGenericRepository<Evento> eventoRepository;
        private readonly IGenericRepository<Circulo> circuloRepository;

        public EventosBusiness(IGenericRepository<Evento> eventoRepository, IGenericRepository<Circulo> circuloRepository)
        {
            this.eventoRepository = eventoRepository;
            this.circuloRepository = circuloRepository;
        }

        public void DeleteEvento(int id)
        {
            eventoRepository.Delete(id);
            eventoRepository.Save();
        }

        public Evento GetEventoAtivo()
        {
            return eventoRepository.GetAll().FirstOrDefault(e => e.Status == StatusEnum.Aberto);
        }

        public Evento GetEventoById(int id)
        {
            return eventoRepository.GetById(id);
        }

        public IQueryable<Evento> GetEventos()
        {
            return eventoRepository.GetAll();
        }

        public void PostEvento(PostEventoModel model)
        {
            Evento evento = null;

            if (model.Id > 0)
            {
                evento = eventoRepository.GetById(model.Id);

                evento.DataEvento = model.DataEvento.AddHours(5);
                evento.Capacidade = model.Capacidade;
                evento.TipoEvento = (TiposEventoEnum)model.TipoEvento;
                evento.Numeracao = model.Numeracao;
                evento.Valor = model.Valor;

                eventoRepository.Update(evento);
            }
            else
            {
                evento = new Evento
                {
                    DataEvento = model.DataEvento.AddHours(5),
                    Numeracao = model.Numeracao,
                    Capacidade = model.Capacidade,
                    Valor = model.Valor,
                    TipoEvento = (TiposEventoEnum)model.TipoEvento,
                    Status = GetEventoAtivo() != null ?
                    StatusEnum.Encerrado :
                    StatusEnum.Aberto
                };

                eventoRepository.Insert(evento);

                circuloRepository.Insert(new Circulo
                {
                    EventoId = evento.Id,
                    Cor = CoresEnum.Vermelho,
                    Momento1 = MomentoSozoEnum.Louvor,
                    Momento2 = MomentoSozoEnum.Acolhimento,
                    Momento3 = MomentoSozoEnum.Imersao,
                    Momento4 = MomentoSozoEnum.Oracao,
                    Momento5 = MomentoSozoEnum.Sexualidade,
                });

                circuloRepository.Insert(new Circulo
                {
                    EventoId = evento.Id,
                    Cor = CoresEnum.Laranja,
                    Momento5 = MomentoSozoEnum.Louvor,
                    Momento1 = MomentoSozoEnum.Acolhimento,
                    Momento2 = MomentoSozoEnum.Imersao,
                    Momento3 = MomentoSozoEnum.Oracao,
                    Momento4 = MomentoSozoEnum.Sexualidade,
                });

                circuloRepository.Insert(new Circulo
                {
                    EventoId = evento.Id,
                    Cor = CoresEnum.Rosa,
                    Momento4 = MomentoSozoEnum.Louvor,
                    Momento5 = MomentoSozoEnum.Acolhimento,
                    Momento1 = MomentoSozoEnum.Imersao,
                    Momento2 = MomentoSozoEnum.Oracao,
                    Momento3 = MomentoSozoEnum.Sexualidade,
                });

                circuloRepository.Insert(new Circulo
                {
                    EventoId = evento.Id,
                    Cor = CoresEnum.AzulClaro,
                    Momento3 = MomentoSozoEnum.Louvor,
                    Momento4 = MomentoSozoEnum.Acolhimento,
                    Momento5 = MomentoSozoEnum.Imersao,
                    Momento1 = MomentoSozoEnum.Oracao,
                    Momento2 = MomentoSozoEnum.Sexualidade,
                });

                circuloRepository.Insert(new Circulo
                {
                    EventoId = evento.Id,
                    Cor = CoresEnum.AzulEscuro,
                    Momento2 = MomentoSozoEnum.Louvor,
                    Momento3 = MomentoSozoEnum.Acolhimento,
                    Momento4 = MomentoSozoEnum.Imersao,
                    Momento5 = MomentoSozoEnum.Oracao,
                    Momento1 = MomentoSozoEnum.Sexualidade,
                });
            }

            eventoRepository.Save();
        }

        public bool ToggleEventoStatus(int id)
        {
            Evento evento = eventoRepository.GetById(id);

            StatusEnum status = evento.Status == StatusEnum.Aberto ?
                StatusEnum.Encerrado :
                StatusEnum.Aberto;

            if (GetEventoAtivo() != null &&
                status == StatusEnum.Aberto)
                return false;

            evento.Status = status;

            eventoRepository.Update(evento);

            eventoRepository.Save();

            return true;
        }
    }
}
