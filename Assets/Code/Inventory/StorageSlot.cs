using Assets.Code.Items.Interfaces;

namespace Assets.Code.Inventory
{
    public class StorageSlot
    {
        public int Id { get; set; }
        public IItem Item { get; set; }

        private int amount;
        public int Amount
        {
            get => amount;
            set => amount = value > 0 ? value : 0;
        }
    }
}
