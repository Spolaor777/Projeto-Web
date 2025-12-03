using Microsoft.AspNetCore.Mvc;
using SistemaSuporte.Data;
using SistemaSuporte.Models;
using Microsoft.AspNetCore.Http;

namespace SistemaSuporte.Controllers {
    public class LoginController : Controller {
        private readonly ApplicationDbContext _context;

        public LoginController(ApplicationDbContext context) {
            _context = context;
        }

        // GET - tela de login
        public IActionResult Index() {
            return View();
        }

        // POST - validação
        [HttpPost]
        public IActionResult Index(string email, string senha) {
            var usuario = _context.Usuarios
                .FirstOrDefault(u => u.Email == email && u.Senha == senha);

            if (usuario == null) {
                ViewBag.Erro = "E-mail ou senha incorretos!";
                return View();
            }

            // ✅ Salvar sessão
            HttpContext.Session.SetInt32("UsuarioId", usuario.UsuarioId);
            HttpContext.Session.SetString("Tipo", usuario.Tipo);

            // ✅ Redirecionar conforme tipo
            if (usuario.Tipo == "admin")
                return RedirectToAction("Dashboard", "Admin");

            return RedirectToAction("Dashboard", "Cliente");
        }

        public IActionResult Logout() {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
    }
}

