

[System.Serializable]
public class ItemHolder
{
    public Item item;
    public int quantity;

    public ItemHolder(Item item,int quantity)
    {
        this.item = item;
        this.quantity = quantity;
    }
}
