using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HomeTrack.Api.Models.Entities
{
public class ItemTag
{
    [Required]
    public int ItemId { get; set; } 

    [Required]
    public int TagId { get; set; }  

    [ForeignKey("ItemId")]
    public required virtual Item Item { get; set; }

    [ForeignKey("TagId")]
    public required virtual Tag Tag { get; set; }

}
}