using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using onlineshop.Context;
using onlineshop.Models;
using onlineshop.Repositroy;
namespace onlineshop.SignalR
{
    public class ChatHub : Hub
    {
        private readonly UserManager<T_User> _userManager;
        private readonly OnlineShopDb _context;

        public ChatHub(UserManager<T_User> userManager,OnlineShopDb context)
        {
            _userManager = userManager;
            _context = context;
        }

        // This method automatically triggers when a user connects
        public override async Task OnConnectedAsync()
        {
            var user = await _userManager.GetUserAsync(Context.User);
            if (user == null || !Context.User.Identity.IsAuthenticated)
            {
                throw new HubException("User is not authenticated.");
            }

            // Get user role (admin or user)
            var userRole = (await _userManager.GetRolesAsync(user)).FirstOrDefault();

          
            // Store connection in memory (or use a DB for persistence)
            var connection = new T_ChatConnection
            {
                Id = Guid.NewGuid(),
                ConnectionId = Context.ConnectionId,
                UserId = user.Id,
                Role = userRole,
                AssignedAdminId = null
            };

            _context.Add(connection);
          await  _context.SaveChangesAsync();

            // Notify user of successful connection
            if (userRole == "admin")
            {
                await Clients.Caller.SendAsync("ReceiveMessage", "System", "You are connected as an admin. Select a client to start chatting.");
            }
            else
            {
                await Clients.Caller.SendAsync("ReceiveMessage", "System", $"Welcome {user.UserName}.");
                await AssignAdminToClient(user.Id);
            }

            await base.OnConnectedAsync();
        }

        // Assign a client to an available admin
        private async Task AssignAdminToClient(string clientId)
        {
            var availableAdmin = await _context.T_ChatConnection.Where(c => c.Role == "admin").OrderBy(c=> c.Id).ToListAsync();
            if (availableAdmin.Any())
            {
                var lastAssignedAdminId = _context.T_ChatConnection.FirstOrDefault(); // Store the last assigned admin
                var nextAdmin = availableAdmin.FirstOrDefault(a => a.UserId != lastAssignedAdminId.LastAssignedAdminId) ?? availableAdmin.First();

                var clientConnection = _context.T_ChatConnection.FirstOrDefault(c => c.UserId == clientId);
                if (clientConnection != null)
                {
                    clientConnection.AssignedAdminId = nextAdmin.UserId;
                    await _context.SaveChangesAsync();

                    await Clients.Client(nextAdmin.ConnectionId).SendAsync("NewClientConnected", clientId);
                }
            }
            else
            {
                await Clients.Caller.SendAsync("ReceiveMessage", "System", "No admin available.");
            }
        }


        // Send message from admin to client
        public async Task SendMessageToClient(string clientId, string message)
        {
            var admin = await _userManager.GetUserAsync(Context.User);
            if (admin == null || !(await _userManager.IsInRoleAsync(admin, "admin")))
            {
                throw new HubException("Only admins can send messages to clients.");
            }

            var clientConnection = _context.T_ChatConnection.FirstOrDefault(c => c.UserId == clientId);
            if (clientConnection != null)
            {
                await Clients.Client(clientConnection.ConnectionId).SendAsync("ReceiveMessage", "Admin", message);
            }
        }

        // Send message from client to assigned admin
        public async Task SendMessageToAdmin(string message)
        {
            var client = await _userManager.GetUserAsync(Context.User);
            if (client == null)
            {
                throw new HubException("User is not authenticated.");
            }

            var clientConnection = _context.T_ChatConnection.FirstOrDefault(c => c.UserId == client.Id);
            if (clientConnection?.AssignedAdminId != null)
            {
                var adminConnection = _context.T_ChatConnection.FirstOrDefault(c => c.UserId == clientConnection.AssignedAdminId);      
                if (adminConnection != null)
                {
                    await Clients.Client(adminConnection.ConnectionId).SendAsync("ReceiveMessage", client.UserName, message);
                }
            }
        }

        // On disconnection, remove connection from database
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var connection =  _context.T_ChatConnection.FirstOrDefault(c => c.ConnectionId == Context.ConnectionId);
            if (connection != null)
            {
                _context.T_ChatConnection.Remove(connection);
                await _context.SaveChangesAsync();
            }

            await base.OnDisconnectedAsync(exception);
        }

    }
}
