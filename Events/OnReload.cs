using Shop.Models;
using Shop.Utils;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;

namespace Shop.Events;

public class OnReload : Event
{
    public override void Disable(TerrariaPlugin plugin)
    {
        GeneralHooks.ReloadEvent -= EventMethod;
    }

    public override void Enable(TerrariaPlugin plugin)
    {
        GeneralHooks.ReloadEvent += EventMethod;
    }

    private void EventMethod(ReloadEventArgs e)
    {
        TSPlayer player = e.Player;
        Structs.MessageResponse response = PluginSettings.Load();
        player.SendMessage(response.Text, response.Color);
    }
}
