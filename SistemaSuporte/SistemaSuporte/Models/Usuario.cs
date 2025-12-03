using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaSuporte.Models {
    [Table("usuarios")]
    public class Usuario {

        [Key]
        [Column("usuario_id")]
        public int UsuarioId { get; set; }

        [Required]
        [Column("nome")]
        [StringLength(100)]
        public string Nome { get; set; } = string.Empty;

        [Required]
        [Column("email")]
        [StringLength(150)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "A senha é obrigatória.")]
        [Column("senha")]
        [StringLength(100, MinimumLength = 4, ErrorMessage = "A senha deve ter no mínimo 4 caracteres.")]
        public string Senha { get; set; } = string.Empty;

        [Required]
        [Column("tipo")]
        [StringLength(20)]
        [DisplayName("Tipo de Usuário")]
        public string Tipo { get; set; } = "cliente";
    }
}

