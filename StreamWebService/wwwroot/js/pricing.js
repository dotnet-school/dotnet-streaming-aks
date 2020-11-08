// wwwroot/js/pricing.js
"use strict";

var pricingConnection ;

const showMessage = (content) => {
    var li = document.createElement("li");
    li.textContent = content;
    document.getElementById("messagesList").prepend(li);
};

const setStreamingEnabled = status =>
    document.getElementById("startStreaming").disabled = !status;

const setStopStreamingEnabled = status =>
    document.getElementById("cancelStreaming").style.display = status ? "" : "none";

const creatConnection = async () => {
    pricingConnection = new signalR.HubConnectionBuilder()
        .withUrl("/subscribe/infoprice")
        .build();
    try{
        await pricingConnection.start();
        setStreamingEnabled(true);
        showMessage("Conncted with server");
    }catch(err){
        showMessage("Failed to connect to server" + err.toString());
    }
}

document
    .getElementById("cancelStreaming")
    .addEventListener("click", () => {
        pricingConnection.connection.stop();
        setStopStreamingEnabled(false);
        setStreamingEnabled(true);
    });

document
    .getElementById("startStreaming")
    .addEventListener("click", async () => {
        await creatConnection();
        setStreamingEnabled(false);
        setStopStreamingEnabled(true);
        const uic = document.getElementById("uic").value;
        const assetType = document.getElementById("assetType").value;

        pricingConnection.stream("Subscribe", uic, assetType)
            .subscribe({
                next: showMessage,
                complete: () => {
                    showMessage("Stream completed");
                    setStreamingEnabled(true);
                    setStopStreamingEnabled(false);
                },
                error: showMessage,
            });
        event.preventDefault();
    });

creatConnection();
