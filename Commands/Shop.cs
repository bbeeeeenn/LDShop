using LDEconomy.Utils;
using LDShop.Models;
using Microsoft.Xna.Framework;
using Terraria;
using TShockAPI;

namespace LDShop.Commands;

public class Shop : Models.Command
{
    public override bool AllowServer => false;
    public override string[] Aliases { get; set; } = { "shop" };
    public override string PermissionNode { get; set; } = "";

    public override void Execute(CommandArgs args)
    {
        TSPlayer player = args.Player;
        if (args.Parameters.Count < 1)
        {
            player.SendMessage(
                "Shop Commands:\n"
                    + "/shop list - Lists all items available in the shop.\n"
                    + "/shop buy <item index/name> [quantity] - Buys an item using its index or name. Example: /shop buy 1.\n"
                    + "/shop sell - Sells the item you are currently holding.\n"
                    + "/shop search [item ID/name] - Searches for an item by ID, name, or the item selected in your hotbar. Displays its price and index.",
                Color.AliceBlue
            );

            return;
        }

        switch (args.Parameters[0])
        {
            case "list":
                ShopList(player);
                break;
            case "buy":
                ShopBuy(player, args.Parameters);
                break;
            default:
                player.SendErrorMessage(
                    "[SHOP] Invalid command. Use /shop for a list of commands."
                );
                break;
        }
    }

    private static void ShopList(TSPlayer player)
    {
        player.SendMessage(
            "[c/2aa351:[SHOP][c/2aa351:] List of items present in the shop]\n"
                + string.Join(
                    "\n",
                    ShopItems
                        .GetList()
                        .Select(
                            (item, i) =>
                            {
                                Item itm = TShock.Utils.GetItemById(item.netID);
                                itm.prefix = item.prefixID;
                                return $"- {i + 1} [[i/p{item.prefixID}:{item.netID}]] - {itm.HoverName} ([c/187335:{(item.amount < 0 ? "+" : item.amount)}]) [c/dfe62c:B]: {EconomyUtils.BalanceToCoin(item.buyprice)[1]} [c/e6672c:S]: {EconomyUtils.BalanceToCoin(item.sellprice)[1]}";
                            }
                        )
                ),
            Color.Cyan
        );
    }

    private static void ShopBuy(TSPlayer player, List<string> parameters)
    {
        if (parameters.Count < 2)
        {
            player.SendErrorMessage(
                "[SHOP] Invalid syntax. Proper syntax: /shop buy <item index/name> [quantity]."
            );
            return;
        }

        var ItemList = ShopItems.GetList();

        if (
            !int.TryParse(parameters[1], out int itemIndex)
            || itemIndex > ItemList.Count
            || itemIndex < 1
        )
        {
            player.SendErrorMessage("[SHOP] Invalid item index.");
            return;
        }

        // var itemPlace =
        var PlayerBanks = LDEconomy.Variables.PlayerMoney;
        ShopItem wantedItem = ItemList[itemIndex - 1];
        Item item = TShock.Utils.GetItemById(wantedItem.netID);
        int quantity = 1;

        if (parameters.Count >= 3 && int.TryParse(parameters[2], out int n) && n > 1)
        {
            quantity = n;
        }

        if (wantedItem.amount == 0)
        {
            player.SendErrorMessage("[SHOP] This item is out of stock.");
            return;
        }
        if (wantedItem.buyprice * quantity > PlayerBanks[player.Account.Name])
        {
            player.SendErrorMessage(
                $"[SHOP] You do not have enough coins to buy {quantity}x [i/p{wantedItem.prefixID}:{wantedItem.netID}]."
            );
            return;
        }
        if (!player.InventorySlotAvailable)
        {
            player.SendErrorMessage("[SHOP] You do not have enough inventory space.");
            return;
        }

        if (item.maxStack == 1)
        {
            for (int i = 0; i < quantity; i++)
            {
                player.GiveItem(wantedItem.netID, 1, wantedItem.prefixID);
            }
        }
        else
        {
            player.GiveItem(wantedItem.netID, quantity);
        }

        EconomyUtils.TakeMoney(player, wantedItem.buyprice * quantity);
        player.SendSuccessMessage(
            $"[SHOP] You successfully bought {quantity}x [i/p{wantedItem.prefixID}:{wantedItem.netID}] for {EconomyUtils.BalanceToCoin(wantedItem.buyprice * quantity)[1]}."
        );
    }
}
