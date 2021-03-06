function CarregarTabelaReuniao() {
    const tableReuniaoConfig = {
        language: languageConfig,
        lengthMenu: [200,500,1000],
        colReorder: false,
        serverSide: false,
        deferloading: 0,
        orderCellsTop: true,
        fixedHeader: true,
        filter: true,
        orderMulti: false,
        responsive: true, stateSave: true,
        destroy: true,
        dom: domConfig,
        buttons: getButtonsConfig('Reuniões'),
        columns: [
            {
                data: "DataReuniao", name: "DataReuniao", autoWidth: true,
                "render": function (data, type, row) {
                    return `${moment(data).format('DD/MM/YYYY')} `;
                }
            },
            { data: "Presenca", name: "Presenca", autoWidth: true },
            {
                data: "Id", name: "Id", className: "text-center", orderable: false, width: "10%",
                "render": function (data, type, row) {
                    return `${GetButton('EditReuniao', data, 'blue', 'fa-edit', 'Editar')}                            
                               ${GetButton('DeleteReuniao', data, 'red', 'fa-trash', 'Excluir')}`;
                }
            }
        ],
        order: [
            [1, "asc"]
        ],
        ajax: {
            url: '/Reuniao/GetReunioes',
            data: { EventoId: $("#reuniao-eventoid-consulta").val() },
            datatype: "json",
            type: "POST"
        }
    };

    $("#table-reunioes").DataTable(tableReuniaoConfig);
}

function GetReuniao(id) {
    if (id > 0) {
        $.ajax({
            url: "/Reuniao/GetReuniao/",
            data: { Id: id },
            datatype: "json",
            type: "GET",
            contentType: 'application/json; charset=utf-8',
            success: function (data) {
                $("#reuniao-id").val(data.Reuniao.Id);
                $("#reuniao-data").val(moment(data.Reuniao.DataReuniao).format('DD/MM/YYYY'));
            }
        });
    }
    else {
        $("#reuniao-id").val(0);
        $("#reuniao-data").val("");
    }
}

function EditReuniao(id) {
    GetReuniao(id);
    $("#modal-reunioes").modal();
}

function DeleteReuniao(id) {
    ConfirmMessageDelete().then((result) => {
        if (result) {
            $.ajax({
                url: "/Reuniao/DeleteReuniao/",
                datatype: "json",
                type: "POST",
                contentType: 'application/json; charset=utf-8',
                data: JSON.stringify(
                    {
                        Id: id
                    }),
                success: function () {
                    SuccessMesageDelete();
                    CarregarTabelaReuniao();
                }
            });
        }
    });
}

function PostReuniao() {
    if (ValidateForm(`#form-reuniao`)) {
        $.ajax({
            url: "/Reuniao/PostReuniao/",
            datatype: "json",
            type: "POST",
            contentType: 'application/json; charset=utf-8',
            data: JSON.stringify(
                {
                    Id: $("#reuniao-id").val(),
                    EventoId: $("#reuniao-eventoid-consulta").val(),
                    DataReuniao: moment($("#reuniao-data").val(), 'DD/MM/YYYY', 'pt-br').toJSON()
                }),
            success: function () {
                SuccessMesageOperation();
                CarregarTabelaReuniao();
                $("#modal-reunioes").modal("hide");
            }
        });
    }
}

$(document).ready(function () {
    CarregarTabelaReuniao();
});


