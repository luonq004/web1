namespace Lab2.Models
{
    public class MenuItem
    {
        public int Id { get; set; }//Item id
        public string Name { get; set; } //Item name
        public string Link { get; set; } //Item label
        public string Url { get; internal set; }

        public string Text { get; internal set; }
    }

}
