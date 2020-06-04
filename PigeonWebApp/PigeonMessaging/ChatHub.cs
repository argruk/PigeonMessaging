using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using DAL;
using Domain;
using Microsoft.AspNetCore.SignalR;

namespace Hubs
{
    
    public class ChatHub : Hub
    {
        //TODO: Reconnect
        //TODO: Change generic string to Guid
        
        private static readonly HubMapper<string> _connections = new HubMapper<string>();
        private readonly AppDbContext _context;
        
        public ChatHub(AppDbContext context)
        { 
            _context = context;
        }

        /// <summary>
        /// Connection to SignalR event
        /// </summary>
        /// <returns></returns>
        public override Task OnConnectedAsync()
        {
            Context.GetHttpContext().Request.Query.TryGetValue("access_token", out var id);
            
            //Here we should check if the id was found in database. If not - refuse connection
            
            _connections.Add(id,Context.ConnectionId);
            
            Console.WriteLine("--> Connection Opened: " + Context.ConnectionId);
            
            return base.OnConnectedAsync();
        }

        /// <summary>
        /// Disconnect from SignalR event
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public override Task OnDisconnectedAsync(Exception exception)
        {
            Context.GetHttpContext().Request.Query.TryGetValue("access_token", out var id);

            _connections.Remove(id,Context.ConnectionId);
            Console.WriteLine("--> Connection Closed: " + Context.ConnectionId);
            return base.OnDisconnectedAsync(exception);
        }

        /// <summary>
        /// Message sending through SignalR
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task SendMessage(string message)
        {
            var routeOb = JsonSerializer.Deserialize<Message>(message);
            _context.Messages.Add(routeOb);
            
            Console.WriteLine("To: " + routeOb.ToId.ToString());
            Console.WriteLine("Message Received on: " + Context.ConnectionId );
            Console.WriteLine("Message: " + routeOb.MessageBody );

            if(routeOb.ToId.ToString() == string.Empty)
            {
                Console.WriteLine("Broadcast");
                await Clients.All.SendAsync("ReceiveMessage", message);
            }
            else
            {
                string toClient = routeOb.ToId.ToString();
                IEnumerable<string> toClientNoConnId = _connections.GetConnections(toClient);
                Console.WriteLine("Targeted on: " + toClient);

                foreach (var conn in toClientNoConnId)
                {
                    Console.WriteLine("ERROR: "+conn);
                    await Clients.Client(conn).SendAsync("ReceiveMessage", message);

                }
            }
            
            await _context.SaveChangesAsync();
        }
    }
}