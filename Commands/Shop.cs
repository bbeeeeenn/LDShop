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
                    + "/shop sell - Sells the item you are currently holding.",
                Color.Cyan
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
            case "sell":
                ShopSell(player);
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
        string list = string.Join(
            "\n",
            ShopItems
                .GetUnlockedItemList()
                .Select(
                    (item, i) =>
                    {
                        Item _item = TShock.Utils.GetItemById(item.netID);
                        _item.prefix = item.prefixID;

                        return $"- {i + 1} [[i/p{item.prefixID}:{item.netID}]] {_item.HoverName} {(item.amount < 0 ? "" : $"({(item.amount > 0 ? $"[c/42f5a7:{item.amount}]" : $"[c/f54242:0]")}) ")}"
                            + $"[c/dfe62c:Price[]{(item.requirements.buyprice > 0 ? EconomyUtils.BalanceToCoin(item.requirements.buyprice)[1] : "")}{string.Join("", item.requirements.items.Select(e => $"[i/s{e.Value}:{e.Key}]"))}[c/dfe62c:]]"
                            + "  "
                            + $"[c/e6672c:Sell[]{EconomyUtils.BalanceToCoin(item.sellreturn.sellprice)[1]}{string.Join("", item.sellreturn.items.Select(e => $"[i/s{e.Value}:{e.Key}]"))}[c/e6672c:]]";
                    }
                )
        );
        player.SendMessage(
            "[c/2aa351:[SHOP][c/2aa351:] List of items present in the shop]\n" + list,
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

        if (
            !int.TryParse(parameters[1], out int itemIndex)
            || itemIndex > ShopItems.GetUnlockedItemList().Count
            || itemIndex < 1
        )
        {
            player.SendErrorMessage("[SHOP] Invalid item index.");
            return;
        }

        ShopItems.ItemPlace itemPlace = ShopItems.GetItemPlace(itemIndex);
        ShopItem itemFromShop = ShopItems.Shop.Items[itemPlace.key][itemPlace.index];
        Item item = TShock.Utils.GetItemById(itemFromShop.netID);
        item.prefix = itemFromShop.prefixID;
        int quantity = 1;

        if (parameters.Count >= 3 && int.TryParse(parameters[2], out int n) && n > 1)
        // Get wanted quantity
        {
            if (n > 9999)
            {
                n = 9999;
                player.SendInfoMessage(
                    "[SHOP] You can only set the quantity to a maximum of 9999."
                );
            }
            quantity = n;
        }

        if (itemFromShop.amount >= 0) // Skips if the item is infinite
        {
            if (itemFromShop.amount == 0)
            {
                player.SendErrorMessage("[SHOP] This item is out of stock.");
                return;
            }
            else if (itemFromShop.amount < quantity)
            {
                player.SendInfoMessage(
                    $"[SHOP] There's only x{itemFromShop.amount} of [i/p{itemFromShop.prefixID}:{itemFromShop.netID}] left in the shop."
                );
                quantity = itemFromShop.amount;
            }
        }

        long totalCost = itemFromShop.requirements.buyprice * quantity;
        if (totalCost > LDEconomy.Variables.PlayerMoney[player.Account.Name])
        {
            player.SendErrorMessage(
                $"[SHOP] You do not have enough balance to buy x{quantity} [i/p{itemFromShop.prefixID}:{itemFromShop.netID}]."
            );
            return;
        }

        // Check free inventory slots and give item
        int freeSlots = 0;
        bool uniqueItem = false;

        if (item.maxStack == 1)
            uniqueItem = true;
        for (int i = 0; i < NetItem.InventorySlots - 1; i++)
        // Loop through inventory slots
        {
            Item slot = player.TPlayer.inventory[i];
            if (i > 49 && i < 54)
                continue; // Skip coin slots

            if (!uniqueItem)
            {
                if (slot.netID == 0 || slot.netID == item.netID)
                {
                    if (!item.FitsAmmoSlot() && i > 53)
                        continue;
                    if (slot.netID == 0)
                        freeSlots += item.maxStack;
                    else
                        freeSlots += slot.maxStack - slot.stack;
                }
            }
            else
            {
                if (slot.netID == 0 && i < 50)
                {
                    freeSlots++;
                }
            }
        }

        if (freeSlots == 0)
        {
            player.SendErrorMessage("[SHOP] You have no available inventory space for this item.");
            return;
        }
        if (freeSlots < quantity)
        {
            player.SendInfoMessage(
                $"[SHOP] You only have {freeSlots} available inventory slots for this item."
            );
            quantity = freeSlots;
            totalCost = itemFromShop.requirements.buyprice * quantity;
        }

        // Give the item
        if (uniqueItem)
            for (int i = 0; i < quantity; i++)
                player.GiveItem(item.netID, 1, itemFromShop.prefixID);
        else
            player.GiveItem(item.netID, quantity);

        EconomyUtils.TakeMoney(player, totalCost);
        player.SendSuccessMessage(
            $"[SHOP] You successfully bought x{quantity} [i/p{itemFromShop.prefixID}:{itemFromShop.netID}] for {EconomyUtils.BalanceToCoin(totalCost)[1]}."
        );

        itemFromShop.amount -= quantity;
        ShopItems.Shop.Items[itemPlace.key][itemPlace.index] = itemFromShop;
        ShopItems.SaveShop();
    }

    private static void ShopSell(TSPlayer player)
    {
        Item selectedItem = player.SelectedItem;
        ShopItem shopItem = ShopItems
            .GetUnlockedItemList()
            .FirstOrDefault(item => item.netID == selectedItem.netID);

        if (selectedItem.netID == 0)
        {
            player.SendErrorMessage(
                "[SHOP] Please select an item in your hotbar and then type /shop sell."
            );
            return;
        }

        if (shopItem.sellreturn.sellprice == 0)
        {
            player.SendErrorMessage("[SHOP] Sorry, we are not buying that item.");
            return;
        }

        if (shopItem.amount >= 0)
        {
            shopItem.amount += selectedItem.stack;
            foreach (var (key, list) in ShopItems.Shop.Items)
            {
                bool done = false;
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].netID == shopItem.netID)
                    {
                        ShopItems.Shop.Items[key][i] = shopItem;
                        done = true;
                        break;
                    }
                }
                if (done)
                    break;
            }
        }
        ShopItems.SaveShop();

        long totalCost = shopItem.sellreturn.sellprice * selectedItem.stack;
        EconomyUtils.GiveMoney(player, totalCost);
        player.SendSuccessMessage(
            $"[SHOP] You successfully sold x{selectedItem.stack} [i/p{selectedItem.prefix}:{shopItem.netID}] for {EconomyUtils.BalanceToCoin(totalCost)[1]}."
        );

        selectedItem.stack = 0;
        for (byte i = 0; i < NetItem.InventorySlots; i++)
            player.SendData(PacketTypes.PlayerSlot, "", player.Index, i);
    }
}
