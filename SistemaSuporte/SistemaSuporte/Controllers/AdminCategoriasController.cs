using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaSuporte.Data;
using SistemaSuporte.Filters;
using SistemaSuporte.Models;

namespace SistemaSuporte.Controllers {
    [UsuarioAutenticado]
    public class AdminCategoriasController : Controller {
        private readonly ApplicationDbContext _context;

        public AdminCategoriasController(ApplicationDbContext context) {
            _context = context;
        }

        // LISTA
        public async Task<IActionResult> Index() {
            var categorias = await _context.Categorias.ToListAsync();
            return View(categorias);
        }

        // DETALHES
        public async Task<IActionResult> Details(int? id) {
            if (id == null) return NotFound();

            var categoria = await _context.Categorias
                .FirstOrDefaultAsync(c => c.CategoriaId == id);

            if (categoria == null) return NotFound();

            return View(categoria);
        }

        // CREATE GET
        public IActionResult Create() {
            return View();
        }

        // CREATE POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Categoria categoria) {
            if (ModelState.IsValid) {
                _context.Add(categoria);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "AdminCategorias");
            }
            return View(categoria);
        }

        // EDIT GET
        public async Task<IActionResult> Edit(int? id) {
            if (id == null) return NotFound();

            var categoria = await _context.Categorias.FindAsync(id);
            if (categoria == null) return NotFound();

            return View(categoria);
        }

        // EDIT POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Categoria categoria) {
            if (id != categoria.CategoriaId)
                return NotFound();

            if (ModelState.IsValid) {
                _context.Update(categoria);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "AdminCategorias");
            }

            return View(categoria);
        }

        // DELETAR GET (CONFIRMAR)
        public async Task<IActionResult> Delete(int? id) {
            if (id == null) return NotFound();

            var categoria = await _context.Categorias
                .FirstOrDefaultAsync(c => c.CategoriaId == id);

            if (categoria == null) return NotFound();

            return View(categoria);
        }

        // DELETAR POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id) {
            var categoria = await _context.Categorias.FindAsync(id);

            _context.Categorias.Remove(categoria);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "AdminCategorias");
        }
    }
}
