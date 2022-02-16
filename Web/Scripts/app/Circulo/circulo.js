function CarregarTabelaCirculo() {
    $('#gerenciar').text("Gerenciar Momentos SOZO");
    $('#participantes-sem').text("Participantes sem Grupo");
    $("#circulo-dirigentes").html(``)

    var columnsTb = [
        { data: "Cor", name: "Cor", autoWidth: true },
        { data: "QtdParticipantes", name: "QtdParticipantes", autoWidth: true },
        {
            data: "Id", name: "Id", className: "text-center", orderable: false, width: "15%",
            "render": function (data, type, row) {
                return `
                            ${GetButton('PrintCirculo', JSON.stringify(row), 'green', 'fa-print', 'Imprimir')}  
                            ${GetButton('DeleteCirculo', data, 'red', 'fa-trash', 'Excluir')}`;
            }
        }
    ]
    //${GetButton('EditCirculo', JSON.stringify(row), 'blue', 'fa-edit', 'Editar')}                            
    $("#circulo-cabecalho").html(`
                        <th>Cor</th>
                        <th>Membros </th>
                        <th>Ações</th>`)



    const tableCirculoConfig = {
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
        columns: columnsTb,
        order: [
            [0, "asc"]
        ],
        ajax: {
            url: '/Circulo/GetCirculos',
            datatype: "json",
            data: { EventoId: $("#circulo-eventoid").val() },
            type: "POST"
        }
    };
    $("#table-circulo").DataTable(tableCirculoConfig);
}

function CarregarTabelaSozo() {


    const tableCirculoConfig = {
        language: languageConfig,
        lengthMenu: [5],
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
        buttons: [
            {
                text: 'Imprimir',
                action: function (e, dt, node, config) {
                    PrintSozo()
                }
            }
        ],
        columns: [
            { data: "Momento", name: "Momento", width: '15%' },
            { data: "Cor1", name: "Cor1", autoWidth: true },
            { data: "Cor2", name: "Cor2", autoWidth: true },
            { data: "Cor3", name: "Cor3", autoWidth: true },
            { data: "Cor4", name: "Cor4", autoWidth: true },
            { data: "Cor5", name: "Cor5", autoWidth: true },
        ],
        rowCallback: function (row, data, index) {

            for (var i = 0; i < Object.keys(data).length; i++) {
                if (i > 0) {
                    $(row).find(`td:eq(${i})`).css('background-color', GetCor(data[Object.keys(data)[i]]));
                    $(row).find(`td:eq(${i})`).css('color', 'white');
                }
            }

        },
        order: [
            [0, "asc"]
        ],
        ajax: {
            url: '/Circulo/GetMomentosSozo',
            datatype: "json",
            data: { EventoId: $("#circulo-eventoid").val() },
            type: "POST"
        }
    };
    $("#table-sozo").DataTable(tableCirculoConfig);
}

$(document).ready(function () {

    $("#Participante").on("keyup", function () {
        var value = $(this).val().toLowerCase();
        $("#table-participantes tr").filter(function () {
            $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1)
        });
    });

    CarregarTabelaCirculo();
    GetParticipantesSemCirculo();
    GetCirculosComParticipantes();
    CarregarTabelaSozo()
});

function PrintSozo() {
    $.ajax({
        url: '/Circulo/GetMomentosSozo',
        data: { EventoId: $("#circulo-eventoid").val() },
        datatype: "json",
        type: "POST",
        success: (result) => {
            var doc = new jsPDF('l', 'mm', "a4");

            var evento = $("#circulo-eventoid option:selected").text()

            if (evento.includes("SVES"))
                logo = "sves";
            else if (evento.includes("SOZO"))
                logo = "sozo";
            else
                logo = "scc";

            var img = new Image();
            img.src = `/Images/logo-${logo}.png`;

            doc.setFontType("normal");
            doc.setFontSize(12);
            doc.addImage(img, 'PNG', 10, 10, 64, 21);
            doc.text(77, 15, $("#circulo-eventoid option:selected").text());



            doc.text(77, 20, `Momentos SOZO`);

            doc.text(77, 25, `Data de Impressão: ${moment().format('DD/MM/YYYY HH:mm')}`);;
            doc.line(10, 38, 290, 38);

            doc.setFontStyle("bold");
            doc.text(12, 43, "Momento");
            doc.text(85, 43, "10:00");
            doc.text(125, 43, "12:00");
            doc.text(165, 43, "16:00");
            doc.text(205, 43, "18:00");
            doc.text(245, 43, "Domingo");
            doc.line(10, 45, 290, 45);
            doc.setFontType("normal");
            height = 50;

            $(result.data).each((index, momento) => {
                doc.setTextColor("#000000")
                doc.text(12, height, momento.Momento);
                doc.setTextColor("#ffffff")
                doc.setFillColor(GetCor(momento.Cor1));
                doc.rect(85, height-5, 45, 8, "F");
                doc.text(87, height, momento.Cor1);
                doc.setFillColor(GetCor(momento.Cor2));
                doc.rect(125, height - 5, 45, 8, "F");
                doc.text(127, height, momento.Cor2);
                doc.setFillColor(GetCor(momento.Cor3));
                doc.rect(165, height - 5, 45, 8, "F");
                doc.text(167, height, momento.Cor3);
                doc.setFillColor(GetCor(momento.Cor4));
                doc.rect(205, height - 5, 45, 8, "F");
                doc.text(207, height, momento.Cor4);
                doc.setFillColor(GetCor(momento.Cor5));
                doc.rect(245, height - 5, 45, 8, "F");
                doc.text(247, height, momento.Cor5);
                height += 6;
            });

            height += -3;
            doc.line(10, height, 290, height);
            doc.setFontStyle("bold");
            doc.text(12, height + 5, "Total:");
            doc.text(24, height + 5, result.data.length.toString());

            PrintDoc(doc);
        }
    });
}

function PrintCirculo(row) {
    $.ajax({
        url: '/Participante/GetParticipantesByCirculo',
        data: { CirculoId: row.Id },
        datatype: "json",
        type: "GET",
        success: (result) => {
            var doc = CriarPDFA4();

            var evento = $("#circulo-eventoid option:selected").text()

            if (evento.includes("SVES"))
                logo = "sves";
            else if (evento.includes("SOZO"))
                logo = "sozo";
            else
                logo = "scc";

            var img = new Image();
            img.src = `/Images/logo-${logo}.png`;

            doc.setFontType("normal");
            doc.setFontSize(12);
            doc.addImage(img, 'PNG', 10, 10, 64, 21);
            doc.text(77, 15, $("#circulo-eventoid option:selected").text());



            if (IsSozo) {
                doc.text(77, 20, `Grupo ${row.Cor}`);
                doc.text(77, 25, `Momento SOZO`);
            }
            else {
                doc.text(77, 20, `Círculo ${row.Cor}`);
                doc.text(77, 25, `${row.Dirigente1} / ${row.Dirigente2}`);
            }
            doc.text(77, 30, `Data de Impressão: ${moment().format('DD/MM/YYYY HH:mm')}`);;
            doc.line(10, 38, 195, 38);

            doc.setFontStyle("bold");
            doc.text(12, 43, "Nome");
            doc.text(117, 43, "Apelido");
            doc.text(152, 43, "Whatsapp");

            doc.line(10, 45, 195, 45);
            doc.setFontType("normal");
            height = 50;

            $(result.data).each((index, participante) => {
                doc.text(12, height, participante.Nome);
                doc.text(117, height, participante.Apelido);
                doc.text(152, height, participante.Fone);
                height += 6;
            });

            AddCount(doc, result.data, height);
            height += 20;
            doc.text(12, height, 'Momentos Sozo');
            height += 8;
            doc.text(12, height, '10:00 - ');
            doc.setFontType("normal");
            doc.text(27, height, row.Momento1);
            height += 6;
            doc.setFontStyle("bold");
            doc.text(12, height, '12:00 - ');
            doc.setFontType("normal");
            doc.text(27, height, row.Momento2);
            height += 6;
            doc.setFontStyle("bold");
            doc.text(12, height, '16:00 - ');
            doc.setFontType("normal");
            doc.text(27, height, row.Momento3);
            height += 6;
            doc.setFontStyle("bold");
            doc.text(12, height, '18:00 - ');
            doc.setFontType("normal");
            doc.text(27, height, row.Momento4);
            height += 6;
            doc.setFontStyle("bold");
            doc.text(12, height, 'Domingo - ');
            doc.setFontType("normal");
            doc.text(35, height, row.Momento5);

            PrintDoc(doc);
        }
    });
}

function GetCirculo(id, cor) {
    if (id > 0) {
        $.ajax({
            url: "/Circulo/GetCirculo/",
            data: { Id: id },
            datatype: "json",
            type: "GET",
            contentType: 'application/json; charset=utf-8',
            success: function (data) {
                $("#circulo-id").val(data.Circulo.Id);

                $('#circulo-cores').append($(`<option value="${data.Circulo.Cor}">${cor}</option>`));
                $("#circulo-cores").val(data.Circulo.Cor).trigger("chosen:updated");

                if (!IsSozo) {
                    $('#circulo-dirigente1').append($(`<option value="${data.Circulo.Dirigente1.Id}">${data.Circulo.Dirigente1.Equipante.Nome}</option>`));
                    $("#circulo-dirigente1").val(data.Circulo.Dirigente1Id).trigger("chosen:updated");

                    $('#circulo-dirigente2').append($(`<option value="${data.Circulo.Dirigente2.Id}">${data.Circulo.Dirigente2.Equipante.Nome}</option>`));
                    $("#circulo-dirigente2").val(data.Circulo.Dirigente2Id).trigger("chosen:updated");
                }

            }
        });
    }
    else {
        $("#circulo-id").val(0);
    }
}

function EditCirculo(row) {
    GetEquipantes();
    GetCores();
    GetCirculo(row.Id, row.Cor);
    $("#modal-circulo").modal();
}


function DeleteCirculo(id) {
    ConfirmMessageDelete().then((result) => {
        if (result) {
            $.ajax({
                url: "/Circulo/DeleteCirculo/",
                datatype: "json",
                type: "POST",
                contentType: 'application/json; charset=utf-8',
                data: JSON.stringify(
                    {
                        Id: id
                    }),
                success: function () {
                    SuccessMesageDelete();
                    CarregarTabelaCirculo();
                }
            });
        }
    });
}

function PostCirculo() {
    if (ValidateForm(`#form-circulo`)) {
        $.ajax({
            url: "/Circulo/PostCirculo/",
            datatype: "json",
            type: "POST",
            contentType: 'application/json; charset=utf-8',
            data: JSON.stringify(
                {
                    Id: $("#circulo-id").val(),
                    EventoId: $("#circulo-eventoid").val(),
                    Dirigente1Id: $("#circulo-dirigente1").val(),
                    Dirigente2Id: $("#circulo-dirigente2").val(),
                    Cor: $("#circulo-cores").val()
                }),
            success: function () {
                SuccessMesageOperation();
                CarregarTabelaCirculo();
                GetCirculosComParticipantes();
                $("#modal-circulo").modal("hide");
            }
        });
    }
}

function DistribuirCirculos() {
    $.ajax({
        url: "/Circulo/DistribuirCirculos/",
        datatype: "json",
        type: "POST",
        contentType: 'application/json; charset=utf-8',
        data: JSON.stringify(
            {
                EventoId: $("#circulo-eventoid").val()
            }),
        success: function () {
            SuccessMesageOperation();
            CarregarTabelaCirculo();
            GetParticipantesSemCirculo();
            GetCirculosComParticipantes();
            $("#modal-circulo").modal("hide");
        }
    });
}


function GetEquipantes(id) {
    $("#circulo-equipantes").empty();

    $.ajax({
        url: "/Circulo/GetEquipantes/",
        data: { EventoId: $("#circulo-eventoid").val() },
        datatype: "json",
        type: "GET",
        contentType: 'application/json; charset=utf-8',
        success: function (data) {
            data.Equipantes.forEach(function (equipante, index, array) {
                $('#circulo-dirigente1').append($(`<option value="${equipante.Id}">${equipante.Nome}</option>`));
                $('#circulo-dirigente2').append($(`<option value="${equipante.Id}">${equipante.Nome}</option>`));
            });
            $("#circulo-dirigente1").val($("#circulo-dirigente1 option:first").val()).trigger("chosen:updated");
            $("#circulo-dirigente2").val($("#circulo-dirigente2 option:eq(1)").val()).trigger("chosen:updated");
            if ($("#circulo-equipantes option").length === 0 && id == 0) {
                ErrorMessage("Não existem Equipantes disponíveis");
                $("#modal-circulo").modal("hide");
            }
        }
    });
}


function GetParticipantesSemCirculo() {
    $("#table-participantes").empty();

    $.ajax({
        url: "/Circulo/GetParticipantesSemCirculo/",
        data: { EventoId: $("#circulo-eventoid").val() },
        datatype: "json",
        type: "GET",
        contentType: 'application/json; charset=utf-8',
        success: function (data) {
            data.Participantes.forEach(function (participante, index, array) {
                $('#table-participantes').append($(`<tr><td class="participante" data-id="${participante.Id}">${participante.Nome}</td></tr>`));
            });

            DragDropg();
        }
    });
}


function GetCirculosComParticipantes() {
    $("#circulos").empty();

    $.ajax({
        url: '/Circulo/GetCirculos',
        datatype: "json",
        data: { EventoId: $("#circulo-eventoid").val() },
        type: "POST",
        success: function (data) {
            data.data.forEach(function (circulo, index, array) {
                if (IsSozo) {
                    htmlCaecalhoCirculo = `<h4 style="padding-top:5px">${circulo.Dirigente1}</h4>
                        <h4 style="padding-bottom:5px">${circulo.Dirigente2}</h4>`
                } else {
                    htmlCaecalhoCirculo = ``
                }

                $("#circulos").append($(`<div data-id="${circulo.Id}" style="margin-bottom:25px;background-color:${GetCor(circulo.Cor)};background-clip: content-box;border-radius: 28px;" class="p-xs col-xs-12 col-lg-4 pg text-center text-white">                     
                       ${htmlCaecalhoCirculo}                        
                                    <table class="table">
                                        <tbody id="pg-${circulo.Id}">
                                            
                                        </tbody>
                                    </table>
 <button type="button" class="btn btn-rounded btn-default print-button" onclick='PrintCirculo(${JSON.stringify(circulo)})'><i class="fa fa-2x fa-print"></i></button>
                                </div>`));
            });

            $.ajax({
                url: "/Circulo/GetCirculosComParticipantes/",
                data: { EventoId: $("#circulo-eventoid").val() },
                datatype: "json",
                type: "GET",
                contentType: 'application/json; charset=utf-8',
                success: function (data) {
                    data.Circulos.forEach(function (circulo, index, array) {
                        $(`#pg-${circulo.CirculoId}`).append($(`<tr><td class="participante" data-id="${circulo.ParticipanteId}">${circulo.Nome}</td></tr>`));
                    });

                    DragDropg();
                }
            });
        }
    });
}

function DragDropg() {
    Drag();

    $('.pg').droppable({
        drop: function (event, ui) {
            $(ui.draggable).parent().remove();
            ChangeCirculo($(ui.draggable).data('id'), $(this).data('id'));
            if ($(this).data('id')) {
                $(`#pg-${$(this).data('id')}`).append($(`<tr><td class="participante" data-id="${$(ui.draggable).data('id')}">${$(ui.draggable).text()}</td></tr>`));
            } else {
                $('#table-participantes').append($(`<tr><td class="participante" data-id="${$(ui.draggable).data('id')}">${$(ui.draggable).text()}</td></tr>`));
            }
            Drag();
        }
    });
}

function Drag() {
    $('.participante').each(function () {
        $(this).css("cursor", "move");
        $(this).draggable({
            zIndex: 999,
            revert: true,
            revertDuration: 0
        });
    });
}

function ChangeCirculo(participanteId, destinoId) {
    $.ajax({
        url: "/Circulo/ChangeCirculo/",
        datatype: "json",
        type: "POST",
        contentType: 'application/json; charset=utf-8',
        data: JSON.stringify(
            {
                ParticipanteId: participanteId,
                DestinoId: destinoId
            }),
        success: function () {
            CarregarTabelaCirculo();
        }
    });
}

function GetCores(id) {
    $("#circulo-cores").empty();

    $.ajax({
        url: "/Circulo/GetCores/",
        data: { EventoId: $("#circulo-eventoid").val() },
        datatype: "json",
        type: "GET",
        contentType: 'application/json; charset=utf-8',
        success: function (data) {
            data.Cores.forEach(function (cor, index, array) {
                $('#circulo-cores').append($(`<option value="${cor.Id}">${cor.Description}</option>`));
            });
            if (id == 0) {
                $("#circulo-cores").val($("#circulo-cores option:first").val()).trigger("chosen:updated");
            } else {
                $("#circulo-cores").trigger("chosen:updated");
            }

            if ($("#circulo-cores option").length === 0 && id == 0) {
                ErrorMessage("Não existem Cores disponíveis");
                $("#modal-circulo").modal("hide");
            }
        }
    });
}