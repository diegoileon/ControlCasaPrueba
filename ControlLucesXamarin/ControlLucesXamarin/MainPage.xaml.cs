using ControlLucesXamarin.Helpers;
using Microsoft.Azure.EventHubs;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace ControlLucesXamarin
{
	public partial class MainPage : ContentPage
	{

        static string eventHubEntity = "iothubpruebascloud";
        static string ConnectionString = $"Endpoint=sb://iothub-ns-iothubprue-345202-1a0d1c2a0b.servicebus.windows.net/;EntityPath={eventHubEntity};SharedAccessKeyName=iothubowner;SharedAccessKey=Uotv2Pzm/LE1VnC3xLQ+1oEwzWa757UdYadKsJnsJe0=";
        static string partitionId = "1";
        static EventHubClient eventHubClient;

        bool luz12Prendida = false, luz13Prendida = false;

        public MainPage()
		{
			InitializeComponent();

            eventHubClient = EventHubClient.CreateFromConnectionString(ConnectionString);

            ReceiveMessagesFromDeviceAsync(partitionId);
        }

        private async void ReceiveMessagesFromDeviceAsync(string partition)
        {
            var eventHubReceiver = eventHubClient.CreateReceiver("mm", partition, DateTime.UtcNow);
            while (true)
            {
                IEnumerable<EventData> messages = await eventHubReceiver.ReceiveAsync(10);
                if (messages == null) continue;
                foreach (var eventData in messages)
                {
                    var data = Encoding.UTF8.GetString(eventData.Body.Array, eventData.Body.Offset, eventData.Body.Count);

                    var sensorData = DeSerialize(data);

                    lblLight.Text = "Light level: " + sensorData["light"].ToString();
                    lblTemperature.Text = "Temperature: " + sensorData["temperature"].ToString() + "°C";
                }
            }
        }

        private JObject DeSerialize(string data)
        {
            return JObject.Parse(data);
        }

        void TurnOnLight12(object sender, EventArgs e)
        {
            HandleLightStatus("12", ref luz12Prendida);
        }

        void TurnOnLight13(object sender, EventArgs e)
        {
            HandleLightStatus("13", ref luz13Prendida);
        }

        private void HandleLightStatus(string light, ref bool handler)
        {
            if (handler)
                FunctionHelper.SendDataToFunction(light, "0");
            else
                FunctionHelper.SendDataToFunction(light, "1");

            handler = !handler;
        }
    }
}
