using System;//permet d'utiliser les type de base de donnés datat time o string 
using System.ComponentModel.DataAnnotations;//permet d'utiliser des annotation sur les filds 
using System.ComponentModel.DataAnnotations.Schema;//

namespace IncidentAPI_Hazem_Trimech.Models
{
    public class Incident
    {
        [Key]//pour dire un cle primaire 
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] //la base génerer automatique 'id incremental 
        public int Id { get; set; }//comment declarer un fild 

        [Required]//champs obligatoire 
        [MaxLength(30)]//maximum length 30
        public string Title { get; set; }

        [Required]
        [MaxLength(200)]
        public string Description { get; set; }

        [Required]
        [RegularExpression("(?i)^(LOW|MEDIUM|HIGH|CRITICAL)$",
            ErrorMessage = "Severity must be LOW, MEDIUM, HIGH or CRITICAL")]//permet de faire une enimeration sur un fild et retourn un msg d'eureru si input different de ce qui est declarer 
        public string Severity { get; set; }

        [Required]
        [RegularExpression("OPEN|IN_PROGRESS|RESOLVED",
            ErrorMessage = "Status must be OPEN, IN_PROGRESS or RESOLVED")]
        public string Status { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}