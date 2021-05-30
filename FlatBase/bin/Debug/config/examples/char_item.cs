namespace test
{
	public class Character
	{
		public string name { get; set; }
		public int level { get; set; }
		public List<Item> inventory { get; set; }
	}
	
	public class Item
	{
		public string name { get; set; }
		public bool usable { get; set; }
	}
}