using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace SignalRHubs
{
    public class MqttHub : Hub
    {
        public async Task Send(string mqttMessage)
        {
            
        }
    }

   


}
