using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using TShockAPI;

namespace LDShop;

public class PluginSettings
{
    #region Config
    public string ShopListPath = Path.Join(PluginDirectory, "Shop.json");
    #endregion


    public static string PluginDisplayName { get; set; } = "LDShop";
    public static readonly string PluginDirectory = Path.Join(TShock.SavePath, "LDShop");
    public static readonly string ConfigPath = Path.Combine(PluginDirectory, $"Config.json");
    public static PluginSettings Config { get; private set; } = new();

    public static void Save()
    {
        string configJson = JsonConvert.SerializeObject(Config, Formatting.Indented);
        File.WriteAllText(ConfigPath, configJson);
    }

    public static Models.ResponseMessage Load()
    {
        if (!Directory.Exists(PluginDirectory))
            Directory.CreateDirectory(PluginDirectory);
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
