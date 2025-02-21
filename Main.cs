using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace LDShop
{
    [ApiVersion(2, 1)]
    public class TShockPlugin : TerrariaPlugin
    {
        public override string Name => "LDShop";
        public override string Author => "TRANQUILZOIIP - github.com/bbeeeeenn";
        public override string Description => base.Description;
        public override Version Version => base.Version;

        public TShockPlugin(Main game)
            : base(game) { }

        public override void Initialize()
        {
            // Load config
            TShock.Log.ConsoleInfo(PluginSettings.Load().Text);
            // Load events
            EventManager.RegisterAll(this);
            // Load commands
            CommandManager.RegisterAll();

            // Load Shop
            TShock.Log.ConsoleInfo(ShopItems.LoadShop().Text);

            TShockAPI.Commands.ChatCommands.Add(new Command(TestCmd, "test"));
        }

        private void TestCmd(CommandArgs args)
        {
            args.Player.SendInfoMessage(string.Join("\n", ShopItems.Shop.Items.Keys));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                EventManager.DeregisterAll(this);
            }
            base.Dispose(disposing);
        }
    }
}
