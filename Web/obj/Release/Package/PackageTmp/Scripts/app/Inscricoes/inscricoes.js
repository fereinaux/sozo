function VerificaCadastro() {
    $.ajax({
        url: "/Inscricoes/VerificaCadastro",
        datatype: "json",
        type: "POST",
        contentType: 'application/json; charset=utf-8',
        data: JSON.stringify({
            Email: $(`#participante-email`).val()
        }),
        success: function (url) {
            if (url) {
                window.location.href = url;
            } else {
                $("#participante-email").prop("disabled", true)
                $(".pnl-cadastro").show();
                $(".pnl-verifica").hide();
                $('.inscricoes.middle-box').height('80%');
                $('.float').css("bottom", "40px")
            }
        }
    })
}

function PostInscricao() {
    if (ValidateForm(`#form-inscricao`)) {
        $.ajax({
            url: "/Inscricoes/PostInscricao/",
            datatype: "json",
            type: "POST",
            contentType: 'application/json; charset=utf-8',
            data: JSON.stringify(
                {
                    Nome: $(`#participante-nome`).val(),
                    Apelido: $(`#participante-apelido`).val(),
                    DataNascimento: moment($("#participante-data-nascimento").val(), 'DD/MM/YYYY', 'pt-br').toJSON(),
                    Email: $(`#participante-email`).val(),
                    Fone: $(`#participante-fone`).val(),
                    HasRestricaoAlimentar: $("input[type=radio][name=participante-hasrestricaoalimentar]:checked").val(),
                    RestricaoAlimentar: $(`#participante-restricaoalimentar`).val(),
                    HasMedicacao: $("input[type=radio][name=participante-hasmedicacao]:checked").val(),
                    Medicacao: $(`#participante-medicacao`).val(),
                    Congregacao: $("input[type=radio][name=participante-congregacao]:checked").val() != "Outra" ? $("input[type=radio][name=participante-congregacao]:checked").val() : $(`#participante-congregacaodescricao`).val(),
                    HasParente: $("input[type=radio][name=participante-hasparente]:checked").val(),
                    Parente: $(`#participante-parente`).val(),
                    HasAlergia: $("input[type=radio][name=participante-hasalergia]:checked").val(),
                    Alergia: $(`#participante-alergia`).val(),
                    Sexo: $("input[type=radio][name=participante-sexo]:checked").val(),
                    NomePai: $(`#participante-nome-pai`).val(),
                    FonePai: $(`#participante-fone-pai`).val(),
                    NomeMae: $(`#participante-nome-mae`).val(),
                    FoneMae: $(`#participante-fone-mae`).val(),
                    NomeConvite: $(`#participante-nome-convite`).val(),
                    FoneConvite: $(`#participante-fone-convite`).val(),
                    Logradouro: $(`#pariticpante-endereco-logradouro`).val(),
                    Complemento: $(`#pariticpante-endereco-complemento`).val(),
                    Bairro: $(`#pariticpante-endereco-bairro`).val()
                }),
            success: function (url) {
                window.location.href = url;
            }
        });
    }
}

$('#trindade').on('ifChecked', function (event) {
    $('.congregacao').addClass('d-none');
    $("#participante-congregacaodescricao").removeClass('required');
});

$('#recon').on('ifChecked', function (event) {
    $('.congregacao').addClass('d-none');
    $("#participante-congregacaodescricao").removeClass('required');
});

$('#outra').on('ifChecked', function (event) {
    $('.congregacao').removeClass('d-none');
    $("#participante-congregacaodescricao").addClass('required');
});


$('#has-medicacao').on('ifChecked', function (event) {
    $('.medicacao').removeClass('d-none');
    $("#participante-medicacao").addClass('required');
});

$('#has-parente').on('ifChecked', function (event) {
    $('.parente').removeClass('d-none');
    $("#participante-parente").addClass('required');
});

$('#not-parente').on('ifChecked', function (event) {
    $('.parente').addClass('d-none');
    $("#participante-parente").removeClass('required');
});

$('#not-medicacao').on('ifChecked', function (event) {
    $('.medicacao').addClass('d-none');
    $("#participante-medicacao").removeClass('required');
});

$('#has-alergia').on('ifChecked', function (event) {
    $('.alergia').removeClass('d-none');
    $("#participante-alergia").addClass('required');
});

$('#not-alergia').on('ifChecked', function (event) {
    $('.alergia').addClass('d-none');
    $("#participante-alergia").removeClass('required');
});

$('#has-restricaoalimentar').on('ifChecked', function (event) {
    $('.restricaoalimentar').removeClass('d-none');
    $("#participante-restricaoalimentar").addClass('required');
});

$('#not-restricaoalimentar').on('ifChecked', function (event) {
    $('.restricaoalimentar').addClass('d-none');
    $("#participante-restricaoalimentar").removeClass('required');
});

$(".pnl-cadastro").hide();
$('.inscricoes.middle-box').height('40%');
$('.float').css("bottom", "25%")