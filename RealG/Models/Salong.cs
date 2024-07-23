namespace RealG.Models
{
    public class Salong
    {
        public int SalongId { get; set; }
        public string Namn { get; set; }
        public int AntalStolar { get; set; }
        public int Row { get; set; }
        public int Number { get; set; }

        // Relation till Föreställning
        public virtual ICollection<Föreställning> Föreställningar { get; set; }
    }

}
