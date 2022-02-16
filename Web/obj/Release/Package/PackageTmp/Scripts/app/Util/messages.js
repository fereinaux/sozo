function ErrorMessage(message) {
    swal({
        title: "Erro!",
        icon: "error",
        text: message
    });
}

function SuccessMessage(message) {
    swal({
        title: "Sucesso!",
        icon: "success",
        text: message
    });
}

function SuccessMesageDelete() {
    SuccessMessage("Registro excluído!");
}

function SuccessMesageOperation() {
    SuccessMessage("A operação foi concluída!");
}

async function ConfirmMessage(message) {
    return await swal({
        title: "Você tem certeza?",
        icon: "warning",
        text: message,
        buttons: {
            cancel: "Desistir",
            confirm: "Confirmar"
        }
    });
}

async function ConfirmMessageDelete() {
    return await ConfirmMessage("Essa ação excluirá permanentemente o registro, deseja continuar?");
}

async function ConfirmMessageCancelar(nome) {
    return await ConfirmMessage(`Deseja cancelar a inscrição de ${nome}?`);
}

function RebciboPagamento(valor, formaPagamento, evento) {
    return `Aqui está o seu recibo de pagamento do Sozo de Vida no Espírito Santo:/n/n*R$ ${valor} - ${formaPagamento}*${RodapeEvento(evento)}`;
}

function RodapeEvento(evento) {
    return `/n/n *Equipe Sozo*`;
}