using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaSuporte.Models {
    [Table("historico_chamado")]
    public class HistoricoChamado {
        [Key]
        [Column("historico_id")]
        public int HistoricoId { get; set; }

        [Required]
        [Column("chamado_id")]
        public int ChamadoId { get; set; }
        [ForeignKey("ChamadoId")]
        public Chamado? Chamado { get; set; }

        [Required]
        [Column("usuario_id")]
        public int UsuarioId { get; set; }

        [ForeignKey("UsuarioId")]
        public Usuario? Usuario { get; set; }

        [Required]
        [Column("acao")]
        public string Acao { get; set; } = string.Empty;

        [Column("data_registro")]
        public DateTime DataRegistro { get; set; } = DateTime.Now;
    }
}
