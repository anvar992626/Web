using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace RealG.Models;

public class Movie
{
    public int Id { get; set; }
    public string? Genre { get; set; }
    public string Titel { get; set; }
    public string Beskrivning { get; set; }
    public int Längd { get; set; }
    public decimal Pris { get; set; }
    public string? ImagePath { get; set; }

    // Relation till Föreställning
    public virtual ICollection<Föreställning> Föreställningar { get; set; }

   
}