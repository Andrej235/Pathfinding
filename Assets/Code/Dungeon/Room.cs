public record Room
{
	public enum RoomType
	{
		Enemy = 1,
		Reward = 2,
		Special = 4,
		Shop = 8,
		Boss = 16,
	}

	public RoomType Type;
}
