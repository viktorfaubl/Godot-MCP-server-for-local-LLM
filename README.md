# Godot MCP Server for Local LLM

A sleek C# class library designed to integrate a local **Model Context Protocol (MCP)** server with a local **Large Language Model (LLM)**, tailored for Godot-based games. This library empowers your LLM to seamlessly interact with the game world through structured tool commands, bringing your *Echoes of Ir*-style vision to life.

## Features

- **MCPHandler**: Masterfully routes tool commands from the LLM (e.g., opening doors or disarming traps) to your game logic with precision.
- **LLMManager**: Seamlessly sends prompts to a local LLM server and processes responses, including tool command execution.
- **Extensible Actions**: Effortlessly register new in-game actions for the LLM to wield, expanding your creative control.
- **Godot Integration**: Crafted as a Godot Node, with practical examples leveraging Godot's `print` and node systems for a native feel.

## Getting Started

### Requirements

- **.NET 9.0**: Ensure your development environment is up to date.
- **Godot**: Required for seamless integration (tested with Godot 4.4).
- **Local LLM Server**: Must support a `/completion` endpoint with JSON input/output (e.g., llama.cpp or Ollama with Mistral Nemo).

### Installation

1. **Clone the Repository**: Grab the latest code with `git clone [repository-url]`.
2. **Integrate the Project**: Add the project to your solution or reference the compiled DLL in your Godot C# project.

### Usage

1. **Add MCPHandler**: Instantiate `MCPHandler` as a node in your Godot scene.
2. **Query the LLM**: Use `LLMManager.QueryLLM(prompt, port)` to send prompts to your local LLM server.
3. **Provide Environment Context**: Include the character’s surroundings with your input, e.g., `E:door_456,S:empty,W:trap_987,N:empty`, to guide the LLM’s decisions.
4. **Handle LLM Responses**: The LLM may return tool commands in JSON format, such as:
   {
     "action": "open_door",
     "parameters": { "target": "door_1" }
   }
5.**Route Commands**: MCPHandler directs these commands to your registered handler methods for execution.

## Example System Prompt

Enhance your LLM’s capabilities by adding this to its system prompt:  
		`You can use tools to interact with the game world. Available tools:
        `open_door: Description: Opens a closed door in the game. Parameters: target (string): The identifier or path of the door to open. Example usage: {""action"":""open_door"",""parameters"": {""target"": ""door_1""}}
        `disarm_trap: Description: Attempts to disarm a trap in the game. Parameters: target (string): The identifier or path of the trap to disarm. Example usage: {""action"": ""disarm_trap"",""parameters"": {""target"": ""trap_3""}}
        `When you want to perform an in-game action, respond with a JSON object using the exact format in example usage. Only use the defined actions and parameters. Also comment on the action.`

## Extending the Library

Unlock new possibilities by registering custom actions:
	**MCPHandler.Instance.RegisterAction("your_action", YourHandlerMethod);**


## License
This project is released under the MIT License. Feel free to use, modify, and distribute it, but please include the original copyright and license notice. See LICENSE for details.


## Acknowledgements
Inspired by the retro grit of Eye of the Beholder and powered by the indie spirit of Echoes of Ir. Thanks to the Godot and LocalLLaMA communities for the support!


## Contact
Have questions or ideas? Reach out to viktor.faubl@gmail.com for support or collaboration.

