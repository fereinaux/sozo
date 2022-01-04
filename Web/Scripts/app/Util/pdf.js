function CriarPDFA4() {
    return new jsPDF('p', 'mm', "a4");
}

function AddCabecalhoEvento(doc, titulo, evento) {
    var logo = "";
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
    doc.addImage(img, 'PNG', 10, 10, 21, 21);
    doc.text(34, 14, evento);
    doc.text(34, 22, titulo);
    doc.text(34, 30, `Data de Impressão: ${moment().format('DD/MM/YYYY HH:mm')}`);

    return doc;
}

function AddCount(doc, data, height) {
    height += -3;
    doc.line(10, height, 195, height);
    doc.setFontStyle("bold");
    doc.text(12, height + 5, "Total:");
    doc.text(24, height + 5, data.length.toString());
}

function PrintDoc(doc) {
    doc.autoPrint();
    window.open(doc.output('bloburl'), '_blank');
}