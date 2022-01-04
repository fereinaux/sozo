var IsSozo = false;

function HideMenu() {
    $("body").addClass("mini-navbar");
}

$("select[id$='eventoid']").change(ChangeSozo)

function ChangeSozo() {
    IsSozo = $("select[id$='eventoid'] option:selected").text().includes("SOZO");
    if (IsSozo) {
        console.log('teste');
        $('.title-circulo').text("Momentos SOZO");
    }
    else {
        $('.title-circulo').text("Círculos");
    }
}

$(document).ready(function () {
    ChangeSozo();
})

function ValidateForm(form) {
    var formResult = IniciarFormResult();

    formResult = ValidateRequired(form, formResult);
    formResult = ValidateMinLength(form, formResult);
    formResult = ValidateMaxLength(form, formResult);
    formResult = ValidateEmail(form, formResult);
    formResult = ValidateFone(form, formResult);

    var camposObrigatorios = "";

    if (!formResult.IsValid) {
        if (formResult.ErrorsInput != "") {
            camposObrigatorios = `Campos Obrigatórios:${formResult.ErrorsInput}`;
        }
        if (formResult.ErrorsMinLength != "") {
            camposMinLength = `Mínimo de caracteres:${formResult.ErrorsMinLength}`;
        }
        if (formResult.ErrorsMaxLength != "") {
            camposMaxLength = `Máximo de caracteres:${formResult.ErrorsMaxLength}`;
        }
        if (formResult.ErrorsFormatacao != "") {
            camposFormatacao = `Erros de Formatação:${formResult.ErrorsFormatacao}`;
        }
        formResult.MessageError = `${typeof camposObrigatorios === "undefined" ? "" : camposObrigatorios}  
                                   ${typeof camposMinLength === "undefined" ? "" : camposMinLength} 
                                   ${typeof camposMaxLength === "undefined" ? "" : camposMaxLength} 
                                   ${typeof camposFormatacao === "undefined" ? "" : camposFormatacao}`;

        ErrorMessage(formResult.MessageError);

        return false;
    }

    return true;
}

function ValidateRequired(form, formResult) {
    AplicarCssPadrao($(`${form} .required`));

    $(`${form} input.required`).each(function () {
        var input = $(this);
        if (!input.val()) {
            formResult.IsValid = false;
            AplicarCssErro(input);
            formResult.ErrorsInput += AddErro(input.data("field"));
        }
    });

    return formResult;
}

function ValidateMinLength(form, formResult) {
    $(`${form} input`).each(function () {
        var input = $(this);
        if (input.data("min-length") > 0) {
            AplicarCssPadrao(input);
            if (input.val().length < input.data("min-length")) {
                formResult.IsValid = false;
                AplicarCssErro(input);
                formResult.ErrorsMinLength += AddErro(`${input.data("field")} deve ter um mínimo de ${input.data("min-length")} caracteres`);
            }
        }
    });

    return formResult;
}

function ValidateEmail(form, formResult) {
    $(`${form} input[type=email]`).each(function () {
        var input = $(this);
        AplicarCssPadrao(input);
        if (!IsEmail(input.val())) {
            formResult.IsValid = false;
            AplicarCssErro(input);
            formResult.ErrorsFormatacao += AddErro(`${input.data("field")}: exemplo@provedor.com.br`);
        }
    });

    return formResult;
}

function ValidateFone(form, formResult) {
    $(`${form} input.fone`).each(function () {
        var input = $(this);
        AplicarCssPadrao(input);
        if (!IsFone(input.val())) {
            formResult.IsValid = false;
            AplicarCssErro(input);
            formResult.ErrorsFormatacao += AddErro(`${input.data("field")}: +55(81)9.9999-9999`);
        }
    });

    return formResult;
}

function ValidateCPF(form, formResult) {
    $(`${form} input.cpf`).each(function () {
        var input = $(this);
        AplicarCssPadrao(input);
        if (!IsCPF(input.val())) {
            formResult.IsValid = false;
            AplicarCssErro(input);
            formResult.ErrorsFormatacao += AddErro(`${input.data("field")}: 999.999.999-99`);
        }
    });

    return formResult;
}

function ValidateMaxLength(form, formResult) {
    $(`${form} input`).each(function () {
        var input = $(this);
        if (input.data("max-length") > 0) {
            AplicarCssPadrao(input);
            if (input.val().length > input.data("max-length")) {
                formResult.IsValid = false;
                AplicarCssErro(input);
                formResult.ErrorsMaxLength += AddErro(`${input.data("field")} deve ter um máximo de ${input.data("max-length")} caracteres`);
            }
        }
    });

    return formResult;
}

function IniciarFormResult() {
    return FormResult = {
        IsValid: true,
        ErrorsInput: "",
        ErrorsMinLength: "",
        ErrorsMaxLength: "",
        ErrorsFormatacao: "",
        MessageError: ""
    };
}

function AddErro(erro) {
    return `\n - ${erro}`;
}

function AplicarCssPadrao(input) {
    input.css("background-color", "#ffffff");
    input.css("border", "1px solid #e5e6e7");
}

function AplicarCssErro(input) {
    input.css("background-color", "#f4bebe");
    input.css("border", "2px solid #dc0b17");
}

function IsEmail(email) {
    var re = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
    return re.test(String(email).toLowerCase());
}

function IsFone(fone) {
    return fone.indexOf("+55") == 0 && fone.indexOf("(") == 3 && fone.indexOf(")") == 6 && fone.indexOf("9.") == 7 && fone.indexOf("-") == 13 && fone.length == 18;
}

function IsCPF(cpf) {
    return cpf.length == 14;
}