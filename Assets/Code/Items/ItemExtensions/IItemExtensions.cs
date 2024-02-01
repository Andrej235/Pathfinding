using Assets.Code.Items.Interfaces;

namespace Assets.Code.Items.ItemExtensions
{
#nullable enable
    public static class IItemExtensions
    {
        public static int GetId(this IItem? item) => item is null ? -1 : ItemManagerSO.Instance.GetItemId(item);
    }
}
