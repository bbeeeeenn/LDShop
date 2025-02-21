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
                    buyprice = 999000000,
                    sellprice = 0,
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

    public static List<ShopItem> GetList()
    {
        List<ShopItem> list = new();
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

    private static void AddItems(List<ShopItem> list, string key, bool condition)
    {
        if (condition)
        {
            foreach (var item in Shop.Items[key])
            {
                list.Add(item);
            }
        }
    }

    public static ItemPlace GetItemPlace(int index)
    {
        int currentPlace = 0;
        foreach (string key in Shop.Items.Keys)
        {
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

    public static int GetItemCount()
    {
        return Shop.Items.Values.Sum(list => list.Count);
    }
}
