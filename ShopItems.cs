using LDShop.Models;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Terraria;
using TShockAPI;

namespace LDShop;

public class ShopItems
{
    public struct ItemPlace
    {
        public int index;
        public string key;
    }

    public static ShopItems Shop { get; set; } = new();
    private static readonly string Path = PluginSettings.Config.ShopListPath;
    public Dictionary<string, List<ShopItem>> Items = new()
    {
        {
            "regular",
            new List<ShopItem>()
            {
                new()
                {
                    name = TShock.Utils.GetItemById(Terraria.ID.ItemID.Zenith).Name,
                    netID = Terraria.ID.ItemID.Zenith,
                    amount = -1,
                    prefixID = Terraria.ID.PrefixID.Legendary,
                    requirements = new()
                    {
                        buyprice = 100,
                        items = new() { { Terraria.ID.ItemID.Wood, 10 } },
                    },
                    sellreturn = new()
                    {
                        sellprice = 50,
                        items = new() { { Terraria.ID.ItemID.DirtBlock, 10 } },
                    },
                },
            }
        },
        {
            "postKingSlime",
            new List<ShopItem> { }
        },
        {
            "postEyeOfCthulhu",
            new List<ShopItem> { }
        },
        {
            "postEvilBoss",
            new List<ShopItem> { }
        },
        {
            "postQueenBee",
            new List<ShopItem> { }
        },
        {
            "postSkeletron",
            new List<ShopItem> { }
        },
        {
            "postDeerclops",
            new List<ShopItem> { }
        },
        {
            "postWallOfFlesh",
            new List<ShopItem> { }
        },
        {
            "postQueenSlime",
            new List<ShopItem> { }
        },
        {
            "postDukeFishron",
            new List<ShopItem> { }
        },
        {
            "postEmpressOfLight",
            new List<ShopItem> { }
        },
        {
            "postMech",
            new List<ShopItem> { }
        },
        {
            "postPlantera",
            new List<ShopItem> { }
        },
        {
            "postGolem",
            new List<ShopItem> { }
        },
        {
            "postLunaticCultist",
            new List<ShopItem> { }
        },
        {
            "postMoonlord",
            new List<ShopItem> { }
        },
    };

    public static void SaveShop()
    {
        string ShopJson = JsonConvert.SerializeObject(Shop, Formatting.Indented);
        File.WriteAllText(Path, ShopJson);
    }

    public static ResponseMessage LoadShop()
    {
        if (!File.Exists(Path))
        {
            SaveShop();
            return new()
            {
                Text = "[ShopItems] The file doesn't exist yet. A new one has been created.",
                Color = Color.Yellow,
            };
        }
        else
        {
            try
            {
                string json = File.ReadAllText(Path);
                ShopItems? deserialized = JsonConvert.DeserializeObject<ShopItems>(json);
                if (deserialized == null)
                {
                    return new()
                    {
                        Text =
                            "[ShopItems] Something went wrong. JSON deserialization returned null.",
                        Color = Color.Red,
                    };
                }

                Shop = deserialized;
                return new() { Text = "[ShopItems] Loaded.", Color = Color.LimeGreen };
            }
            catch (Exception ex)
            {
                TShock.Log.ConsoleError($"[ShopItems] {ex.Message}");
                return new()
                {
                    Text = "[ShopItems] Something went wrong. Check logs for more details.",
                };
            }
        }
    }

    public static List<ShopItem> GetUnlockedItemList()
    {
        List<ShopItem> list = new();
        foreach (var (key, value) in Shop.Items)
        {
            if (BossDown(key))
            {
                foreach (var item in value)
                    list.Add(item);
            }
        }
        return list;
    }

    public static ItemPlace GetItemPlace(int index)
    {
        int currentPlace = 0;
        foreach (string key in Shop.Items.Keys)
        {
            if (!BossDown(key))
                continue;
            if (index - currentPlace <= Shop.Items[key].Count)
            {
                return new() { index = index - currentPlace - 1, key = key };
            }
            else
            {
                currentPlace += Shop.Items[key].Count;
            }
        }
        return new() { index = -1, key = "regular" };
    }

    public static bool BossDown(string key)
    {
        return key switch
        {
            "postKingSlime" => NPC.downedSlimeKing,
            "postEyeOfCthulhu" => NPC.downedBoss1,
            "postEvilBoss" => NPC.downedBoss2,
            "postQueenBee" => NPC.downedQueenBee,
            "postSkeletron" => NPC.downedBoss3,
            "postDeerclops" => NPC.downedDeerclops,
            "postWallOfFlesh" => Main.hardMode,
            "postQueenSlime" => NPC.downedQueenSlime,
            "postDukeFishron" => NPC.downedFishron,
            "postEmpressOfLight" => NPC.downedEmpressOfLight,
            "postMech" => NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3,
            "postPlantera" => NPC.downedPlantBoss,
            "postGolem" => NPC.downedGolemBoss,
            "postLunaticCultist" => NPC.downedAncientCultist,
            "postMoonlord" => NPC.downedMoonlord,
            _ => true,
        };
    }
}
