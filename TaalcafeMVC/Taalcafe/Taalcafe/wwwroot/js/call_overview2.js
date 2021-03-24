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
const hubUrl = 'https://samsam-taalcafe-dev.azurewebsites.net/connectionhub'; //Production-dev
//const hubUrl = 'https://samsam-taalcafe.azurewebsites.net/connectionhub'; //Production
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
        { 'urls': 'stun:stun.nextcloud.com:443' },
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
            document.getElementById("muteButton").disabled = false;
            document.getElementById("pauseButton").disabled = false;
            console.log("Camera and microphone are connected");
            initializeSignalR();
        })
        .catch(err => {
            navigator.mediaDevices.getUserMedia(noVideoConstraints).then(stream => {
                localVideoStream = stream;
                localVideo.srcObject = stream;
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

    // add our audio/video stream
    if (localVideoStream != null) {
        //connection.addStream(localVideoStream); //DEPRECATED
        localVideoStream.getTracks().forEach(track => connection.addTrack(track, localVideoStream));
        console.log("WebRTC: Added local stream to (OFFER) connection");
    }
    else {
        console.error("WebRTC: No local stream found! OFFER failed!");
        return;
    }

    connection.createOffer({ iceRestart: true }).then(offer => {
        connection.setLocalDescription(offer).then(() => {
            console.log("WebRTC: Added OFFER to connection's localdescription. Sending OFFER now...", offer, connection);
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

    connections[partnerUserId] = connection; // Store away the connection based on username
    return connection;
}

function callbackIceGatheringStateChanged(evt, connection, partnerUserId) {
    console.log("WebRTC: ICE GATHERING STATE CHANGED from partner ", partnerUserId, evt, connection.iceConnectionState)
}

function callbackIceConnectionStateChanged(evt, connection, partnerUserId) {
    console.log("WebRTC: ICE CONNENCTION STATE CHANGED from partner ", partnerUserId, evt, connection.iceConnectionState)
    if (connection.iceConnectionState == "connected") {
        console.log("WebRTC: CONNECTION SUCCESS! Video element will now be showed for partner ", partnerUserId);
        //show user video on screen now
        let videoElement = document.getElementById('Video' + partnerUserId);
        if (videoElement != null) {
            videoElement.hidden = false;
            videoElement.disabled = false;
        }
        else {
            console.error("No video element found (while being connected) for partner ", partnerUserId);
        }
    }
    else if (connection.iceConnectionState == "disconnected") {
        //Try to recall user
        let localDesc = connection.localDescription;
        if (localDesc == null) { localDesc = connection.currentLocalDescription; }

        if (localDesc != null) {
            if (localDesc.type == "offer") {
                //This user is the one that started the call! He should recall!
                console.warn("WebRTC: CONNECTION DISCONNECTED, trying to recall partner ", partnerUserId);
                closeConnection(partnerUserId);
                sendOffer(partnerUserId);
                let title = document.getElementById('call_title');
                reconnectTrial++;
                title.innerHTML = "WebRTC reconnection trial #" + reconnectTrial;
                return;
            }
            else {
                console.warn("WebRTC: CONNECTION DISCONNECTED, waiting for recall from partner ", partnerUserId);
                return;
            }
        }
        console.error("WebRTC: CONNECTION DISCONNECTED, but no localdescription found! Unable to handle recall from this side!", partnerUserId, connection);
    }
}

var reconnectTrial = 0;

function callbackTrackAdded(evt, connection, partnerUserId) {
    console.log("A new (video) track will be added from partner " + partnerUserId, evt);
    let videoElement = document.getElementById('Video' + partnerUserId);
    if (videoElement == null) {
        //add video element
        console.log('Creating video element for partner ', partnerUserId);
        let elementString = '<div class="col" id="' + partnerUserId + '"><video id="Video' + partnerUserId + '" autoplay width="100%" height:250px;"> </video></div>';
        $('#webcams').prepend(elementString);
        videoElement = document.getElementById('Video' + partnerUserId);
        videoElement.hidden = true;
        videoElement.disabled = true;
    }
    videoElement.srcObject = event.streams[0];
    console.log("Video element source has been set to ", videoElement.srcObject);
}

function callbackTrackRemoved(evt, connection, partnerUserId) {
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

//THIS WILL LOGIN AS COORDINATOR BAS ACCOUNT
function joinConnectionHub() {
    console.log("SignalR: joining connectionhub...")
    wsConn.invoke("Join", 6).catch(err => {
        console.error("Failed SignalR connection: Not able to connect to signaling server.", err);
    });
    return;

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
    if (connection.remoteDescription != null) {
        console.log("WebRTC: adding ICE CANDIDATE...");
        connection.addIceCandidate(new RTCIceCandidate(candidate), () => console.log("WebRTC: ICE CANDIDATE has been added", candidate), () => console.warn("WebRTC: ICE CANDIDATE could not be added", candidate));
    }
    else {
        console.log("WebRTC: ICE CANDIDATE will be added as soon as Remote Description has been set!");
        earlyIceCandidates.push({ connection: connection, partnerUserId: partnerUserId, candidate: candidate });
    }
}

function receivedSdpSignal(connection, partnerUserId, sdp) {

    let sessionDesc = new RTCSessionDescription(sdp);

    if (connection.remoteDescription != null || connection.currentRemoteDescription != null) {
        //remote description has already been set! Create new connection object for this partner!
        if (sdp.type == "offer") {
            closeConnection(partnerUserId);
            connection = initializeConnection(partnerUserId);
        }
        else {
            console.error("Wrong scernario occurred! Answer has been received while remote description is already set!");
            return;
        }
    }

    connection.setRemoteDescription(sessionDesc, () => {
        console.log('WebRTC: Remote Description has been set', connection);

        addEarlyIceCandidates(connection, partnerUserId);

        if (connection.remoteDescription.type == "offer") {
            console.log('WebRTC: Remote Description is of type OFFER');

            //connection.addStream(localVideoStream); //DEPRECATED
            localVideoStream.getTracks().forEach(track => connection.addTrack(track, localVideoStream));

            console.log('WebRTC: creating and sending ANSWER to ' + partnerUserId);
            connection.createAnswer().then((desc) => {
                connection.setLocalDescription(desc).then(() => {
                    console.log("WebRTC: Added ANSWER to connection's localdescription. Sending ANSWER now...", desc, connection);
                    sendSignalTo(partnerUserId, JSON.stringify({ "sdp": connection.localDescription }));
                }).catch((error) => console.log(error));
            }).catch((error) => console.log(error));

        } else if (connection.remoteDescription.type == "answer") {
            console.log('WebRTC: Remote Description is of type ANSWER');

        }
    }, errorHandler);
}

function addEarlyIceCandidates(connection, partnerUserId) {
    if (connection.remoteDescription != null) {
        console.log("WebRTC: Adding remaining early ice candidates from partner ", partnerUserId, earlyIceCandidates);

        for (let earlyIceCandidate of earlyIceCandidates) {
            if (earlyIceCandidate.partnerUserId == partnerUserId) {
                receivedIceCandidateSignal(earlyIceCandidate.connection, partnerUserId, earlyIceCandidate.candidate);
            }
        }
        earlyIceCandidates = earlyIceCandidates.filter(candidate => candidate.partnerUserId != partnerUserId);
    }
    else {
        console.error("Tried to add early ICE CANDIDATES while Remote Description has not been set yet!");
    }
}

function closeConnection(partnerUserId) {
    console.log("WebRTC: closing connection with partner " + partnerUserId);
    let connection = connections[partnerUserId];

    connection.ontrack = null;
    connection.onremovetrack = null;
    connection.onicecandidate = null;
    connection.oniceconnectionstatechange = null;
    connection.onicegatheringstatechange = null;

    let videoElement = document.getElementById('Video' + partnerUserId);
    if (videoElement != null) {
        if (videoElement.srcObject != null) {
            videoElement.srcObject.getTracks().forEach(track => track.stop());
        }
        videoElement = document.getElementById(partnerUserId); //complete video element
        videoElement.remove();
    }

    earlyIceCandidates = earlyIceCandidates.filter(candidate => candidate.partnerUserId != partnerUserId); //remove early ice candidates for partner

    if (connection) {
        // Close the connection
        connection.close();
        delete connections[partnerUserId]; // Remove the property
    }
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
    //document.getElementById("stopCallButton").disabled = false;
});

wsConn.on('JoinedFailed', (reason) => {
    console.log("SignalR: joining connectionhub failed! Reason: " + reason);
});

wsConn.on("CallUser", (partner) => {
    if (partner != null) {
        sendOffer(partner.userId);
        document.getElementById("stopCallButton").disabled = false;
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

        //Coordinator could be the only one in a group
        //If so, the group has been removed and thus should the button states be changed to normal
        if (Object.keys(connections).length == 0) {
            document.getElementById("stopCallButton").disabled = true;
        }
    }
});


wsConn.on("ReceiveOnlineGroups", (onlineGroups) => {
    console.log("Groups update received", onlineGroups);
    //update UI with onlineGroups
    $('#callList li.call').remove(); //removes current list

    for (let group of onlineGroups) {
        let usersString = "";
        for (let groupUser of group.onlineUsers) {
            usersString += groupUser.name + " ";
        }

        let coordinatorString = "None";
        if (group.coordinator != null) {
            coordinatorString = group.coordinator.name;
        }

        let listString = '<li class="list-group-item call">';

        if (group.needsHelp) {
            listString += '<div class="container-fluid alert-warning"><div class="row">';
            
            listString += '<div class="col-3">' + usersString + '</div>';
            listString += '<div class="col-3">' + coordinatorString + '</div>';
            listString += '<div class="col-3">Handje omhoog gestoken</div>';
        }
        else {
            listString += '<div class="container-fluid"><div class="row">';
            listString += '<div class="col-3">' + usersString + '</div>';
            listString += '<div class="col-3">' + coordinatorString + '</div>';
            listString += '<div class="col-3"></div>';
        }

        listString += '<input class="col-3" value="Deelnemen" type="button" onclick="joinGroup(' + group.groupId + ')"';
        if (group.coordinator != null) {
            // disable button if a coordinator already is in the call
            listString += ' disabled></input></div></div></li>';
        }
        else {
            listString += '></input></div></div></li>';
        }

        $('#callList').append(listString);
    }

    if (onlineGroups.length === 0) {
        /* builds the following structure:
 
        <li class="list-group-item call">
            <div class="container-fluid alert-secondary">Er zijn op dit moment geen actieve gesprekken.</div>
        </li>
        */
        $('#callList').append('<li class="list-group-item call"><div class="container-fluid alert-secondary">Er zijn op dit moment geen actieve gesprekken.</div></li>');
    }
});

function joinGroup(groupId) {
    wsConn.invoke("JoinGroupCallAsCoordinator", groupId).catch(err => {
        console.error("Failed SignalR connection: Not able to connect to signaling server.", err);
    });
}


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

// A coordinator can not see the help symbol in a call
//function toggleHelp() {
//    setHelpNeeded(!askHelp);
//    wsConn.invoke('AskForHelp');
//}
//wsConn.on('NeedHelpSetTo', (helpNeeded) => {
//    //setHelpNeeded(helpNeeded);
//});
//function setHelpNeeded(helpNeeded) {
//    if (!helpNeeded) {
//        askHelp = false;
//        document.getElementById("askHelpButton").value = "Vraag om hulp";
//    }
//    else {
//        askHelp = true;
//        document.getElementById("askHelpButton").value = "Niet meer om hulp vragen";
//    }
//}

// Close the current calling sessions
function hangup() {
    document.getElementById("stopCallButton").disabled = true;
    wsConn.invoke("Leave");
    closeAllConnections();
}

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
