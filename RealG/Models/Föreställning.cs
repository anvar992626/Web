namespace RealG.Models
{
    public class Föreställning
    {
        public int FöreställningId { get; set; }
        public DateTime Datum { get; set; }

        public TimeSpan Tider { get; set; }
        // Relationer
        public int FilmId { get; set; }
        public virtual Movie Film { get; set; }

        public int SalongId { get; set; }
        public virtual Salong Salong { get; set; }

        // Relation till Bokning
        public virtual ICollection<Bokning> Bokningar { get; set; }
    }

}
