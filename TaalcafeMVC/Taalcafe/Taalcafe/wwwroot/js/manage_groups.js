"use strict"

let model;
let groups = []; //alle groepen (lijst)
let taalcoaches = []; //alle taalcoaches (lijst)
let cursisten = []; //alle cursisten (lijst)
let tempGroup = []; //tijdelijke groep (voor toevoegen)(lijst)

$(document).ready(function () {
    model = getModel();
    console.log(model);

    divideUsersIntoCorrespondingArrays();

    console.log("Cursisten", cursisten);
    console.log("Taalcoaches", taalcoaches);
    console.log("Groepen", groups);

    renderCursisten();
    renderTaalcoaches();
    renderGroepen();
    renderNieuwGroep();
});

function save() {
    let UsersBar = document.getElementById("Users");
    let allUsers = [];

    cursisten.forEach(user => {
        user.groupName = null;
        allUsers.push(user);
    });
    taalcoaches.forEach(user => {
        user.groupName = null;
        allUsers.push(user);
    });
    tempGroup.forEach(user => {
        user.groupName = null;
        allUsers.push(user);
    });
    groups.forEach(group => {
        group.members.forEach(user => {
            user.groupName = "" + group.name;
            allUsers.push(user);
        });
    });

    let result = allUsers;

    UsersBar.value = JSON.stringify(result);
    console.log("Userbar value: ", UsersBar.value);
    console.log("result value: ", result);

    let btnOpslaan = document.getElementById("btn_opslaan");
    btnOpslaan.click();
}

function divideUsersIntoCorrespondingArrays() {
    model.users.forEach(user => {

        if (user.groupName != null) {
            let group = getGroupByName(user.groupName);
            if (group != null) {
                group.members.push(user);
            }
            else {
                group = { name: user.groupName, members: [] }
                group.members.push(user);
                groups.push(group);
            }
        }
        else if (user.role === "Taalcoach") {
            taalcoaches.push(user);
        }
        else if (user.role === "Cursist") {
            cursisten.push(user);
        }
        else {
            console.warn("User could not be divided into corresponding array", user);
        }
    });
}

function getGroupByName(name) {
    for (var i = 0; i < groups.length; i++) {
        if (groups[i].name == name) { return groups[i]; }
    }
    return null;
}

function addTaalcoach(id) {
    let user = taalcoaches.find(user => user.userId == id);
    tempGroup.push(user);
    taalcoaches = taalcoaches.filter(user => user.userId != id);
    renderNieuwGroep();
    renderTaalcoaches();
}

function addCursist(id) {
    let user = cursisten.find(user => user.userId == id);
    tempGroup.push(user);
    cursisten = cursisten.filter(user => user.userId != id);
    renderNieuwGroep();
    renderCursisten();
}

function removeUserFromTempGroup(id) {
    let user = tempGroup.find(user => user.userId == id);
    tempGroup = tempGroup.filter(user => user.userId != id);
    if (user.role == "Cursist") {
        cursisten.push(user);
        renderCursisten();
    }
    else {
        taalcoaches.push(user);
        renderTaalcoaches();
    }
    renderNieuwGroep();
}

let nextGroupName = 1;
function addGroup() {
    let newGroup = { name: nextGroupName++, members: [] }
    tempGroup.forEach(user => {
        newGroup.members.push(user);
    });
    tempGroup = [];
    groups.push(newGroup);
    renderNieuwGroep();
    renderGroepen();
}

function removeGroup(id) {
    let group = getGroupByName(id);
    group.members.forEach(user => {
        if (user.role === "Taalcoach") {
            taalcoaches.push(user);
        }
        else if (user.role === "Cursist") {
            cursisten.push(user);
        }
    });
    groups = groups.filter(g => g.name != id);
    renderCursisten();
    renderTaalcoaches();
    renderGroepen();
}


function renderCursisten() {
    //<li class="list-group-item">
    //    <div class="row justify-content-center align-items-center">
    //        <div class="col-8">
    //            <p class="m-0">
    //                Henk Fama<br />
    //                <small><i>Cursist</i></small>
    //            </p>
    //        </div>
    //        <div class="col-4">
    //            <button class="btn btn-primary m-auto onclick="addCursist()"">+</button>
    //        </div>
    //    </div>
    //</li>


    let lijst = document.getElementById("cursisten_lijst");
    lijst.innerHTML = "";
    cursisten.forEach(user => {
        let holder = '<li class="list-group-item p-1"><div class="row justify-content-center align-items-center"><div class="col-8"><p class="m-0">' + user.userName + '<br /><small><i>' + user.role + '</i></small></p></div><div class="col-4"><button class="btn btn-primary m-auto" onclick="addCursist(' + "'" + user.userId + "'" + ')">+</button></div></div ></li >';
        lijst.innerHTML += holder;
    });
}


function renderTaalcoaches() {
    //<li class="list-group-item">
    //    <div class="row justify-content-center align-items-center">
    //        <div class="col-8">
    //            <p class="m-0">
    //                Henk Fama<br />
    //                <small><i>Cursist</i></small>
    //            </p>
    //        </div>
    //        <div class="col-4">
    //            <button class="btn btn-primary m-auto onclick="addTaalcoach()"">+</button>
    //        </div>
    //    </div>
    //</li>


    let lijst = document.getElementById("taalcoaches_lijst");
    lijst.innerHTML = "";
    taalcoaches.forEach(user => {
        let holder = '<li class="list-group-item p-1"><div class="row justify-content-center align-items-center"><div class="col-8"><p class="m-0">' + user.userName + '<br /><small><i>' + user.role + '</i></small></p></div><div class="col-4"><button class="btn btn-primary m-auto" onclick="addTaalcoach(' + "'" + user.userId + "'" + ')">+</button></div></div ></li >';
        lijst.innerHTML += holder;
    });
}

function renderNieuwGroep() {
    //<li class="list-group-item">
    //    <div class="row justify-content-center align-items-center">
    //        <div class="col-8">
    //            <p class="m-0">
    //                Henk Fama<br />
    //                <small><i>Cursist</i></small>
    //            </p>
    //        </div>
    //        <div class="col-4">
    //            <button class="btn btn-primary m-auto onclick="removeUserFromTempGroup()"">-</button>
    //        </div>
    //    </div>
    //</li>

    let lijst = document.getElementById("nieuwe_groep_lijst");
    lijst.innerHTML = "";
    tempGroup.forEach(user => {
        let holder = '<li class="list-group-item p-1"><div class="row justify-content-center align-items-center"><div class="col-8"><p class="m-0">' + user.userName + '<br /><small><i>' + user.role + '</i></small></p></div><div class="col-4"><button class="btn btn-danger m-auto" onclick="removeUserFromTempGroup(' + "'" + user.userId + "'" +  ')">-</button></div></div ></li >';
        lijst.innerHTML += holder;
    });
}

function renderGroepen() {
    //<li class="list-group-item">
    //    <div class="row justify-content-center align-items-center">
    //        <div class="col-8">
    //            <ul class="list-group border border-5">
    //                <li class="list-group-item">
    //                    <p class="m-0">
    //                        Sophia Vlemmo<br />
    //                        <small><i>Taalcoach</i></small>
    //                    </p>
    //                </li>
    //            </ul>
    //        </div>
    //        <div class="col-4">
    //            <button class="btn btn-danger m-auto" onclick="removeGroup()">-</button>
    //        </div>
    //    </div>
    //</li>

    let lijst = document.getElementById("groepen_lijst");
    lijst.innerHTML = "";
    groups.forEach(group => {
        let holder = '<li class="list-group-item"><div class="row justify-content-center align-items-center" ><div class="col-8"><ul class="list-group border border-5">';
        group.members.forEach(user => {
            holder += '<li class="list-group-item p-1"><p class="m-0" >' + user.userName + '<br><small><i>' + user.role +'</i></small></p ></li >';
        })
        holder += '</ul></div ><div class="col-4"><button class="btn btn-danger m-auto" onclick="removeGroup(' + "'" + group.name + "'" + ')">-</button></div></div ></li >';
        lijst.innerHTML += holder;
    });
}