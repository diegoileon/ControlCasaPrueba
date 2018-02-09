using GHIElectronics.UWP.Shields;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using Windows.Devices.Enumeration;
using Windows.Devices.I2c;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// La plantilla de elemento Página en blanco está documentada en https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0xc0a

namespace ControlCasaLuces
{
    /// <summary>
    /// Página vacía que se puede usar de forma independiente o a la que se puede navegar dentro de un objeto Frame.
    /// </summary>
    /// 
    public sealed partial class MainPage : Page
    {
        static DeviceClient deviceClient;
        private I2cDevice bridgeDevice;
        static string iotHubUri = "iothubpruebascloud.azure-devices.net";
        static string deviceKey = "IZip2TDe5US3Uc8W8SUJByD5JkjuIsPsihDmQRLWVmk=";
        static string deviceId = "devicetest";

        private FEZHAT hat;
        private DispatcherTimer timer;
        private string temperature, light;
        private int messageId = 1;

        public MainPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            deviceClient = DeviceClient.Create(iotHubUri, AuthenticationMethodFactory.CreateAuthenticationWithRegistrySymmetricKey(deviceId, deviceKey), TransportType.Http1);
            Setup();
            IniciateMonitoring();
            ReceiveMessage();
        }

        private async void Setup()
        {
            this.hat = await FEZHAT.CreateAsync();

            this.timer = new DispatcherTimer();
            this.timer.Interval = TimeSpan.FromMilliseconds(1000);
            this.timer.Tick += OnTick;
            this.timer.Start();
        }

        private void OnTick(object sender, object e)
        {
            light = hat.GetLightLevel().ToString("P2");
            temperature = hat.GetTemperature().ToString("N2");

            ctrlLight.LightName = "Light: " + light;
            ctrlTemperature.LightName = "Temperature: " + temperature + "°C";

            messageId++;

            SendDeviceToCloudMessagesAsync(temperature, light, messageId);
        }

        private static async void SendDeviceToCloudMessagesAsync(string temperatureSensor, string lighSensor, int messageId)
        {

            Random rand = new Random();

            var telemetryDataPoint = new
            {
                messageId = messageId++,
                deviceId = deviceId,
                temperature = temperatureSensor,
                light = lighSensor
            };
            var messageString = JsonConvert.SerializeObject(telemetryDataPoint);
            var message = new Message(Encoding.ASCII.GetBytes(messageString));

            await deviceClient.SendEventAsync(message);
        }

        private async void IniciateMonitoring()
        {
            var settings = new I2cConnectionSettings(0x40);
            settings.BusSpeed = I2cBusSpeed.StandardMode;
            string aqs = I2cDevice.GetDeviceSelector();
            IReadOnlyList<DeviceInformation> devices = await DeviceInformation.FindAllAsync(aqs);
            bridgeDevice = await I2cDevice.FromIdAsync(devices[0].Id, settings);
        }

        private async void ReceiveMessage()
        {
            while (true)
            {
                Message receivedMessage = await deviceClient.ReceiveAsync();
                if (receivedMessage == null) continue;

                HandleButtons(Encoding.ASCII.GetString(receivedMessage.GetBytes()));
                await deviceClient.CompleteAsync(receivedMessage);
                bridgeDevice.Write(receivedMessage.GetBytes());
            }
        }

        private void HandleButtons(string receivedMessage)
        {
            string[] lightArray = receivedMessage.Split(',');
            string lightNumber = lightArray[0];
            string lightStatus = lightArray[1];

            bool decision = Convert.ToBoolean(Convert.ToInt32(lightStatus)); ;

            switch (lightNumber)
            {

                case "12":
                    ctrlLed12.IsLightOn = decision;
                    break;
                case "13":
                    ctrlLed13.IsLightOn = decision;
                    break;

                    /*case "01":
                        ctrlBanoAbjInt.IsLightOn = decision;
                        break;
                    case "02":
                        ctrlBanoArrInt.IsLightOn = decision;
                        break;
                    case "03":
                        ctrlSala.IsLightOn = decision;
                        break;
                    case "04":
                        ctrlOscar.IsLightOn = decision;
                        break;
                    case "05":
                        ctrlBanoArrExt.IsLightOn = decision;
                        break;
                    case "06":
                        ctrlBanoAbjExt.IsLightOn = decision;
                        break;
                    case "07":
                        ctrlGimnasio.IsLightOn = decision;
                        break;
                    case "08":
                        ctrlBanoAbjInt.IsLightOn = decision;
                        break;
                    case "09":
                        ctrlVentilador.IsLightOn = decision;
                        break;
                    case "10":
                        ctrlTaller.IsLightOn = decision;
                        break;
                    case "11":
                        ctrlJuegos.IsLightOn = decision;
                        break;
                    case "12":
                        ctrlPatio.IsLightOn = decision;
                        break;*/
            }
        }
    }
}
