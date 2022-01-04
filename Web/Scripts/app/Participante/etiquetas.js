function CarregarEtiquetas() {
    if ($('.tipo:checked').val() == 1) {
        if ($("#etiquetas-participantes").val() != "Pesquisar") {
            $("#etiquetas-participantes").val("Pesquisar").trigger("chosen:updated");
        }
        if ($("#etiquetas-circulos").val() != "Pesquisar") {
            $.ajax({
                url: '/Participante/GetParticipantesByCirculo',
                data: { CirculoId: $("#etiquetas-circulos").val() },
                datatype: "json",
                type: "GET",
                success: (result) => {
                    MontarEtiquetasParticipantes(result);
                }
            });
        } else {
            $.ajax({
                url: '/Participante/GetParticipantesConfirmados',
                data: { EventoId: $("#etiquetas-eventoid").val() },
                datatype: "json",
                type: "POST",
                success: (result) => {
                    MontarEtiquetasParticipantes(result);
                }
            });
        }
    } else {
        if ($("#etiquetas-equipantes").val() != "Pesquisar") {
            $("#etiquetas-equipantes").val("Pesquisar").trigger("chosen:updated");
        }
        if ($("#etiquetas-equipes").val() != "Pesquisar") {
            $.ajax({
                url: '/Equipe/GetMembrosEquipe',
                data: {
                    EventoId: $("#etiquetas-eventoid").val(),
                    EquipeId: $("#etiquetas-equipes").val()
                },
                datatype: "json",
                type: "POST",
                success: (result) => {
                    MontarEtiquetasEquipantes(result);
                }
            });
        } else {
            $.ajax({
                url: '/Equipe/GetEquipantesByEvento',
                data: { EventoId: $("#etiquetas-eventoid").val() },
                datatype: "json",
                type: "GET",
                success: (result) => {
                    MontarEtiquetasEquipantes(result);
                }
            });
        }
    }
}

function MontarEtiquetasParticipantes(result) {
    var printDoc = new jsPDF('p', 'mm', 'letter');
    printDoc.setFont('helvetica', "normal")
    printDoc.setFontSize(18);
    $(result.data).each((index, participante) => {
        if (index % 14 == 0 || index == 0) {
            heightApelido = 32;
            heightNome = 42;
            width = 55;
            if (index > 0) {
                printDoc.addPage();
            }
        }
        printDoc.setFont('helvetica', "bold")
        printDoc.setFontSize(18);
        printDoc.text(width, heightApelido, participante.Apelido, 'center');
        printDoc.setFont('helvetica', "normal")
        printDoc.setFontSize(15);
        var splitNome = printDoc.splitTextToSize(participante.Nome, 70);
        printDoc.text(width, heightNome, splitNome, 'center');
        width = index % 2 == 0 ? 162 : 55;
        heightNome = index % 2 == 0 ? heightNome : heightNome + 33.9;
        heightApelido = index % 2 == 0 ? heightApelido : heightApelido + 33.9;
    });
    printDoc.autoPrint();
    window.open(printDoc.output('bloburl'), '_blank');
}

function MontarEtiquetasEquipantes(result) {
    var printDoc = new jsPDF('p', 'mm', 'letter');
    printDoc.setFontType("normal");
    printDoc.setFontSize(18);
    $(result.data).each((index, equipante) => {
        if (index % 14 == 0 || index == 0) {
            heightApelido = 37;
            heightNome = 30;
            heightEquipe = 47;
            width = 55;
            if (index > 0) {
                printDoc.addPage();
            }
        }
        printDoc.setFontType("normal");
        printDoc.setFontSize(13);
        printDoc.text(width, heightEquipe, "  (" + equipante.Equipe + ")", 'center');
        var splitNome = printDoc.splitTextToSize(equipante.Nome, 70);
        printDoc.text(width, heightNome, splitNome, 'center');
        printDoc.setFontType("bold");
        printDoc.setFontSize(16);
        printDoc.text(width, splitNome.length > 1 ? heightApelido + 4 : heightApelido, equipante.Apelido, 'center');
        width = index % 2 == 0 ? 162 : 55;
        heightNome = index % 2 == 0 ? heightNome : heightNome + 33.9;
        heightApelido = index % 2 == 0 ? heightApelido : heightApelido + 33.9;
        heightEquipe = index % 2 == 0 ? heightEquipe : heightEquipe + 33.9;
    });
    printDoc.autoPrint();
    window.open(printDoc.output('bloburl'), '_blank');
}

function CarregarEtiquetaIndividual(position) {
    if ($('.tipo:checked').val() == 1) {
        if ($("#etiquetas-participantes").val() == "Pesquisar") {
            ErrorMessage("Selecione um participante");
        } else {
            $.ajax({
                url: "/Participante/GetParticipante/",
                data: { Id: $("#etiquetas-participantes").val() },
                datatype: "json",
                type: "GET",
                contentType: 'application/json; charset=utf-8',
                success: function (data) {
                    ImprimirIndividual(data, position);
                }
            });
        }
    } else {
        if ($("#etiquetas-equipantes").val() == "Pesquisar") {
            ErrorMessage("Selecione um equipante");
        } else {
            $.ajax({
                url: "/Equipante/GetEquipante/",
                data: { Id: $("#etiquetas-equipantes").val() },
                datatype: "json",
                type: "GET",
                contentType: 'application/json; charset=utf-8',
                success: function (data) {
                    ImprimirIndividual(data, position);
                }
            });
        }
    }
}

function ImprimirIndividual(data, position) {
    var printDoc = new jsPDF('p', 'mm', 'letter');
    printDoc.setFontType("normal");
    printDoc.setFontSize(18);
    width = position % 2 == 0 ? 162 : 55;
    multiplicador = 0;

    switch (position) {
        case 1:
            multiplicador = 0;
            break;
        case 2:
            multiplicador = 0;
            break;
        case 3:
            multiplicador = 1;
            break;
        case 4:
            multiplicador = 1;
            break;
        case 5:
            multiplicador = 2;
            break;
        case 6:
            multiplicador = 2;
            break;
        case 7:
            multiplicador = 3;
            break;
        case 8:
            multiplicador = 3;
            break;
        case 9:
            multiplicador = 3;
            break;
        case 10:
            multiplicador = 4;
            break;
        case 11:
            multiplicador = 5;
            break;
        case 12:
            multiplicador = 5;
            break;
        case 13:
            multiplicador = 6;
            break;
        case 18:
            multiplicador = 6;
            break;
        default:
    }
    if ($('.tipo:checked').val() == 1) {
        var participante = data.Participante;
        heightNome = multiplicador * 33.9;
        heightApelido = multiplicador * 33.9;
        heightNome += 42;
        heightApelido += 32;
        printDoc.setFontType("bold");
        printDoc.setFontSize(18);
        printDoc.text(width, heightApelido, participante.Apelido, 'center');
        printDoc.setFontType("normal");
        printDoc.setFontSize(15);
        var splitNome = printDoc.splitTextToSize(participante.Nome, 70);
        printDoc.text(width, heightNome, splitNome, 'center');

    } else {
        var equipante = data.Equipante;
        heightNome = multiplicador * 33.9;
        heightApelido = multiplicador * 33.9;
        heightEquipe = multiplicador * 33.9;
        heightNome += 30;
        heightApelido += 37;
        heightEquipe += 47;
        printDoc.setFontType("normal");
        printDoc.setFontSize(13);
        printDoc.text(width, heightEquipe, "  (" + equipante.Equipe + ")", 'center');
        var splitNome = printDoc.splitTextToSize(equipante.Nome, 70);
        printDoc.text(width, heightNome, splitNome, 'center');
        printDoc.setFontType("bold");
        printDoc.setFontSize(16);
        printDoc.text(width, splitNome.length > 1 ? heightApelido + 4 : heightApelido, equipante.Apelido, 'center');
    }
    printDoc.autoPrint();
    window.open(printDoc.output('bloburl'), '_blank');
}

function GetParticipantes() {
    $("#etiquetas-participantes").empty();
    $('#etiquetas-participantes').append($('<option>Pesquisar</option>'));
    $.ajax({
        url: '/Participante/GetParticipantes',
        data: { EventoId: $("#etiquetas-eventoid").val() },
        datatype: "json",
        type: "POST",
        success: (result) => {
            result.data.forEach(function (participante, index, array) {
                if (participante.Status != Cancelado)
                    $('#etiquetas-participantes').append($(`<option value="${participante.Id}">${participante.Nome}</option>`));
            });
            $("#etiquetas-participantes").val("Pesquisar").trigger("chosen:updated");
        }
    });
}

function GetEquipantes() {
    $("#etiquetas-equipantes").empty();
    $('#etiquetas-equipantes').append($('<option>Pesquisar</option>'));
    $.ajax({
        url: '/Equipe/GetEquipantesByEvento',
        data: { EventoId: $("#etiquetas-eventoid").val() },
        datatype: "json",
        type: "GET",
        success: (result) => {
            result.data.forEach(function (equipante, index, array) {
                $('#etiquetas-equipantes').append($(`<option value="${equipante.Id}">${equipante.Nome}</option>`));
            });
            $("#etiquetas-equipantes").val("Pesquisar").trigger("chosen:updated");
        }
    });
}

function GetCirculos() {
    $("#etiquetas-circulos").empty();
    $('#etiquetas-circulos').append($('<option>Pesquisar</option>'));
    $.ajax({
        url: '/Circulo/GetCirculos',
        data: { EventoId: $("#etiquetas-eventoid").val() },
        datatype: "json",
        type: "POST",
        success: (result) => {
            result.data.forEach(function (circulo, index, array) {
                $('#etiquetas-circulos').append($(`<option value="${circulo.Id}">${circulo.Cor}</option>`));
            });
            $("#etiquetas-circulos").val("Pesquisar").trigger("chosen:updated");
        }
    });
}

function GetEquipes() {
    $("#etiquetas-equipes").empty();
    $('#etiquetas-equipes').append($('<option>Pesquisar</option>'));
    $.ajax({
        url: '/Equipe/GetEquipes',
        data: { EventoId: $("#etiquetas-eventoid").val() },
        datatype: "json",
        type: "POST",
        success: (result) => {
            result.data.forEach(function (equipe, index, array) {
                $('#etiquetas-equipes').append($(`<option value="${equipe.Id}">${equipe.Equipe}</option>`));
            });
            $("#etiquetas-equipes").val("Pesquisar").trigger("chosen:updated");
        }
    });
}

$(document).ready(function () {
    HideMenu();
    GetAdjacentes();
});

function GetAdjacentes() {
    GetParticipantes();
    GetCirculos();
    GetEquipantes();
    GetEquipes();
}

$('.tipo').on('ifChanged', function (event) {
    var checkboxChecked = $(this).is(':checked');

    if (checkboxChecked && ($(this).val() == 1)) {
        $('#equipante').addClass('d-none');
        $('#participante').removeClass('d-none');
    } else {
        $('#participante').addClass('d-none');
        $('#equipante').removeClass('d-none');
    }
});

function LimparCampos() {
    $("#etiquetas-participantes").val("Pesquisar").trigger("chosen:updated");
    $("#etiquetas-equipantes").val("Pesquisar").trigger("chosen:updated");
}


