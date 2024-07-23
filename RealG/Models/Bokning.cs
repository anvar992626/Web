namespace RealG.Models
{
    public class Bokning
    {
        public int MovieId { get; set; }
        public int BokningId { get; set; }
        public string StolNummer { get; set; }
        public string KundNamn { get; set; }
        public string KundEmail { get; set; }

        // Relation till Föreställning
        public int FöreställningId { get; set; }
        public virtual Föreställning Föreställning { get; set; }
     
        public DateTime BokadDatum { get; set; }
    }

}
