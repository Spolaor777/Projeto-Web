using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaSuporte.Models {
    [Table("formulario_avaliacao")]
    public class FormularioAvaliacao {
        [Key]
        [Column("avaliacao_id")]
        public int AvaliacaoId { get; set; }

        [Required]
        [Column("chamado_id")]
        public int ChamadoId { get; set; }
        [ForeignKey("ChamadoId")]
        public Chamado? Chamado { get; set; }

        [Required]
        [Column("satisfacao")]
        [Range(1, 5)]
        public int Satisfacao { get; set; }

        [Column("comentario")]
        public string? Comentario { get; set; }

        [Column("data_avaliacao")]
        public DateTime DataAvaliacao { get; set; } = DateTime.Now;
    }
}

