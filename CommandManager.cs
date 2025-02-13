using LDShop.Commands;
using LDShop.Models;

namespace LDShop;

public class CommandManager
{
    public static readonly List<Command> Commands = new()
    {
        // Commands
        new Shop(),
    };

    public static void RegisterAll()
    {
        foreach (Command command in Commands)
        {
            TShockAPI.Commands.ChatCommands.Add(command);
        }
    }
}
