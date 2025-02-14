using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using TShockAPI;

namespace LDShop;

public class PluginSettings
{
    #region Config
    #endregion
    #region ShopList
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

    public static Models.ResponseMessage Load()
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
                    return new Models.ResponseMessage()
                    {
                        Text = $"[{PluginDisplayName}] Loaded config.",
                        Color = Color.LimeGreen,
                    };
                }
                else
                {
                    return new Models.ResponseMessage()
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
                return new Models.ResponseMessage()
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
            return new Models.ResponseMessage()
            {
                Text =
                    $"[{PluginDisplayName}] Config file doesn't exist yet. A new one has been created.",
                Color = Color.Yellow,
            };
        }
    }
}
