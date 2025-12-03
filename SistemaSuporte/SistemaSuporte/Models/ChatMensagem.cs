using SistemaSuporte.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class ChatMensagem
{
    [Key]
    public int ChatMensagemId { get; set; }

    [Required]
    public int ChamadoId { get; set; }
    [ForeignKey("ChamadoId")]
    public Chamado? Chamado { get; set; }

    [Required]
    public string Remetente { get; set; } // "cliente", "ia" ou "admin"

    [Required]
    public string Mensagem { get; set; }

    public DateTime DataEnvio { get; set; } = DateTime.Now;
}
