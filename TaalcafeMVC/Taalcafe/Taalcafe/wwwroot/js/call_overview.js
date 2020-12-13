"use strict";

// Zorgen dat dezelfde definitie voor de methodes gebruikt kunnen worden ongeacht de browser
// navigator.getUserMedia = navigator.getUserMedia || navigator.mozGetUserMedia || navigator.webkitGetUserMedia;
window.RTCPeerConnection = window.RTCPeerConnection || window.mozRTCPeerConnection || window.webkitRTCPeerConnection;
window.RTCIceCandidate = window.RTCIceCandidate || window.mozRTCIceCandidate || window.webkitRTCIceCandidate;
window.RTCSessionDescription = window.RTCSessionDescription || window.mozRTCSessionDescription || window.webkitRTCSessionDescription;
window.SpeechRecognition = window.SpeechRecognition || window.webkitSpeechRecognition || window.mozSpeechRecognition 
    || window.msSpeechRecognition || window.oSpeechRecognition;

// Constraints for Webcam and microphone
const mediaConstraints = {
    audio: { 
        echoCancellation: true,
        noiseSuppression: true
    }, 
    video: {
        facingMode: {ideal: "user"}
    }
};

const localVideo = document.getElementById("localVideo");
const remoteVideo1 = document.getElementById("remoteVideo1");
const remoteVideo2 = document.getElementById("remoteVideo2");
var localVideoStream = null;
var myUsername = null;
var availableUsers = null;
var activeCalls = null;
var connections = {}

const hubUrl = 'https://localhost:5001/connectionhub'; //document.location.pathname + '/connectionhub';
let wsConn = new signalR.HubConnectionBuilder()
    .withUrl(hubUrl, {transport: signalR.HttpTransportType.Websockets})
    // Logging levels from most to least:
    // Trace, Debug, Information, Warning, Error, Critical, None
    .configureLogging(signalR.LogLevel.Trace)
    .withAutomaticReconnect()
    .build();

// ICE server url's (these are ice servers freely provided by Google and Mozilla)
const peerConnCfg = {'iceServers': [
    //{'url': 'stun:stun.services.mozilla.com'}, 
    {'url': 'stun:stun.l.google.com:19302'}
]};


// Triggers when the page is done loading
$(document).ready(function () {
    initializeSignalR();

    initializeUserMedia();
});


// Logging errors
const errorHandler = (error) => {
    if (error.message)
        console.error('Error Occurred: ', error.message, error);
    else
        console.error('Error Occurred: ', error);
};


// Initialize communication session with ConnectionHub
function initializeSignalR() {
    wsConn.start()
        .then( () => { 
            console.log("SignalR: Connected");
            //getUsername();
            getActiveCalls();
        })
        .catch(err => console.log(err));
}


// Prompt for local webcam and microphone media streams 
function initializeUserMedia() {
    navigator.mediaDevices.getUserMedia(mediaConstraints)
        .then(stream => {
            localVideoStream = stream;
            localVideo.srcObject = stream;
            localVideo.play();
            document.getElementById("muteButton").disabled = false;
            document.getElementById("pauseButton").disabled = false;
        })
        .catch(err => {
            console.error("Access to microphone and/or webcam denied.", err);
        });
}


// Create initial RTC session offer
function initiateOffer(partnerClientId, stream) {
    console.log('WebRTC: called initiateoffer: ');

    // get a connection for the given partner
    var connection = getConnection(partnerClientId);

    // add our audio/video stream
    connection.addStream(stream);
    console.log("WebRTC: Added local stream");

    connection.createOffer().then(offer => {
        console.log('WebRTC: created Offer: ');
        console.log('WebRTC: Description after offer: ', offer);
        connection.setLocalDescription(offer).then(() => {
            console.log('WebRTC: set Local Description: ');
            console.log('connection before sending offer ', connection);
            setTimeout(() => {
                sendHubSignal(JSON.stringify({ "sdp": connection.localDescription }), partnerClientId);
            }, 1000);
        }).catch(err => console.error('WebRTC: Error while setting local description', err));
    }).catch(err => console.error('WebRTC: Error while creating offer', err));
}


function getConnection(partnerClientId) {
    console.log("WebRTC: called getConnection");
    if (connections[partnerClientId]) {
        console.log("WebRTC: connections partner client exist");
        return connections[partnerClientId];
    }
    else {
        console.log("WebRTC: initialize new connection");
        return initializeConnection(partnerClientId)
    }
}


// Create the RTCPeerConnection
function initializeConnection(partnerClientId) {
    console.log('WebRTC: Initializing connection...');

    var connection = new RTCPeerConnection(peerConnCfg);

    // connection.iceConnectionState = evt => console.log("WebRTC: iceConnectionState", evt); //not triggering on edge
    // connection.iceGatheringState = evt => console.log("WebRTC: iceGatheringState", evt); //not triggering on edge
    // connection.ondatachannel = evt => console.log("WebRTC: ondatachannel", evt); //not triggering on edge
    // connection.oniceconnectionstatechange = evt => console.log("WebRTC: oniceconnectionstatechange", evt); //triggering on state change 
    // connection.onicegatheringstatechange = evt => console.log("WebRTC: onicegatheringstatechange", evt); //triggering on state change 
    // connection.onsignalingstatechange = evt => console.log("WebRTC: onsignalingstatechange", evt); //triggering on state change 
    // connection.ontrack = evt => console.log("WebRTC: ontrack", evt);

    connection.onicecandidate = evt => callbackIceCandidate(evt, connection, partnerClientId); // ICE Candidate Callback
    //connection.onnegotiationneeded = evt => console.log("WebRTC: Negotiation needed.. Event: ", evt); // Negotiation Needed Callback
    connection.onaddstream = evt => callbackAddStream(connection, evt); // Add stream handler callback
    connection.onremovestream = evt => callbackRemoveStream(connection, evt); // Remove stream handler callback

    connections[partnerClientId] = connection; // Store away the connection based on username
    //console.log(connection);
    return connection;
}


function callbackIceCandidate(evt, connection, partnerClientId) {
    console.log("WebRTC: Ice Candidate callback");
    //console.log("evt.candidate: ", evt.candidate);
    if (evt.candidate) {
        // Found a new candidate
        console.log('WebRTC: new ICE candidate');
        //console.log("evt.candidate: ", evt.candidate);
        sendHubSignal(JSON.stringify({ "candidate": evt.candidate }), partnerClientId);
    } else {
        // null candidate means we are done collecting candidates.
        console.log('WebRTC: ICE candidate gathering complete');
        sendHubSignal(JSON.stringify({ "candidate": null }), partnerClientId);
    }
}


function callbackAddStream(connection, evt) {
    console.log('WebRTC: called callbackAddStream');

    // Bind the remote stream to the partner video
    attachMediaStream(evt);
}


function callbackRemoveStream(connection, evt) {
    console.log('WebRTC: removing remote stream from partner window');
    remoteVideo.sourceObject = null;
}


// Ask for update on active calls
function getActiveCalls() {
    console.log("called getActiveCalls");
    wsConn.invoke('getCallList').then( () => {
        console.log("SignalR: Asked for callList update");
    }).catch( (err) => {
        console.error("SignalR: Failed to update active calls:", err);
    });
}


// Sets the username of the client
function setUsername(username) {
    console.log('SignalR: setting username to ' + username + "...");
    myUsername = username;

    // Send the name of this client to the Hub to make it's existence known to other clients
    wsConn.invoke("Join", username).catch(err => {
        console.error("Failed SignalR connection: Not able to connect to signaling server.", err);
    });
}


// TODO get ID/name from current user
function getUsername() {
    generateRandomUsername();
}


// temporary solution for getting and using usernames
function generateRandomUsername() {
    console.log('SignalR: Generating random username...');
    let username = 'User' + Math.floor((Math.random() * 10000) + 1);
    setUsername(username);
}


function getUser(username) {
    for (let i in availableUsers) {
        if (availableUsers[i]["userName"] === username) {
            return availableUsers[i];
        }
    }
    console.error("Couldn't find client:", username, "in list of available users")
}


function sendHubSignal(candidate, partnerClientId) {
    console.log('candidate', candidate);
    console.log('SignalR: called sendhubsignal ');
    wsConn.invoke('sendSignal', candidate, partnerClientId).catch(err => {
            console.error("SignalR: something went wrong when sending signal:", err);
        });
}


// Called upon receiving a signal from the other client via the ConnectionHub
function newSignal(partnerClientId, data) {
    console.log('WebRTC: called newSignal');
    //console.log('connections: ', connections);

    var signal = JSON.parse(data);
    var connection = getConnection(partnerClientId);
    //console.log("signal: ", signal);
    //console.log("signal: ", signal.sdp || signal.candidate);
    //console.log("partnerClientId: ", partnerClientId);
    console.log("connection: ", connection);

    // Route signal based on type
    if (signal.sdp) {
        console.log('WebRTC: sdp signal');
        receivedSdpSignal(connection, partnerClientId, signal.sdp);
    } else if (signal.candidate) {
        console.log('WebRTC: candidate signal');
        receivedCandidateSignal(connection, partnerClientId, signal.candidate);
    } else {
        console.log('WebRTC: adding null candidate');
        connection.addIceCandidate(null).then( () => {
                console.log("WebRTC: added null candidate successfully");
            }).catch( (err) => {
                console.warn("WebRTC: cannot add null candidate:", err);
            });
    }
}


function receivedCandidateSignal(connection, partnerClientId, candidate) {
    //console.log('candidate', candidate);
    console.log('WebRTC: adding full candidate');
    connection.addIceCandidate(new RTCIceCandidate(candidate), () => console.log("WebRTC: added candidate successfully"), () => console.log("WebRTC: cannot add candidate"));
}


function receivedSdpSignal(connection, partnerClientId, sdp) {
    console.log('connection: ', connection);
    console.log('sdp', sdp);
    console.log('WebRTC: called receivedSdpSignal');
    console.log('WebRTC: processing sdp signal');
    connection.setRemoteDescription(new RTCSessionDescription(sdp), () => {
        console.log('WebRTC: set Remote Description');
        if (connection.remoteDescription.type == "offer") {
            console.log('WebRTC: remote Description type offer');
            connection.addStream(localVideoStream);
            console.log('WebRTC: added stream');
            connection.createAnswer().then( (desc) => {
                console.log('WebRTC: create Answer...');
                connection.setLocalDescription(desc, () => {
                    console.log('WebRTC: set Local Description...');
                    console.log('connection.localDescription: ', connection.localDescription);
                    //setTimeout(() => {
                    sendHubSignal(JSON.stringify({ "sdp": connection.localDescription }), partnerClientId);
                    //}, 1000);
                }, errorHandler);
            }, errorHandler);
        } else if (connection.remoteDescription.type == "answer") {
            console.log('WebRTC: remote Description type answer');
        }
    }, errorHandler);
}


function closeConnection(partnerClientId) {
    console.log("WebRTC: called closeConnection ");
    let connection = connections[partnerClientId];

    if (connection) {
        // Close the connection
        connection.close();
        delete connections[partnerClientId]; // Remove the property
    }

    if (Object.keys(connections).length === 0) {
        document.getElementById("stopCallButton").disabled = true;
    }
}


function closeAllConnections() {
    console.log("WebRTC: call closeAllConnections ");
    for (let connectionId in connections) {
        closeConnection(connectionId);
    }
}


// Hub Callback: Call accepted
wsConn.on('callAccepted', (acceptingUser) => {
    console.log('SignalR: call accepted from: ' + JSON.stringify(acceptingUser) + '.  Initiating WebRTC call and offering my stream up...');

    // Callee accepted our call, let's send them an offer with our video stream
    initiateOffer(acceptingUser.connectionId, localVideoStream);
});


// Hub Callback: Call Declined
wsConn.on('callDeclined', (decliningUser, reason) => {
    console.log('SignalR: call declined from: ' + decliningUser["connectionId"]);
    console.log('For reason: ' + reason);

    // toggle buttons
    document.getElementById("stopCallButton").disabled = true;
});


// Hub Callback: Signal from other client
wsConn.on('receiveSignal', (signalingUser, signal) => {
    //console.log('WebRTC: receive signal ');
    //console.log(signalingUser);
    //console.log('NewSignal', signal);
    newSignal(signalingUser.connectionId, signal);
});


// Hub Callback: Incoming Call
wsConn.on('incomingCall', (callingUser) => {
    console.log('SignalR: incoming call from: ' + JSON.stringify(callingUser));
    console.log('Accepting calling session...');

    // Accept the call
    wsConn.invoke('AnswerCall', true, callingUser).catch(err => console.log(err));

    // toggle buttons
    document.getElementById("stopCallButton").disabled = false;
    
    // Decline the call
    //wsConn.invoke('AnswerCall', false, callingUser).catch(err => console.log(err));
});


// Hub Callback: Call Ended
wsConn.on('callEnded', (signalingUser, signal) => {
    //console.log(signalingUser);
    //console.log(signal);

    // Let the user know why the server says the call is over
    console.log('SignalR: call with ' + signalingUser.connectionId + ' has ended: ' + signal);

    // Close the WebRTC connection
    closeConnection(signalingUser.connectionId);
});


// Update the list of known and online users
wsConn.on('updateUserList', (userList) => {
    console.log('SignalR: called updateUserList' + JSON.stringify(userList));
    availableUsers = userList;
});


// Update the list of currently active calls
wsConn.on('updateActiveCalls', (callList) => {
    console.log('SignalR: called updateActiveCalls' + JSON.stringify(callList));
    activeCalls = callList;
    $('#callList li.call').remove();

    $.each(activeCalls, function (index) {
        /* builds the following structure:

        <li class="list-group-item call">
            <div class="container-fluid">
                <div class="row">
                    <div class="col-3">coach</div>
                    <div class="col-3">NN</div>
                    <div class="col-3">Notification</div>
                    <div class="col-3">Participate in call button</div>
                </div>
            </div>
        </li>
        */
        let listString = '<li class="list-group-item call" data=\"\">'

        if (callList[index].help) {
            listString += '<div class="container-fluid alert-warning"><div class="row">';
            listString += '<div class="col-3">' + callList[index].users[0].userName + '</div>';
            listString += '<div class="col-3">' + callList[index].users[1].userName + '</div>';
            listString += '<div class="col-3">Handje omhoog gestoken</div>';
        }
        else {
            listString += '<div class="container-fluid"><div class="row">';
            listString += '<div class="col-3">' + callList[index].users[0].userName + '</div>';
            listString += '<div class="col-3">' + callList[index].users[1].userName + '</div>';
            listString += '<div class="col-3"></div>';
        }
        listString += '<input class="col-3" value="Deelnemen" type="button" onclick="initiateCall(' + index + ')"';
        if (callList[index].users.length > 2) {
            // disable button if a coordinator already is in the call
            listString += ' disabled></input></div></div></li>';
        }
        else {
            listString += '></input></div></div></li>';
        }
        $('#callList').append(listString);
    });

    if ($('#callList li').length === 1) {
        /* builds the following structure:

        <li class="list-group-item call">
            <div class="container-fluid alert-secondary">Er zijn op dit moment geen actieve gesprekken.</div>
        </li>
        */

        //$('#callList').append('<li class="list-group-item call"><div class="container-fluid"><div class="row"><div class="col-12 alert-secondary">Er zijn op dit moment geen actieve gesprekken.</div></div></div></li>');
        $('#callList').append('<li class="list-group-item call"><div class="container-fluid alert-secondary">Er zijn op dit moment geen actieve gesprekken.</div></li>');
    }
});


// Notify the user that the client is trying to reconnect
wsConn.onreconnecting(err => {
    console.assert(connection.state === signalR.HubConnectionState.Reconnecting);
    console.log('SignalR: Connection lost due to error "${err}". Reconnecting.');
});


// Notify the user that the client has been reconnected
wsConn.onreconnected(connectionId => {
    console.assert(connection.state === signalR.HubConnectionState.Connected);
    console.log('SignalR: Connection reestablished. Connected with connectionId "${connectionId}".');
});


// Notify the user that the connection with the Hub has been closed
wsConn.onclose(err => {
    if (err) {
        console.log("SignalR: closed with error:", err);
    }
    else {
        console.log("Disconnected");
    }
});


// Call random available user
function initiateCall(call) {
    call = activeCalls[call];
    if (localVideoStream === null) {
        initializeUserMedia();
        if (localVideoStream === null){
            return;
        }
    }

    if (myUsername === null) {
        getUsername();
    }
    else if (getUser(myUsername).inCall) {
        hangup();
    }

    console.log("Joining call:", call);
    for (let i in call.users) {
        let user = call.users[i];
        if (call.users[i]["inCall"]) {
            console.log("Calling user:", user);
            wsConn.invoke('callUser', user);
        }
    }
    document.getElementById("stopCallButton").disabled = false;
}


// Close the current calling sessions
function hangup() {
    wsConn.invoke('hangUp');
    closeAllConnections();
}


// Attatch remote mediastream to video element
function attachMediaStream(e) {
    console.log(e);

    if (remoteVideo1.srcObject !== e.stream && remoteVideo1.srcObject == null) {
        console.log("attatching mediastream to video 1: ", e.stream);
        remoteVideo1.srcObject = e.stream;
        remoteVideo1.play();
    }
    else if (remoteVideo2.srcObject !== e.stream && remoteVideo2.srcObject == null) {
        console.log("attatching mediastream to video 2: ", e.stream);
        remoteVideo2.srcObject = e.stream;
        remoteVideo2.play();
    }
}


// Mute sound of user microphone
function muteLocalSound() {
    const sPause = document.getElementById("muteButton");

    for(let i in localVideo.srcObject.getAudioTracks()) {
        let track = localVideo.srcObject.getAudioTracks()[i]
        if(track.enabled){
            sPause.value = "Microfoon aanzetten";
            track.enabled = false;
        }
        else {
            sPause.value = "Demp microfoon";
            track.enabled = true;
        }
    }
}


// Stop showing the user webcam recording
function muteLocalVideo() {
    const vPause = document.getElementById("pauseButton");

    for(let i in localVideo.srcObject.getVideoTracks()) {
        let track = localVideo.srcObject.getVideoTracks()[i]
        if(track.enabled){
            vPause.value = "Zet camera aan";
            track.enabled = false;
        }
        else {
            vPause.value = "Zet camera uit";
            track.enabled = true;
        }
    }
}
