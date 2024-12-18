
////const connection = new signalR.HubConnectionBuilder()
////    .withUrl("/notificationHub")  // URL to your SignalR hub
////    .build();

//////connection.on("ReceiveNotification", function (message) {
//////    console.log("Notification received:", message);
//////    alert(message);  // Or use a custom notification system like toast, etc.
//////});
////connection.on("ReceiveNotification", function (message) {
////    console.log("Notification received:", message);  // Log the message
////    alert(message);  // Show the message in an alert
////});

////connection.start()
////    .then(function () {
////        console.log("SignalR connection established.");
////    })
////    .catch(function (err) {
////        console.error("SignalR connection failed: ", err);
////    });
//// Establish connection to the SignalR hub
//const connection = new signalR.HubConnectionBuilder()
//    .withUrl("/notificationHub")  // URL to your SignalR hub
//    .build();

//// Array to store displayed notifications
//let notifications = [];

//// Function to render notifications on the page
//function renderNotifications() {
//    const notificationContainer = document.getElementById("notification-container");
//    notificationContainer.innerHTML = ""; // Clear the container

//    // Render each notification
//    notifications.forEach((notification) => {
//        const notificationElement = document.createElement("div");
//        notificationElement.textContent = notification;
//        notificationElement.classList.add("notification");

//        // Add a "Remove" button
//        const removeButton = document.createElement("button");
//        removeButton.textContent = "Remove";
//        removeButton.onclick = () => removeNotification(notification);

//        notificationElement.appendChild(removeButton);
//        notificationContainer.appendChild(notificationElement);
//    });
//}

//// Function to remove a notification
//function removeNotification(notification) {
//    notifications = notifications.filter((n) => n !== notification); // Update local list
//    renderNotifications(); // Re-render notifications

//    // Notify the server to remove the notification
//    connection.invoke("RemoveNotification", notification)
//        .catch((err) => console.error("Failed to remove notification:", err));
//}

//// Handle incoming notifications
//connection.on("ReceiveNotification", function (message) {
//    notifications.push(message); // Add to local list
//    renderNotifications(); // Update UI
//});

//// Handle preloaded notifications
//connection.on("LoadNotifications", function (savedNotifications) {
//    notifications = savedNotifications; // Load notifications
//    renderNotifications(); // Update UI
//});

//// Start the connection
//connection.start()
//    .then(function () {
//        console.log("SignalR connection established.");
//    })
//    .catch(function (err) {
//        console.error("SignalR connection failed:", err);
//    });
//x
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/notificationHub")
    .build();

let notifications = [];

// Function to render notifications
function renderNotifications() {
    const notificationContainer = document.getElementById("notification-container");
    notificationContainer.innerHTML = "";

    notifications.forEach((notification) => {
        const notificationElement = document.createElement("div");
        notificationElement.textContent = notification.message;

        const removeButton = document.createElement("button");
        removeButton.textContent = "Remove";
        removeButton.onclick = () => removeNotification(notification.id);

        notificationElement.appendChild(removeButton);
        notificationContainer.appendChild(notificationElement);
    });
}

// Fetch and render notifications on connection
connection.on("LoadNotifications", (savedNotifications) => {
    notifications = savedNotifications;
    renderNotifications();
});

// Handle new notifications
connection.on("ReceiveNotification", (notification) => {
    notifications.push(notification);
    renderNotifications();
});

// Remove notification from UI
connection.on("NotificationRemoved", (notificationId) => {
    notifications = notifications.filter((n) => n.id !== notificationId);
    renderNotifications();
});

// Start the connection
connection.start()
    .then(() => console.log("SignalR connection established."))
    .catch((err) => console.error("SignalR connection failed:", err));

// Remove notification
function removeNotification(notificationId) {
    connection.invoke("RemoveNotification", notificationId)
        .catch((err) => console.error("Failed to remove notification:", err));
}
