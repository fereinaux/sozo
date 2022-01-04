﻿using Core.Models.Lancamento;
using Core.Models.Participantes;
using System.Linq;

namespace Core.Business.Participantes
{
    public interface IParticipantesBusiness
    {
        IQueryable<Data.Entities.Participante> GetParticipantes();
        IQueryable<Data.Entities.Participante> GetParticipantesByEvento(int eventoId);
        IQueryable<Data.Entities.Participante> GetAniversariantesByEvento(int eventoId);
        IQueryable<Data.Entities.Participante> GetParentesByEvento(int eventoId);
        IQueryable<Data.Entities.Participante> GetRestricoesByEvento(int eventoId);
        Data.Entities.Participante GetParticipanteById(int id);
        Data.Entities.Participante GetParticipanteByReference(string reference);
        int PostInscricao(PostInscricaoModel model);
        void CancelarInscricao(int id);
        void TogglePendenciaContato(int id);
        void TogglePendenciaBoleto(int id);
        void SolicitarBoleto(int id);
        void ToggleCheckin(int id);
        void PostInfo(PostInfoModel model);
        void ToggleSexo(int id);
        void ToggleVacina(int id);
    }
}
