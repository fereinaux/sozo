using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Utils.Enums
{
    public enum StatusEnum
    {
        [Description("Ativo")]
        Ativo = 1,
        [Description("Inativo")]
        Inativo = 2,
        [Description("Deletado")]
        Deletado = 3,
        [Description("Aberto")]
        Aberto = 4,
        [Description("Quitado")]
        Quitado = 5,
        [Description("Cancelado")]
        Cancelado = 6,
        [Description("Encerrado")]
        Encerrado = 7,
        [Description("Inscrito")]
        Inscrito = 8,
        [Description("Confirmado")]
        Confirmado = 9,
        [Description("Em Espera")]
        Espera = 10
    }

    public enum SexoEnum
    {
        [Description("Masculino")]
        Masculino = 1,
        [Description("Feminino")]
        Feminino = 2
    }

    public enum MomentoSozoEnum
    {
        [Description("Louvor e Adoração")]
        Louvor = 1,
        [Description("Sexualidade")]
        Sexualidade = 2,
        [Description("Sala de Imersão")]
        Imersao = 3,
        [Description("Sala de Oração")]
        Oracao = 4,
        [Description("Sala de aconselhamento")]
        Acolhimento = 5,
    }


    public enum EquipesEnum
    {
        [Description("Ordem")]
        Ordem = 1,
        [Description("Refeitório")]
        Refeitorio = 2,
        [Description("Vigília")]
        Vigilia = 3,
        [Description("Coordenador Geral")]
        CoordenadorGeral = 4,
        [Description("Secretaria")]
        Secretaria = 5,
        [Description("Ligação Interna")]
        LigacaoInterna = 6,
        [Description("Círculos")]
        Circulo = 7,
        [Description("Louvor")]
        Louvor = 8,
        [Description("Sala")]
        Sala = 9,
        [Description("Bem Estar")]
        BemEstar = 10,
        [Description("Cozinha")]
        Cozinha = 11,
        [Description("Lanche")]
        Lanche = 12,
        [Description("Infraestrutura")]
        Infraestrutura = 13,
        [Description("Crianças")]
        Criancas = 14,
        [Description("Filhos")]
        Filhos = 15,
        [Description("Livraria")]
        Livraria = 16,
        [Description("Coordenador do Serviço")]
        CoordServico = 17,
        [Description("Dirigente Espiritual")]
        DirigenteEspiritual = 18,
        [Description("Palestrante")]
        Palestrante = 19,
        [Description("Pastor")]
        Pastor = 20,
        [Description("Vigilante")]
        Vigilante = 21,
        [Description("Momento Sozo de Louvor e Adoração")]
        MomentoSozoLouvor = 22,
        [Description("Sala de Imersão")]
        SalaImersao = 23,
        [Description("Sala de Oração")]
        SalaOracao = 24,
        [Description("Momento Sozo de Sexualidade ")]
        MomentoSozoSexualidade = 25,
        [Description("Sala de aconselhamento")]
        SalaAconselhamento = 26
    }

    public enum TiposEventoEnum
    {

        [EmailPagSeguro("sves.trindade@gmail.com")]
        [TokenPagSeguro("ba655565-e03a-4b6d-a6e3-2580cca7e8c9a2f98b924ac2839d219f932121848b0dbfc3-fe90-4770-ab9f-3699835aaad2")]
        [Nickname("SOZO")]
        [Description("SOZO")]
        [Equipes(new int[] { 1, 2, 3, 4, 5, 6, 8, 9, 10, 11, 12, 13, 16, 17, 18, 19, 20, 22, 23, 24, 25, 26 })]
        SOZO = 2

    }

    public enum BancosEnum
    {
        [Description("Bradesco")]
        Bradesco = 1,
        [Description("Santander")]
        Santander = 2,
        [Description("Itaú")]
        Itau = 3,
        [Description("Caixa")]
        Caixa = 4,
        [Description("Banco do Brasil")]
        BancoBrasil = 5,
        [Description("NuBank")]
        Nubank = 6,
        [Description("Banco Inter")]
        Inter = 7,
        [Description("HSBC")]
        HSBC = 8
    }

    public enum TiposEquipeEnum
    {
        [Description("Coordenador")]
        Coordenador = 1,
        [Description("Membro")]
        Membro = 2
    }

    public enum PerfisUsuarioEnum
    {
        [Description("Master")]
        Master,
        [Description("Admin")]
        Admin,
        [Description("Coordenador")]
        Coordenador,
        [Description("Secretaria")]
        Secretaria,
    }

    public enum TiposLancamentoEnum
    {
        [Description("Receber")]
        Receber = 1,
        [Description("Pagar")]
        Pagar = 2
    }

    public enum TiposCentroCustoEnum
    {
        [Description("Receita")]
        Receita = 1,
        [Description("Despesa")]
        Despesa = 2
    }

    public enum CentroCustoPadraoEnum
    {
        [Description("Inscrições")]
        Inscricoes = 1,
        [Description("Taxa de Equipante")]
        TaxaEquipante = 2
    }

    public enum ValoresPadraoEnum
    {
        [Description("Inscrições")]
        Inscricoes = 250,
        [Description("Taxa de Equipante")]
        TaxaEquipante = 120
    }

    public enum TipoPessoaEnum
    {
        [Description("Equipante")]
        Equipante,
        [Description("Participante")]
        Participante
    }


    public enum MeioPagamentoPadraoEnum
    {
        [Description("Dinheiro")]
        Dinheiro,
        [Description("Transferência Bancária")]
        Transferencia,
        [Description("Cheque")]
        Cheque,
        [Description("Isenção")]
        Isencao,
        [Description("PagSeguro")]
        PagSeguro
    }

    public enum CoresEnum
    {
        [Description("Vermelho")]
        Vermelho,
        [Description("Laranja")]
        Laranja,
        [Description("Rosa")]
        Rosa,
        [Description("Azul Claro")]
        AzulClaro,
        [Description("Azul Escuro")]
        AzulEscuro,
        [Description("Verde Claro")]
        VerdeClaro,
        [Description("Verde Escuro")]
        VerdeEscuro,
        [Description("Amarelo")]
        Amarelo,
        [Description("Cinza")]
        Cinza,
        [Description("Lilás")]
        Lilas
    }

    internal class NicknameAttribute : Attribute
    {
        public virtual string Nickname { get; }

        public NicknameAttribute(string Nickname)
        {
            this.Nickname = Nickname;
        }
    }

    internal class EquipesAttribute : Attribute
    {
        public virtual int[] Equipes { get; }

        public EquipesAttribute(int[] Equipes)
        {
            this.Equipes = Equipes;
        }
    }

    internal class EmailPagSeguroAttribute : Attribute
    {
        public virtual string EmailPagSeguro { get; }

        public EmailPagSeguroAttribute(string EmailPagSeguro)
        {
            this.EmailPagSeguro = EmailPagSeguro;
        }
    }

    internal class TokenPagSeguroAttribute : Attribute
    {
        public virtual string TokenPagSeguro { get; }

        public TokenPagSeguroAttribute(string TokenPagSeguro)
        {
            this.TokenPagSeguro = TokenPagSeguro;
        }
    }
}