using System;

namespace SysIgreja.ViewModels
{
    public class CirculoViewModel
    {
        public int Id { get; set; }
        public string Dirigente1 { get; set; }
        public string Dirigente2 { get; set; }
        public int QtdParticipantes { get; set; }
        public string Cor { get; set; }
        public string Momento1 { get; internal set; }
        public string Momento2 { get; internal set; }
        public string Momento3 { get; internal set; }
        public string Momento4 { get; internal set; }
        public string Momento5 { get; internal set; }
    }


    public class MomentoSozoViewModel
    {
        public string Momento { get; set; }

        public string Cor1 { get; set; }
        public string Cor2 { get; set; }
        public string Cor3 { get; set; }
        public string Cor4 { get; set; }
        public string Cor5 { get; set; }

    }



}
