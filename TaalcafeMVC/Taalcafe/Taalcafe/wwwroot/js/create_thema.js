"use strict";

const vragen = document.getElementById("Vragen");
const vraagBox = document.getElementById("vraagBox");
var vragenLijst = [];


// Triggers when the page is done loading
$(document).ready(function () {
    // Check wether there already are known questions to add to vragenlijst
    if (vragen.value != "") {
        vragenLijst = vragen.value.split('~')
    }
});


// Add the question input in the textbox to the list of questions.
function AddVraag() {
    if (vraagBox.value.includes('~')){
        return;
    }

    if (vragenLijst.length === 0) {
        vragen.value = vraagBox.value;    
    }
    else {
        vragen.value = vragen.value + "~" + vraagBox.value;
    }
    vragenLijst.push(vraagBox.value);
    vraagBox.value = "";

    RenderVragenLijst();
}


// Remove the question with the specified index from the list of questions.
function RemoveVraag(index) {
    vragenLijst.splice(index, 1);

    vragen.value = null;
    for (let i in vragenLijst) {
        vragen.value += "~" + vragenLijst[i];
    }
    vragen.value = vragen.value.substring(1);

    RenderVragenLijst();
}


// clear the <ul> with al the questions and add new <li> elements for all the current questions
function RenderVragenLijst() {
    $('#vragenLijst li.vraag').remove();

    $.each(vragenLijst, function (index) {
        /* creates the following structure to fill the unorderd list with questions:

        <li class="list-group-item vraag">
            <div class="row">
                <div class="col-3">
                    <input class="btn btn-danger" type="button" onclick="RemoveVraag(index)" value="Verwijderen"></input>
                </div>
                <div class="col"> vraag </div>
            </div>
        </li>
        */

        let listString = '<li class="list-group-item vraag"><div class="row"><div class="col-3">'
        listString += '<input class="btn btn-danger" type="button" onclick="RemoveVraag(' + index.toString() + ')" value="Verwijderen"></input></div>';
        listString += '<div class="col">' + vragenLijst[index] + '</div>';
        listString += '</div></li>';

        $('#vragenLijst').append(listString);
    });
}