using LDShop.Events;
using LDShop.Models;
using TerrariaApi.Server;

namespace LDShop;

public class EventManager
{
    public static readonly List<Event> events = new()
    {
        // Events
        new OnReload(),
    };

    public static void RegisterAll(TerrariaPlugin plugin)
    {
        foreach (Event _event in events)
        {
            _event.Enable(plugin);
        }
    }

    public static void DeregisterAll(TerrariaPlugin plugin)
    {
        foreach (Event _event in events)
        {
            _event.Disable(plugin);
        }
    }
}
