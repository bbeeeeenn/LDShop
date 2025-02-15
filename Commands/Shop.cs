using LDEconomy.Utils;
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
            default:
                break;
        }
    }

    private static void ShopList(TSPlayer player)
    {
        player.SendMessage(
            "[c/2aa351:[SHOP][c/2aa351:] List of items present in the shop]\n"
                + string.Join(
                    "\n",
                    AllShopItems()
                        .Select(
                            (item, i) =>
                                $"- {i + 1} [[i/p{item.prefixID}:{item.netID}]] - {TShock.Utils.GetItemById(item.netID).HoverName} ([c/187335:{(item.amount < 0 ? "+" : item.amount)}]) [c/dfe62c:B]: {EconomyUtils.BalanceToCoin(item.buyprice)[1]} [c/e6672c:S]: {EconomyUtils.BalanceToCoin(item.sellprice)[1]}"
                        )
                ),
            Color.Cyan
        );
    }

    private static List<Models.ShopItem> AllShopItems()
    {
        List<Models.ShopItem> list = new();
        AddItems(list, "regular", true);
        AddItems(list, "postKingSlime", NPC.downedSlimeKing);
        AddItems(list, "postEyeOfCthulhu", NPC.downedBoss1);
        AddItems(list, "postEvilBoss", NPC.downedBoss2);
        AddItems(list, "postQueenBee", NPC.downedQueenBee);
        AddItems(list, "postSkeletron", NPC.downedBoss3);
        AddItems(list, "postDeerclops", NPC.downedDeerclops);
        AddItems(list, "postWallOfFlesh", Main.hardMode);
        AddItems(list, "postQueenSlime", NPC.downedQueenSlime);
        AddItems(list, "postDukeFishron", NPC.downedFishron);
        AddItems(list, "postEmpressOfLight", NPC.downedEmpressOfLight);
        AddItems(
            list,
            "postMech",
            NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3
        );
        AddItems(list, "postPlantera", NPC.downedPlantBoss);
        AddItems(list, "postGolem", NPC.downedGolemBoss);
        AddItems(list, "postLunaticCultist", NPC.downedAncientCultist);
        AddItems(list, "postMoonlord", NPC.downedMoonlord);

        return list;
    }

    private static void AddItems(List<Models.ShopItem> list, string key, bool condition)
    {
        if (condition)
        {
            foreach (var item in PluginSettings.Config.Items[key])
            {
                list.Add(item);
            }
        }
    }
}
