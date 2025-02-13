using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Shop.Utils;
using TShockAPI;

namespace Shop;

public class PluginSettings
{
    #region Config
    public string ShopRegionName = "Shop";
    #endregion
    #region ShopList
    public Dictionary<string, List<Structs.ShopItem>> Items = new()
    {
        {
            "regular",
            new List<Structs.ShopItem>()
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
            new List<Structs.ShopItem> { }
        },
        {
            "postEyeOfCthulhu",
            new List<Structs.ShopItem> { }
        },
        {
            "postEaterOfWorlds",
            new List<Structs.ShopItem> { }
        },
        {
            "postBrainOfCthulhu",
            new List<Structs.ShopItem> { }
        },
        {
            "postQueenBee",
            new List<Structs.ShopItem> { }
        },
        {
            "postSkeletron",
            new List<Structs.ShopItem> { }
        },
        {
            "postDeerclops",
            new List<Structs.ShopItem> { }
        },
        {
            "postWallOfFlesh",
            new List<Structs.ShopItem> { }
        },
    };
    #endregion
    public static string PluginDisplayName { get; set; } = "LDShop";
    public static readonly string ConfigPath = Path.Combine(
        TShock.SavePath,
        $"{PluginDisplayName}.json"
    );
    public static PluginSettings Config { get; private set; } = new();

    public static void Save()
    {
        string configJson = JsonConvert.SerializeObject(Config, Formatting.Indented);
        File.WriteAllText(ConfigPath, configJson);
    }

    public static Structs.MessageResponse Load()
    {
        if (File.Exists(ConfigPath))
        {
            try
            {
                string json = File.ReadAllText(ConfigPath);
                PluginSettings? deserializedConfig = JsonConvert.DeserializeObject<PluginSettings>(
                    json
                );
                if (deserializedConfig != null)
                {
                    Config = deserializedConfig;
                    return new Structs.MessageResponse()
                    {
                        Text = $"[{PluginDisplayName}] Loaded config.",
                        Color = Color.LimeGreen,
                    };
                }
                else
                {
                    return new Structs.MessageResponse()
                    {
                        Text =
                            $"[{PluginDisplayName}] Config file was found, but deserialization returned null.",
                        Color = Color.Red,
                    };
                }
            }
            catch (Exception ex)
            {
                TShock.Log.ConsoleError(
                    $"[{PluginDisplayName}] Error loading config: {ex.Message}"
                );
                return new Structs.MessageResponse()
                {
                    Text =
                        $"[{PluginDisplayName}] Error loading config. Check logs for more details.",
                    Color = Color.Red,
                };
            }
        }
        else
        {
            Save();
            return new Structs.MessageResponse()
            {
                Text =
                    $"[{PluginDisplayName}] Config file doesn't exist yet. A new one has been created.",
                Color = Color.Yellow,
            };
        }
    }
}
