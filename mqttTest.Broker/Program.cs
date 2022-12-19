using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Server;

namespace mqttTest.Broker
{
    internal class Program
    {
        private static int MessageCounter { get; set; }
        
        private static async Task Main(string[] args)
        {
            var brokerIp = IPAddress.Parse("192.168.251.144");
            //var brokerIp = IPAddress.Parse("192.168.0.111");


            // Create the options for our MQTT Broker
            var options = new MqttServerOptionsBuilder()
                // set endpoint to localhost
                .WithDefaultEndpointBoundIPAddress(brokerIp)
                // port used will be 707
                .WithDefaultEndpointPort(1883)
                // handler for new connections
                .WithConnectionValidator(OnNewConnection)
                // handler for new messages
                .WithApplicationMessageInterceptor(OnNewMessage);

            // creates a new mqtt server     
            var mqttServer = new MqttFactory().CreateMqttServer();

            // start the server with options  
            await mqttServer.StartAsync(options.Build());

            // keep application running until user press a key
            Console.ReadLine();
        }

        private static void OnNewConnection(MqttConnectionValidatorContext context)
        {
            Console.WriteLine($"New connection: ClientId = {context.ClientId}, Endpoint = {context.Endpoint}");
        }

        private static void OnNewMessage(MqttApplicationMessageInterceptorContext context)
        {
            var payload = context.ApplicationMessage?.Payload == null
                ? null
                : Encoding.UTF8.GetString(context.ApplicationMessage?.Payload);

            MessageCounter++;

            Console.WriteLine($"MessageId: {MessageCounter} - TimeStamp: {DateTime.Now} -- Message: ClientId = {context.ClientId}, Topic = {context.ApplicationMessage?.Topic}, Payload = {Encoding.UTF8.GetString(context.ApplicationMessage?.Payload)}, QoS = {context.ApplicationMessage?.QualityOfServiceLevel}, Retain-Flag = {context.ApplicationMessage?.Retain}");
        }
    }
}