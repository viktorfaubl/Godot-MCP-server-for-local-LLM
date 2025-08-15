using EchoesofIr.MCP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Godot_MCP_server_for_local_LLM
{
    internal class LLMManager
    {

        private async Task<string> QueryLLM(string prompt, int port)
        {
            string json = prompt;
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await client.PostAsync($"http://localhost:{port}/completion", content);
                var body = await response.Content.ReadAsStringAsync();
                //GD.Print($"[LLM RESPONSE FULL] {body}");
                var trimmedResponse = SentenceTrimmer.TrimToLastCompleteSentence(RemoveJsonObject(ExtractTextFromJson(body)));
                var fullResponse = ExtractJsonObject(ExtractTextFromJson(body));

                // MCP tool usage command handling
                try
                {
                    if (fullResponse != null)
                    {
                        using var doc = JsonDocument.Parse(fullResponse);
                        if (doc.RootElement.TryGetProperty("action", out _))
                        {
                            // It's a tool command, pass to MCPHandler
                            var mcpHandler = MCPHandler.Instance;
                            if (mcpHandler == null)
                                GD.PrintErr("[LLMManager] MCPHandler.Instance is null");
                            else
                            {
                                string resultJson = await mcpHandler.HandleMCPMessageAsync(fullResponse);
                                var result = JsonSerializer.Deserialize<MCPResult>(resultJson);
                                GD.Print($"[LLMManager] Tool result: {result?.Message}");
                                // Append to system chat panel
                                var chatPanel = GetNode<ChatPanel>("/root/DungeonMain/ChatPanel");
                                if (chatPanel != null)
                                {
                                    // Use dynamic to allow calling internal async method
                                    _ = chatPanel.AppendSystemChatMessage(result?.Message ?? "Tool executed.");
                                }
                                return trimmedResponse;
                            }
                        }
                    }
                }
                catch (JsonException) { /* Not a tool command, treat as normal chat */ }

                return trimmedResponse;
            }
            catch (Exception e)
            {
                GD.PrintErr($"[LLM ERROR] {e.Message}");
                return "[LLM ERROR]";
            }
        }
    }
}
