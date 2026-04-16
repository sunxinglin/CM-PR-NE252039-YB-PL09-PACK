import * as signalR from '@aspnet/signalr'
var connection = new signalR.HubConnectionBuilder().withUrl("/productionHub").build();
//var connection = new signalR.HubConnectionBuilder().withUrl("http://139.9.233.203:22222/productionHub").build();

export function startNewConnection(type) {
	if (connection.connectionState == 0) {
		connection.start().then(function() {

			console.log('signalr success');
		}).catch(function(err) {

			console.log('signalr error:' + err);
		});
		connection.on("showAlarmMsg", function(output) {
			
			
			if (typeof window !== "undefined" && typeof window.RefreshdAlarmMsg === "function") {
				window.RefreshdAlarmMsg(output)
			}
			
		});
		connection.on("showMsg", function(output) {
			
			if (typeof window !== "undefined" && typeof window.RefreshMsg === "function") {
				window.RefreshMsg(output)
			}
			
		});
		
		if (type == 'duiduoji') {
			
			
			connection.on("receiveMBStockerMsg1", function(output) {
				
				if (typeof window !== "undefined" && typeof window.RefreshPlcMsg === "function") {
					window.RefreshPlcMsg(output, 1)
				}
				
			});
			connection.on("receiveMBStockerMsg2", function(output) {
				
				if (typeof window !== "undefined" && typeof window.RefreshPlcMsg === "function") {
					window.RefreshPlcMsg(output, 2)
				}
				
			});
			connection.on("receiveMBStockerMsg3", function(output) {
				
				if (typeof window !== "undefined" && typeof window.RefreshPlcMsg === "function") {
					window.RefreshPlcMsg(output, 1)
				}
				
			});

		}


	}
}



export function showProductionMsg(msg) {


}
