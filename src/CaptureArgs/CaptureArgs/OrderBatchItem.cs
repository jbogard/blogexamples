namespace CaptureArgs
{
	public enum OrderType
	{
		Express,
		Normal
	}

	public class OrderBatch
	{
		public int Id { get; set; }
		public OrderType Type { get; set; }
		public decimal TotalWeight { get; set; }
	}
}