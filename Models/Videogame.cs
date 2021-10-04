namespace ApiWithoutEF.Models
{
    public class Videogame
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Company { get; set; }
        public string Clasification { get; set; }
    }
}
