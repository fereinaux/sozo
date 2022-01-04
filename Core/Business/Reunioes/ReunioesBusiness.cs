using Core.Models.Eventos;
using Core.Models.Reunioes;
using Data.Entities;
using Data.Repository;
using System.Data.Entity;
using System.Linq;
using Utils.Enums;

namespace Core.Business.Reunioes
{
    public class ReunioesBusiness : IReunioesBusiness
    {
        private readonly IGenericRepository<ReuniaoEvento> reuniaoRepository;        

        public ReunioesBusiness(IGenericRepository<ReuniaoEvento> reuniaoRepository)
        {
            this.reuniaoRepository = reuniaoRepository;            
        }

        public void DeleteReuniao(int id)
        {
            reuniaoRepository.Delete(id);
            reuniaoRepository.Save();
        }

        public ReuniaoEvento GetReuniaoAtiva()
        {
            return reuniaoRepository.GetAll().OrderByDescending(x => x.DataReuniao).First();
        }

        public ReuniaoEvento GetReuniaoById(int id)
        {
            return reuniaoRepository.GetById(id);
        }

        public IQueryable<ReuniaoEvento> GetReunioes(int eventoId)
        {
            return reuniaoRepository.GetAll(x => x.EventoId == eventoId).Include(x => x.Presenca);
        }

        public void PostReuniao(PostReuniaoModel model)
        {
            ReuniaoEvento reuniao = null;

            if (model.Id > 0)
            {
                reuniao = reuniaoRepository.GetById(model.Id);
                reuniao.DataReuniao = model.DataReuniao.AddHours(4);

                reuniaoRepository.Update(reuniao);
            }
            else
            {
                reuniao = new ReuniaoEvento
                {
                    DataReuniao = model.DataReuniao.AddHours(4),
                    EventoId = model.EventoId,
                    Status = StatusEnum.Ativo
                };

                reuniaoRepository.Insert(reuniao);
            }

            reuniaoRepository.Save();
        }
    }
}
