"use strict";

const afbeeldingen = document.getElementById("Afbeeldingen");
const vragen = document.getElementById("Vragen");
const vraagBox = document.getElementById("vraagBox");
var vragenLijst = [];
var fileRows = 0;


// Triggers when the page is done loading
$(document).ready(function () {
    // Check wether there already are known questions to add to vragenlijst
    if (vragen.value != "") {
        vragenLijst = vragen.value.split('~')
    }

    if (afbeeldingen.value != "") {
        fileRows = afbeeldingen.value.split(';').length;
    }

    $('.fileInput').change( function() {
        readURL(this);
    });
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


// Request and add the partial view for Files to the form.        
function AddFile() {
    $.ajax({
        async: true,
        data: $('#form').serialize(),
        type: "POST",
        url: '/Thema/AddFile',
        success: function (partialView) {
            //console.log("partialView: " + partialView);
            
            // do some shuffling with the returned elements so that the already existing File inputs don't get their values resest
            $('#HiddenFiles').html(partialView);
            
            // set onInputChanged event
            $('#HiddenFiles .row').eq(fileRows).find('.fileInput')
                .change( function() {
                    readURL(this);
                });

            $('#Files').append($('#HiddenFiles .row').eq(fileRows));
            $('#HiddenFiles').html("");
            fileRows += 1;            
        },
        error: function (reqObj, status, err) {
            console.log(reqObj);
            console.log(reqObj.responseText)
            console.log(status);
            console.log(err);
        }
    });
}

function readURL(input) {
    if (input.files && input.files[0]) {
        var reader = new FileReader();
        
        reader.onload = function (e) {
            $(input).parent().children('.previewImage').attr('src', e.target.result);
            $(input).parent().children('.previewImage').prop('hidden', false);
        }
        
        reader.readAsDataURL(input.files[0]);
    }
}