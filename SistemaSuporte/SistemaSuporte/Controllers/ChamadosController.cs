using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaSuporte.Data;
using SistemaSuporte.Models;

namespace SistemaSuporte.Controllers
{
    public class ChamadosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ChamadosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Chamados
        public async Task<IActionResult> Index() {
            // Recupera o usuário logado
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");

            // Se não estiver logado, volta pro login
            if (usuarioId == null) {
                return RedirectToAction("Index", "Login");
            }

            // Filtra apenas os chamados do usuário logado
            var chamadosUsuario = await _context.Chamados
                .Include(c => c.Usuario)
                .Include(c => c.Categoria)
                .Where(c => c.UsuarioId == usuarioId)
                .ToListAsync();

            // Envia os dados pra view
            return View(chamadosUsuario);
        }




        // GET: Chamados/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chamado = await _context.Chamados
                .Include(c => c.Categoria)
                .Include(c => c.Usuario)
                .FirstOrDefaultAsync(m => m.ChamadoId == id);
            if (chamado == null)
            {
                return NotFound();
            }

            return View(chamado);
        }

        // GET: Chamados/Create
        public IActionResult Create() {
            // Carrega as categorias para o dropdown
            ViewBag.Categorias = new SelectList(_context.Categorias, "CategoriaId", "NomeCategoria");

            // Não precisa carregar usuários aqui, pois vem do login
            return View();
        }

        // POST: Chamados/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ChamadoId,UsuarioId,CategoriaId,Titulo,Descricao,Status,DataAbertura,DataFechamento")] Chamado chamado)
        {
            if (ModelState.IsValid)
            {
                var usuarioId = HttpContext.Session.GetInt32("UsuarioId");

                if (usuarioId == null)
                {
                    return RedirectToAction("Index", "Login");
                }

                // Preenche informações do chamado
                chamado.UsuarioId = usuarioId.Value;
                chamado.Status = "Aberto";
                chamado.DataAbertura = DateTime.Now;

                // 1️⃣ Salva o chamado primeiro
                _context.Add(chamado);
                await _context.SaveChangesAsync();

                // 2️⃣ Cria o histórico com o ID real do chamado e do usuário
                var historico = new HistoricoChamado
                {
                    ChamadoId = chamado.ChamadoId,
                    UsuarioId = usuarioId.Value,
                    Acao = "Chamado criado"
                };

                _context.HistoricoChamados.Add(historico);
                await _context.SaveChangesAsync();

                TempData["MensagemSucesso"] = "Chamado criado com sucesso! 🎉";

                return RedirectToAction("Dashboard", "Cliente");
            }

            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "CategoriaId", "NomeCategoria", chamado.CategoriaId);
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "UsuarioId", "Email", chamado.UsuarioId);

            return View(chamado);
        }



        // GET: Chamados/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chamado = await _context.Chamados.FindAsync(id);
            if (chamado == null)
            {
                return NotFound();
            }
            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "CategoriaId", "NomeCategoria", chamado.CategoriaId);
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "UsuarioId", "Email", chamado.UsuarioId);
            return View(chamado);
        }

        // POST: Chamados/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ChamadoId,UsuarioId,CategoriaId,Titulo,Descricao,Status,DataAbertura,DataFechamento")] Chamado chamado)
        {
            if (id != chamado.ChamadoId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(chamado);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ChamadoExists(chamado.ChamadoId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "CategoriaId", "NomeCategoria", chamado.CategoriaId);
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "UsuarioId", "Email", chamado.UsuarioId);
            return View(chamado);
        }

        // GET: Chamados/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chamado = await _context.Chamados
                .Include(c => c.Categoria)
                .Include(c => c.Usuario)
                .FirstOrDefaultAsync(m => m.ChamadoId == id);
            if (chamado == null)
            {
                return NotFound();
            }

            return View(chamado);
        }

        // POST: Chamados/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var chamado = await _context.Chamados.FindAsync(id);
            if (chamado != null)
            {
                _context.Chamados.Remove(chamado);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Chat(int? id)
        {
            if (id == null)
                return NotFound();

            var chamado = await _context.Chamados
                .Include(c => c.Categoria)
                .Include(c => c.Usuario)
                .FirstOrDefaultAsync(m => m.ChamadoId == id);

            if (chamado == null)
                return NotFound();

            var mensagens = await _context.ChatMensagens
                .Where(m => m.ChamadoId == id)
                .OrderBy(m => m.DataEnvio)
                .ToListAsync();

            ViewBag.MensagensChat = mensagens;
            return View(chamado);
        }

        private bool ChamadoExists(int id)
        {
            return _context.Chamados.Any(e => e.ChamadoId == id);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarcarResolvidoIa(int chamadoId) {
            var chamado = await _context.Chamados.FindAsync(chamadoId);
            if (chamado == null)
                return NotFound();

            chamado.Status = "Fechado";
            chamado.DataFechamento = DateTime.Now;

            _context.HistoricoChamados.Add(new HistoricoChamado {
                ChamadoId = chamadoId,
                Acao = "Cliente informou que a IA resolveu o problema",
                DataRegistro = DateTime.Now
            });

            await _context.SaveChangesAsync();

            // Leva o usuário para o formulário de avaliação já com o ID do chamado
            return RedirectToAction("Create", "FormularioAvaliacoes", new { chamadoId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarcarComoNaoResolvido(int id) {
            var chamado = await _context.Chamados.FindAsync(id);

            if (chamado == null)
                return NotFound();

            // volta para Aberto
            chamado.Status = "Aberto";

            await _context.SaveChangesAsync();

            // redireciona para a tela de edição do chamado
            return RedirectToAction("Edit", new { id = id });
        }



    }
}
