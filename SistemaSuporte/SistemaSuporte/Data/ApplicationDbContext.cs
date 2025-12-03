using Microsoft.EntityFrameworkCore;
using SistemaSuporte.Models;

namespace SistemaSuporte.Data {
    public class ApplicationDbContext : DbContext {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) {
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Chamado> Chamados { get; set; }
        public DbSet<RespostaIA> RespostasIA { get; set; }
        public DbSet<HistoricoChamado> HistoricoChamados { get; set; }
        public DbSet<FormularioAvaliacao> FormularioAvaliacao { get; set; }
        public DbSet<ChatMensagem> ChatMensagens { get; set; }

    }
}

