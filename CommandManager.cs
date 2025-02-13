using LDShop.Models;

namespace LDShop;

public class CommandManager
{
    public static readonly List<Command> Commands = new()
    {
        // Commands
    };

    public static void RegisterAll()
    {
        foreach (Command command in Commands)
        {
            TShockAPI.Commands.ChatCommands.Add(command);
        }
    }
}
