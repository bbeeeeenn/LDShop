using LDShop.Models;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using TShockAPI;

namespace LDShop;

public class ShopItems
{
    private static readonly string Path = PluginSettings.Config.ShopListPath;
    public Dictionary<string, List<Models.ShopItem>> Items = new()
    {
        {
            "regular",
            new List<Models.ShopItem>()
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
            new List<Models.ShopItem> { }
        },
        {
            "postEyeOfCthulhu",
            new List<Models.ShopItem> { }
        },
        {
            "postEvilBoss",
            new List<Models.ShopItem> { }
        },
        {
            "postQueenBee",
            new List<Models.ShopItem> { }
        },
        {
            "postSkeletron",
            new List<Models.ShopItem> { }
        },
        {
            "postDeerclops",
            new List<Models.ShopItem> { }
        },
        {
            "postWallOfFlesh",
            new List<Models.ShopItem> { }
        },
        {
            "postQueenSlime",
            new List<Models.ShopItem> { }
        },
        {
            "postDukeFishron",
            new List<Models.ShopItem> { }
        },
        {
            "postEmpressOfLight",
            new List<Models.ShopItem> { }
        },
        {
            "postMech",
            new List<Models.ShopItem> { }
        },
        {
            "postPlantera",
            new List<Models.ShopItem> { }
        },
        {
            "postGolem",
            new List<Models.ShopItem> { }
        },
        {
            "postLunaticCultist",
            new List<Models.ShopItem> { }
        },
        {
            "postMoonlord",
            new List<Models.ShopItem> { }
        },
    };

    public static ShopItems Shop { get; set; } = new();

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
}
