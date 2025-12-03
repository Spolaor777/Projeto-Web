using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaSuporte.Models {
    [Table("chamados")]
    public class Chamado {
        [Key]
        [Column("chamado_id")]
        [Display(Name = "Chamados")]
        public int ChamadoId { get; set; }

        [Required]
        [Column("usuario_id")]
        public int UsuarioId { get; set; }
        [ForeignKey("UsuarioId")]
        public Usuario? Usuario { get; set; }

        [Required]
        [Column("categoria_id")]
        public int CategoriaId { get; set; }
        [ForeignKey("CategoriaId")]
        public Categoria? Categoria { get; set; }

        [Required]
        [Column("titulo")]
        [StringLength(200)]
        public string Titulo { get; set; } = string.Empty;

        [Column("descricao")]
        public string? Descricao { get; set; }

        [Required]
        [Column("status")]
        [DisplayName("Status")]
        public string Status { get; set; } = "Aberto";

        [Column("data_abertura")]
        public DateTime DataAbertura { get; set; } = DateTime.Now;

        [Column("data_fechamento")]
        public DateTime? DataFechamento { get; set; }
    }
}

