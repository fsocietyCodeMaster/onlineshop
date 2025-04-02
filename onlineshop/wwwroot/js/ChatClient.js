const connection = new signalR.HubConnectionBuilder()
    .withUrl("/chatHub")
    .withAutomaticReconnect([0, 2000, 5000, 10000])
    .configureLogging(signalR.LogLevel.Information)
    .build();

// Listen for messages from the admin
connection.on("ReceiveMessage", function (sender, message) {
    const messageList = document.getElementById("messages");
    const newMessage = document.createElement("li");
    newMessage.innerHTML = `<strong>${sender}:</strong> ${message}`;
    messageList.appendChild(newMessage);
});


// Send message to admin
function sendMessageToAdmin() {
    const message = document.getElementById("messageInput").value;
    connection.invoke("SendMessageToAdmin", message);
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
