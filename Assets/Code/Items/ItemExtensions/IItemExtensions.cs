using Assets.Code.Items.Interfaces;

namespace Assets.Code.Items.ItemExtensions
{
    public static class IItemExtensions
    {
        public static int GetId(this IItem item) => ItemManagerSO.Instance.GetItemId(item);
    }
}
