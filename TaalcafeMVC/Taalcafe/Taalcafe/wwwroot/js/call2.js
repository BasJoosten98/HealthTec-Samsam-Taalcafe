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
        facingMode: { ideal: "user" }
    }
};

const noVideoConstraints = {
    audio: {
        echoCancellation: true,
        noiseSuppression: true
    }
};

const localVideo = document.getElementById("localVideo");
var localVideoStream = null;

var model = null;
var connections = {}
let askHelp = false;
var earlyIceCandidates = [];

//document.location.pathname + '/connectionhub';
//const hubUrl = 'https://samsam-taalcafe-dev.azurewebsites.net/connectionhub'; //Production-dev
const hubUrl = 'https://samsam-taalcafe.azurewebsites.net/connectionhub'; //Production
//const hubUrl = 'https://taalcafedigitaal.azurewebsites.net/connectionhub'; //Production
//const hubUrl = 'https://localhost:5001/connectionhub'; //Development
//const hubUrl = 'https://localhost:44324/connectionhub'; //Development
var wsConn = new signalR.HubConnectionBuilder()
    .withUrl(hubUrl, { transport: signalR.HttpTransportType.Websockets })
    // Logging levels from most to least:
    // Trace, Debug, Information, Warning, Error, Critical, None
    //.configureLogging(signalR.LogLevel.Trace)
    .withAutomaticReconnect()
    .build();

// ICE server url's (these are ice servers freely provided by Google and Mozilla)
const peerConnCfg = {
    'iceServers': [
        //{'url': 'stun:stun.services.mozilla.com'}, 
        { 'urls': 'stun:stun.l.google.com:19302' },
        { 'urls': 'stun:stun1.l.google.com:19302' },
        { 'urls': 'stun:stun2.l.google.com:19302' },
        { 'urls': 'stun:stun3.l.google.com:19302' },
        { 'urls': 'stun:stun4.l.google.com:19302' }
    ]
};


// Triggers when the page is done loading
$(document).ready(function () {
    document.getElementById("stopCallButton").disabled = true;
    document.getElementById("startCallButton").disabled = true;
    document.getElementById("askHelpButton").disabled = true;

    model = getModel();
    console.log("model obtained: ", model);

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
    console.log("Connecting with SignalR...")
    wsConn.start()
        .then(() => {
            console.log("SignalR: Connected");
            joinConnectionHub();
        })
        .catch(err => console.log("SignalR Error: ", err));
}


// Prompt for local webcam and microphone media streams 
function initializeUserMedia() {
    navigator.mediaDevices.getUserMedia(mediaConstraints)
        .then(stream => {
            localVideoStream = stream;
            localVideo.srcObject = stream;
            //localVideo.play();
            document.getElementById("muteButton").disabled = false;
            document.getElementById("pauseButton").disabled = false;
            console.log("Camera and microphone are connected");
            initializeSignalR();
        })
        .catch(err => {
            navigator.mediaDevices.getUserMedia(noVideoConstraints).then(stream => {
                localVideoStream = stream;
                localVideo.srcObject = stream;
                //localVideo.play();
                document.getElementById("muteButton").disabled = false;
                console.log("Microphone is connected");
                initializeSignalR();
            })
                .catch(error => {
                    console.error("Access to microphone and/or webcam denied.", error);

                });
        });
}

function sendOffer(partnerUserId) {
    console.log("WebRTC: creating and sending OFFER to " + partnerUserId);

    // get a connection for the given partner
    var connection = getConnection(partnerUserId);

    //// add our audio/video stream
    //if (localVideoStream != null) {
    //    connection.addStream(localVideoStream);
    //    console.log("WebRTC: Added local stream to (OFFER) connection");
    //}
    //else {
    //    console.error("WebRTC: No local stream found! OFFER failed!");
    //    return;
    //}

    connection.createOffer({ iceRestart: true }).then(offer => {
        connection.setLocalDescription(offer).then(() => {
            console.log("WebRTC: Added OFFER to connection's localdesciption. Sending OFFER now...", offer);
            sendSignalTo(partnerUserId, JSON.stringify({ "sdp": connection.localDescription }));
        }).catch(err => console.error('WebRTC: Error while setting local description', err));
    }).catch(err => console.error('WebRTC: Error while creating OFFER', err));
}


function getConnection(partnerUserId) {
    if (connections[partnerUserId]) {
        console.log("Found connection for partner ", partnerUserId, connections[partnerUserId]);
        return connections[partnerUserId];
    }
    else {
        return initializeConnection(partnerUserId)
    }
}


// Create the RTCPeerConnection
function initializeConnection(partnerUserId) {

    console.log("Creating new connection for partner... ", partnerUserId);
    var connection = new RTCPeerConnection(peerConnCfg);
    console.log("Created connection for partner ", partnerUserId, connection);

    localVideoStream.getTracks().forEach(track => connection.addTrack(track, localVideoStream));

    connection.onicecandidate = evt => callbackIceCandidate(evt, connection, partnerUserId); // ICE Candidate Callback
    connection.ontrack = evt => callbackTrackAdded(evt, connection, partnerUserId);
    connection.removeTrack = evt => callbackTrackRemoved(evt, connection, partnerUserId);
    connection.oniceconnectionstatechange = evt => callbackIceConnectionStateChanged(evt, connection, partnerUserId);
    connection.onicegatheringstatechange = evt => callbackIceGatheringStateChanged(evt, connection, partnerUserId);


    // connection.iceConnectionState = evt => console.log("WebRTC: iceConnectionState", evt); //not triggering on edge
    // connection.iceGatheringState = evt => console.log("WebRTC: iceGatheringState", evt); //not triggering on edge
    // connection.ondatachannel = evt => console.log("WebRTC: ondatachannel", evt); //not triggering on edge
    // connection.oniceconnectionstatechange = evt => console.log("WebRTC: oniceconnectionstatechange", evt); //triggering on state change 
    // connection.onicegatheringstatechange = evt => console.log("WebRTC: onicegatheringstatechange", evt); //triggering on state change 
    // connection.onsignalingstatechange = evt => console.log("WebRTC: onsignalingstatechange", evt); //triggering on state change 
    // connection.ontrack = evt => console.log("WebRTC: ontrack", evt);
    //connection.onnegotiationneeded = evt => console.log("WebRTC: Negotiation needed.. Event: ", evt); // Negotiation Needed Callback
    //connection.onaddstream = evt => callbackAddStream(connection, evt, partnerUserId); // Add stream handler callback
    //connection.onremovestream = evt => callbackRemoveStream(connection, evt, partnerUserId); // Remove stream handler callback

    connections[partnerUserId] = connection; // Store away the connection based on username
    return connection;
}

function callbackIceGatheringStateChanged(evt, connection, partnerUserId) {
    console.log("ICE GATHERING STATE CHANGED from partner ", partnerUserId, evt, connection.iceConnectionState)
}

function callbackIceConnectionStateChanged(evt, connection, partnerUserId) {
    console.log("ICE CONNENCTION STATE CHANGED from partner ", partnerUserId, evt, connection.iceConnectionState)
}

function callbackTrackAdded(evt, connection, partnerUserId) {
    console.log("A new (video) track will be added from partner " + partnerUserId, evt);
    let videoElement = document.getElementById('Video' + partnerUserId);
    if (videoElement == null) {
        //add video element
        console.log('Creating video element for partner ', partnerUserId);
        document.getElementById("EvaluationBox").hidden = true;
        let elementString = '<div class="col" id="' + partnerUserId + '"><video id="Video' + partnerUserId + '" autoplay width="100%" height:250px;"> </video></div>';
        $('#webcams').prepend(elementString);
        videoElement = document.getElementById('Video' + partnerUserId);
    }
    videoElement.srcObject = event.streams[0];
    console.log("Video element source has been set to ", videoElement.srcObject);
}

function callbackTrackRemoved(evt, connection, partnerUserId){
    console.log("Some track has been removed from partner " + partnerUserId, evt);
    let videoElement = document.getElementById('Video' + partnerUserId);
    if (videoElement != null) {
        let stream = videoElement.srcObject;
        let tracklist = stream.getTracks();
        console.log("Video element source contains ", videoElement.srcObject);
        if (tracklist.length <= 0) {
            console.warn("TrackList is empty for partner ", partnerUserId);
        }
    }
    else {
        console.error("No video element found for partner ", partnerUserId);
    }
}


function callbackIceCandidate(evt, connection, partnerUserId) {
    if (evt.candidate) {
        // Found a new candidate
        console.log('WebRTC: new ICE CANDIDATE found, sending it to partner ' + partnerUserId, evt.candidate);
        sendSignalTo(partnerUserId, JSON.stringify({ "candidate": evt.candidate }));
    } else {
        // null candidate means we are done collecting candidates.
        console.log('WebRTC: ICE CANDIDATE gathering completed');
    }
}


function callbackAddStream(connection, evt, partnerClientId) {
    console.log('WebRTC: called callbackAddStream, attaching media stream from partner ' + partnerClientId);

    // Bind the remote stream to the partner video
    attachMediaStream(evt, partnerClientId);
}


//function callbackRemoveStream(connection, evt, partnerUserId) {
//    console.log('WebRTC: called callbackRemoveStream, removing remote stream from partner ' + partnerUserId);
//    let videoElement = document.getElementById(partnerUserId);
//    if (videoElement != null) {
//        videoElement.remove();
//    }
//}


function joinConnectionHub() {
    if (model.gebruikerId == null) {
        console.warn("Username is empty in model. FAILED ");
    }
    else {
        console.log("SignalR: joining connectionhub...")
        wsConn.invoke("Join", model.gebruikerId).catch(err => {
            console.error("Failed SignalR connection: Not able to connect to signaling server.", err);
        });
    }
}

function sendSignalTo(userId, data) {
    console.log("SignalR: sending data to " + userId, data);
    wsConn.invoke('SendSignal', userId, data).catch(err => {
        console.error("SignalR: something went wrong when sending data signal:", err);
    });
}

function signalReceived(partnerUserId, data) {
    console.log('SignalR: signal received from ' + partnerUserId + ' with data ', data);

    var connection = getConnection(partnerUserId);
    var signal = JSON.parse(data);

    // Route signal based on type
    if (signal.sdp) {
        console.log('WebRTC: signal contains SDP', signal.sdp);
        receivedSdpSignal(connection, partnerUserId, signal.sdp);
    } else if (signal.candidate) {
        console.log('WebRTC: signal contains ICE CANDIDATE', signal.candidate);
        receivedIceCandidateSignal(connection, partnerUserId, signal.candidate);
    } else {
        console.warn("WebRTC: signal contains NOTHING, no further actions needed");
    }
}


function receivedIceCandidateSignal(connection, partnerUserId, candidate) {
    console.log("WebRTC: adding ICE CANDIDATE...");
    connection.addIceCandidate(new RTCIceCandidate(candidate), () => console.log("WebRTC: ICE CANDIDATE has been added", candidate), () => console.warn("WebRTC: ICE CANDIDATE could not be added", candidate));

    //if (connection.remoteDescription != null) {
    //}
    //else {
    //    console.log("WebRTC: ICE CANDIDATE will be added as soon as Remote Description has been set!");
    //    earlyIceCandidates.push({ connection: connection, partnerUserId: partnerUserId, candidate: candidate });
    //}
}

function receivedSdpSignal(connection, partnerUserId, sdp) {
    connection.setRemoteDescription(new RTCSessionDescription(sdp), () => {
        console.log('WebRTC: Remote Description has been set', connection);
        if (connection.remoteDescription.type == "offer") {
            console.log('WebRTC: Remote Description is of type OFFER');
            //connection.addStream(localVideoStream);
            console.log('WebRTC: creating and sending ANSWER to ' + partnerUserId);
            connection.createAnswer().then((desc) => {
                connection.setLocalDescription(desc, () => {
                    console.log("WebRTC: Added ANSWER to connection's localdesciption. Sending ANSWER now...", desc);
                    sendSignalTo(partnerUserId, JSON.stringify({ "sdp": connection.localDescription }));
                }, errorHandler);
            }, errorHandler);
        } else if (connection.remoteDescription.type == "answer") {
            console.log('WebRTC: Remote Description is of type ANSWER');
            console.log("WebRTC: Adding remaining early ice candidates for this connection...", earlyIceCandidates);
            for (let earlyCandidate of earlyIceCandidates) {
                if (earlyCandidate.partnerUserId == partnerUserId) {
                    receivedIceCandidateSignal(connection, partnerUserId, earlyCandidate.candidate);
                }
            }
        }
    }, errorHandler);
}


function closeConnection(partnerUserId) {
    console.log("WebRTC: closing connection with partner " + partnerUserId);
    let connection = connections[partnerUserId];

    connection.ontrack = null;
    connection.onremovetrack = null;
    connection.onicecandidate = null;
    connection.oniceconnectionstatechange = null;
    connection.onicegatheringstatechange = null;

    let videoElement =  = document.getElementById('Video' + partnerUserId);
    if (videoElement != null) {
        if (videoElement.srcObject != null) {
            videoElement.srcObject.getTracks().forEach(track => track.stop());
        }
        videoElement = document.getElementById(partnerUserId); //complete video element
        videoElement.remove();
    }

    if (connection) {
        // Close the connection
        connection.close();
        delete connections[partnerUserId]; // Remove the property
    }

    //callbackRemoveStream(null, null, partnerUserId);

    //if (Object.keys(connections).length === 0) {
    //    document.getElementById("stopCallButton").disabled = true;
    //    document.getElementById("startCallButton").disabled = true;
    //    document.getElementById("askHelpButton").disabled = true;
    //    document.getElementById("EvaluationBox").hidden = false;
    //}
}


function closeAllConnections() {
    console.log("WebRTC: call closeAllConnections ");
    for (let partnerUserId in connections) {
        closeConnection(partnerUserId);
    }
}


// Hub Callback: Call accepted
wsConn.on('JoinedSuccess', () => {
    console.log("SignalR: joining connectionhub was a success!");
    document.getElementById("stopCallButton").disabled = false;
    document.getElementById("startCallButton").disabled = false;
    document.getElementById("askHelpButton").disabled = false;
});

wsConn.on('JoinedFailed', (reason) => {
    console.log("SignalR: joining connectionhub failed! Reason: " + reason);
});

wsConn.on("CallUser", (partner) => {
    if (partner != null) {
        sendOffer(partner.userId);
    }
});

wsConn.on("ReceiveSignal", (partner, data) => {
    if (partner != null && data != null) {
        signalReceived(partner.userId, data);
    }
});

wsConn.on("UserHasLeft", (partner) => {
    if (partner != null) {
        closeConnection(partner.userId);
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

// ask for help or stop asking for help
function toggleHelp() {
    setHelpNeeded(!askHelp);
    wsConn.invoke('AskForHelp');
}
wsConn.on('NeedHelpSetTo', (helpNeeded) => {
    setHelpNeeded(helpNeeded);
});
function setHelpNeeded(helpNeeded) {
    if (!helpNeeded) {
        askHelp = false;
        document.getElementById("askHelpButton").value = "Vraag om hulp";
    }
    else {
        askHelp = true;
        document.getElementById("askHelpButton").value = "Niet meer om hulp vragen";
    }
}

// Close the current calling sessions
function hangup() {
    document.getElementById("stopCallButton").disabled = true;
    document.getElementById("startCallButton").disabled = true;
    document.getElementById("askHelpButton").disabled = true;
    document.getElementById("EvaluationBox").hidden = false;
    wsConn.invoke("Leave");
    closeAllConnections();
}

// Attatch remote mediastream to video element
//function attachMediaStream(e, partnerUserId) {
//    console.log('Attaching video stream from partner ', partnerUserId, e);
//    document.getElementById("EvaluationBox").hidden = true;
//    let elementString = '<div class="col" id="' + partnerUserId + '"><video id="Video' + partnerUserId + '" width="100%" height:250px;"> </video></div>';
//    $('#webcams').prepend(elementString);
//    let videoElement = document.getElementById('Video' + partnerUserId);
//    videoElement.srcObject = e.stream;
//    console.log('Video stream from partner has been set ', e.stream);
//    videoElement.play();
//}


// Mute sound of user microphone
function muteLocalSound() {
    const sPause = document.getElementById("muteButton");

    for (let i in localVideo.srcObject.getAudioTracks()) {
        let track = localVideo.srcObject.getAudioTracks()[i]
        if (track.enabled) {
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

    for (let i in localVideo.srcObject.getVideoTracks()) {
        let track = localVideo.srcObject.getVideoTracks()[i]
        if (track.enabled) {
            vPause.value = "Zet camera aan";
            track.enabled = false;
        }
        else {
            vPause.value = "Zet camera uit";
            track.enabled = true;
        }
    }
}
