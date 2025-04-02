const connection = new signalR.HubConnectionBuilder()
    .withUrl("/chatHub")
    .withAutomaticReconnect([0, 2000, 5000, 10000])
    .configureLogging(signalR.LogLevel.Information)
    .build();

// Listen for new clients connecting
connection.on("NewClientConnected", function (clientId) {
    const clientList = document.getElementById("clientList");
    const newClient = document.createElement("li");
    newClient.innerHTML = `Client ${clientId} <button onclick="selectClient('${clientId}')">Chat</button>`;
    clientList.appendChild(newClient);
});

//  admin selects a client
function selectClient(clientId) {
    document.getElementById("selectedClient").value = clientId;
}



// Listen for incoming messages
connection.on("ReceiveMessage", function (sender, message) {
    const messageList = document.getElementById("messages");
    const newMessage = document.createElement("li");
    newMessage.innerHTML = `<strong>${sender}:</strong> ${message}`;
    messageList.appendChild(newMessage);
});

function sendMessageToClient() {
    const clientId = document.getElementById("selectedClient").value;
    console.log(clientId);
    const message = document.getElementById("messageInput").value;

    connection.invoke("SendMessageToClient", clientId, message);

    document.getElementById("messageInput").value = ""; // Clear input after sending
}


// Auto-reconnect logic
connection.onclose(async () => {
    console.warn("Disconnected! Reconnecting...");
    await startConnection();
});

async function startConnection() {
    try {
        await connection.start();
        console.log("Connected to SignalR");
    } catch (err) {
        console.error("Connection failed, retrying...", err);
        setTimeout(startConnection, 5000);
    }
}


startConnection();