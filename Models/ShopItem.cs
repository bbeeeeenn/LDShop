namespace LDShop.Models;

public struct ShopItem
{
    public string name;
    public short netID;
    public byte prefixID;
    public int amount;
    public BuyPrice requirements;
    public SellPrice sellreturn;
}

public struct BuyPrice
{
    public long buyprice;
    public Dictionary<int, long> items;
}

public struct SellPrice
{
    public long sellprice;
    public Dictionary<int, long> items;
}
