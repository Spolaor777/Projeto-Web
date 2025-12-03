using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaSuporte.Data;
using SistemaSuporte.Filters;
using SistemaSuporte.Models;

namespace SistemaSuporte.Controllers {
    [UsuarioAutenticado]
    public class AdminUsuariosController : Controller {
        private readonly ApplicationDbContext _context;

        public AdminUsuariosController(ApplicationDbContext context) {
            _context = context;
        }

        // LISTA USUÁRIOS
        public async Task<IActionResult> Index() {
            var usuarios = await _context.Usuarios.ToListAsync();
            return View(usuarios);
        }

        // DETALHES (NÃO MOSTRA SENHA)
        public async Task<IActionResult> Details(int? id) {
            if (id == null) return NotFound();

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.UsuarioId == id);

            if (usuario == null) return NotFound();

            return View(usuario);
        }

        // EDITAR — ADMIN SÓ PODE EDITAR NOME E TIPO
        public async Task<IActionResult> Edit(int? id) {
            if (id == null) return NotFound();

            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null) return NotFound();

            return View(usuario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Usuario dados) {
            if (id != dados.UsuarioId) return NotFound();

            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null) return NotFound();

            if (ModelState.IsValid) {

                // ⚠️ ADMIN NÃO PODE MEXER NA SENHA
                usuario.Nome = dados.Nome;
                usuario.Email = dados.Email;
                usuario.Tipo = dados.Tipo; // cliente/admin

                _context.Update(usuario);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(dados);
        }

        // EXCLUIR USUÁRIO
        public async Task<IActionResult> Delete(int? id) {
            if (id == null) return NotFound();

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.UsuarioId == id);

            if (usuario == null) return NotFound();

            return View(usuario);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id) {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario != null) {
                _context.Usuarios.Remove(usuario);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
