namespace TMDemo.ViewModel
{
    public class GroupedTreksByCategory
    {
        public string Category { get; set; } = string.Empty;
        public List<Trek> Treks { get; set; } = new();
    }
}
