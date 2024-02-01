using Assets.Code.Items.Interfaces;
using Assets.Code.Items.ItemExtensions;

namespace Assets.Code.Inventory
{
#nullable enable
    public class StorageSlot
    {
        private int id;
        public int Id
        {
            get
            {
                if (id == -1)
                    id = -1;

                return id;
            }
            set
            {
                if (value < 0)
                {
                    id = -1;
                    item = null;
                    return;
                }

                item = ItemManagerSO.Instance.GetItem(value);
                id = item is null ? -1 : value;
            }
        }

        private IItem? item;
        public IItem? Item
        {
            get
            {
                if (Id == -1)
                    return null;

                item ??= ItemManagerSO.Instance.GetItem(Id);
                return item;
            }

            set
            {
                if (value is null)
                {
                    item = null;
                    return;
                }

                var newId = value.GetId();
                if (newId == -1)
                    return;

                item = value;
                id = newId;
            }
        }

        private int amount;
        public int Amount
        {
            get => amount;
            set => amount = value > 0 ? value : 0;
        }
    }
}
