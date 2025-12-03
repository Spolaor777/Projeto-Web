using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaSuporte.Models {
    [Table("categorias")]
    public class Categoria {
        [Key]
        [Column("categoria_id")]
        public int CategoriaId { get; set; }

        [Required]
        [Column("nome_categoria")]
        [StringLength(100)]
        public string NomeCategoria { get; set; } = string.Empty;

        [Column("descricao_categoria")]
        [StringLength(300)]
        public string? DescricaoCategoria { get; set; }
    }
}

