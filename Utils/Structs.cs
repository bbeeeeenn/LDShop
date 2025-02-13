namespace Shop.Utils;

public class Structs
{
    public struct MessageResponse
    {
        public string Text;
        public Microsoft.Xna.Framework.Color Color;
    }

    public struct ShopItem
    {
        public string name;
        public short netID;
        public int amount;
        public short prefixID;
        public int buyprice;
        public int sellprice;
    }
}
