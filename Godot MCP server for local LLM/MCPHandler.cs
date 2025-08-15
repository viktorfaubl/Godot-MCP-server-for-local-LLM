using Godot;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace EchoesofIr.MCP
{
    public partial class MCPHandler : Node
    {

        //add this to your system prompt : 
        private static readonly string mcpSystemPrompt = @"You can use tools to interact with the game world. Available tools:
        open_door: Description: Opens a closed door in the game. Parameters: target (string): The identifier or path of the door to open. Example usage: {""action"":""open_door"",""parameters"": {""target"": ""door_1""}}
        disarm_trap: Description: Attempts to disarm a trap in the game. Parameters: target (string): The identifier or path of the trap to disarm. Example usage: {""action"": ""disarm_trap"",""parameters"": {""target"": ""trap_3""}}
        When you want to perform an in-game action, respond with a JSON object using the exact format in example usage. Only use the defined actions and parameters. Also comment on the action.";

        //USE QueryLLM method to send MCP commands to LLM and get the response.

        public static MCPHandler Instance { get; private set; }

        public delegate Task<MCPResult> MCPActionHandler(Dictionary<string, object> parameters);
        private readonly Dictionary<string, MCPActionHandler> _actions = new();

        public override void _Ready()
        {
            Instance = this;
            RegisterAction("open_door", HandleOpenDoorAsync);
            RegisterAction("disarm_trap", HandleDisarmTrapAsync);
        }

        public void RegisterAction(string action, MCPActionHandler handler)
        {
            _actions[action] = handler;
        }

        public async Task<string> HandleMCPMessageAsync(string json)
        {
            try
            {
                var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;
                if (!root.TryGetProperty("action", out var actionProp))
                    return JsonSerializer.Serialize(new MCPResult { Success = false, Message = "Missing action." });
                string action = actionProp.GetString();
                var parameters = new Dictionary<string, object>();
                if (root.TryGetProperty("parameters", out var paramProp))
                {
                    foreach (var prop in paramProp.EnumerateObject())
                        parameters[prop.Name] = prop.Value.ToString();
                }
                if (_actions.TryGetValue(action, out var handler))
                {
                    var result = await handler(parameters);
                    return JsonSerializer.Serialize(result);
                }
                return JsonSerializer.Serialize(new MCPResult { Success = false, Message = $"Unknown action: {action}" });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new MCPResult { Success = false, Message = ex.Message });
            }
        }

        // Example handler for open_door
        private async Task<MCPResult> HandleOpenDoorAsync(Dictionary<string, object> parameters)
        {
            // TODO: Implement actual game logic to open a door
            string target = parameters.ContainsKey("target") ? parameters["target"].ToString() : null;
            GD.Print($"[MCP] open_door called for {target}");
            // Simulate success
            await Task.Yield();
            return new MCPResult { Success = true, Message = $"Door {target} opened." };
        }

        // Example handler for disarm_trap
        private async Task<MCPResult> HandleDisarmTrapAsync(Dictionary<string, object> parameters)
        {
            // TODO: Implement actual game logic to disarm a trap
            string target = parameters.ContainsKey("target") ? parameters["target"].ToString() : null;
            GD.Print($"[MCP] disarm_trap called for {target}");
            // Simulate success
            await Task.Yield();
            return new MCPResult { Success = true, Message = $"Trap {target} disarmed." };
        }
    }

    public partial class MCPResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}
