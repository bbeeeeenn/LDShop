using LDShop.Models;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;

namespace LDShop.Events;

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

        ResponseMessage ConfigResponse = PluginSettings.Load();
        player.SendMessage(ConfigResponse.Text, ConfigResponse.Color);
        ResponseMessage ShopItemsResponse = ShopItems.LoadShop();
        player.SendMessage(ShopItemsResponse.Text, ShopItemsResponse.Color);
    }
}
