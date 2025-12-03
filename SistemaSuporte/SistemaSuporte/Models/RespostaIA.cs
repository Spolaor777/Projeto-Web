using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaSuporte.Models {
    [Table("respostas_ia")]
    public class RespostaIA {
        [Key]
        [Column("resposta_id")]
        public int RespostaId { get; set; }

        [Required]
        [Column("chamado_id")]
        public int ChamadoId { get; set; }
        [ForeignKey("ChamadoId")]
        public Chamado? Chamado { get; set; }

        [Required]
        [Column("prompt")]
        public string Prompt { get; set; } = string.Empty;

        [Required]
        [Column("resposta_gerada")]
        public string RespostaGerada { get; set; } = string.Empty;

        [Column("data_geracao")]
        public DateTime DataGeracao { get; set; } = DateTime.Now;
    }
}
