function CarregarTabelaListaTelefonica() {
    const tableListaTelefonicaConfig = {
        language: languageConfig,
        lengthMenu: [200, 500, 1000],
        colReorder: false,
        serverSide: false,
        deferloading: 0,
        orderCellsTop: true,
        fixedHeader: true,
        filter: true,
        orderMulti: false,
        responsive: true, stateSave: true,
        destroy: true,
        dom: domConfigNoButtons,
        columns: [
            {
                data: "Id", name: "Id", className: "text-center", orderable: false, width: "5%",
                "render": function (data, type, row) {
                    return `${GetCheckBox('ToggleContato', data, data, row.PendenciaContato)}`;
                }
            },
            {
                data: "Id", orderable: false, name: "Id", className: "text-center", width: "15%",
                "render": function (data, type, row) {
                    if (row.Sexo == "Masculino") {
                        icon = "fa-male";
                        cor = "#0095ff";
                    }
                    else {
                        icon = "fa-female";
                        cor = "#ff00d4";
                    }
                    if (row.Status === Confirmado)
                        corStatus = "primary";
                    if (row.Status === Cancelado)
                        corStatus = "danger";
                    if (row.Status === Inscrito)
                        corStatus = "info";
                    return `<span style = "font-size:18px;color:${cor};" class="p-l-xs"> <i class="fa ${icon}" aria-hidden="true" title="${row.Sexo}"></i></span >
                            <span style="font-size:13px" class="text-center label label-${corStatus}">${row.Status}</span>
                            ${row.Status != Cancelado ? GetButton('CancelarInscricao', JSON.stringify(row), 'red', 'fa-times', 'Cancelar Inscrição') : ""}`;
                }
            },
            { data: "Nome", name: "Nome", autoWidth: true },
            {
                data: "Fone", name: "Fone", orderable: false, autoWidth: true,
                "render": function (data, type, row) {
                    return `
                            ${GetIconWhatsApp(data, GetText(row))}${GetIconTel(data)}`;
                }
            }
        ],
        order: [
            [2, "asc"]
        ],
        initComplete: function () {
            $('.i-checks-green').iCheck({
                checkboxClass: 'icheckbox_square-green',
                radioClass: 'iradio_square-green'
            });
            $('.i-checks-green').on('ifClicked', function (event) {
                ToggleContato($(event.target).data("id"));
            });
        },
        ajax: {
            url: '/Participante/GetListaTelefonica',
            data: { EventoId: $("#lista-telefonica-eventoid").val() },
            datatype: "json",
            type: "POST"
        }
    };

    $("#table-lista-telefonica").DataTable(tableListaTelefonicaConfig);
}

$(document).ready(function () {
    CarregarTabelaListaTelefonica();
});

function ToggleContato(id) {
    $.ajax({
        url: "/Participante/ToggleContato/",
        datatype: "json",
        type: "POST",
        contentType: 'application/json; charset=utf-8',
        data: JSON.stringify(
            {
                ParticipanteId: id
            })
    });
}

function CancelarInscricao(row) {
    ConfirmMessageCancelar(row.Nome).then((result) => {
        if (result) {
            $.ajax({
                url: "/Participante/CancelarInscricao/",
                datatype: "json",
                type: "POST",
                contentType: 'application/json; charset=utf-8',
                data: JSON.stringify(
                    {
                        Id: row.Id
                    }),
                success: function () {
                    SuccessMesageOperation();
                    CarregarTabelaListaTelefonica();
                }
            });
        }
    });
}

function GetText(row) {
    if (row.Status == Confirmado) {
        return `Olá ${row.Nome}, /n/nSeja Bem Vindo ao *${row.Evento}*. Estamos Felizes com sua participação e temos certeza que serão dias muito especiais em sua vida. Aproveite cada minuto desse Seminário!!
Avisos importantes:
1.	O evento ocorrerá no *Espaço Colonial de Aldeia, Km 11,5-nº 2554* - Link para o Mapa: https://goo.gl/maps/HYHA1NSx8pWU5F7M8;
2.	Horário de chegada no dia *22 de novembro às 19h* e encerramento no dia *24 de novembro às 12h*;
3.	Será  fornecido itens de cama  banho (travesseiro, lençol, cobertor e toalha de banho). Caso queira levar os de uso pessoal, fique à vontade;
4.	Levar *remédios de uso continuo e eventual*, como também, dietas restritivas. O Bem Estar trabalha com medicações básicas e para uso em casos de emergência, como também a Cozinha, que trabalha com itens para a produção do cardápio já definido pela organização do SVES.
5.	Levar *casaco de frio*;
6.	Atentar para uso de *roupas que estejam condizentes* com o ambiente;
7.	Se você faz uso de algum medicamento no decorrer do dia, leve-o com você ao sair do quarto;
8.	Evite sair do Plenário, pois cada momento é único e esteja atento ao que *Deus quer falar com você*. ${RodapeEvento()}`;

    } else {
        return `Olá ${row.Nome}, /n/nAguardamos o pagamento da sua inscrição no *${row.Evento}* até o dia *17/11*./nApós essa data sua vaga será disponibilizada.

O pagamento pode ser feito de duas maneiras: 

1. Cartão de crédito ou débito *na Igreja* 

2. Transferência ou Depósito:

Caixa Econômica 
Ag 3484
Conta poupança: 00010367-8
Junia Fidelis de Souza Cavalcanti 
CPF: 321.968.814-49

Enviar o comprovante para o WhatsApp *98811.5271-Junia(Barbie)*`;
    }


}