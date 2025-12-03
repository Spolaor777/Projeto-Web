using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaSuporte.Filters;
using SistemaSuporte.Data;
using SistemaSuporte.Models.ViewModels;

namespace SistemaSuporte.Controllers {
    public class AdminController : Controller {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context) {
            _context = context;
        }

        [UsuarioAutenticado]
        public async Task<IActionResult> Dashboard() {
            // contagens de chamados
            var totalChamados = await _context.Chamados.CountAsync();
            var chamadosAbertos = await _context.Chamados.CountAsync(c => c.Status == "Aberto");
            var chamadosFechados = await _context.Chamados.CountAsync(c => c.Status == "Fechado");

            // se quiser, futuramente, pode ter um card só de Respondido pela IA:
            // var chamadosRespondidosIA = await _context.Chamados.CountAsync(c => c.Status == "Respondido pela IA");

            // outras contagens
            var totalUsuarios = await _context.Usuarios.CountAsync();
            var totalCategorias = await _context.Categorias.CountAsync();

            var model = new DashboardViewModel {
                TotalChamados = totalChamados,
                ChamadosAbertos = chamadosAbertos,
                ChamadosFechados = chamadosFechados,
                TotalUsuarios = totalUsuarios,
                TotalCategorias = totalCategorias
            };

            return View(model);
        }

    }
}

