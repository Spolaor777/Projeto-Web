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
    public class FormularioAvaliacoesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FormularioAvaliacoesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: FormularioAvaliacoes
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.FormularioAvaliacao.Include(f => f.Chamado);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: FormularioAvaliacoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var formularioAvaliacao = await _context.FormularioAvaliacao
                .Include(f => f.Chamado)
                .FirstOrDefaultAsync(m => m.AvaliacaoId == id);
            if (formularioAvaliacao == null)
            {
                return NotFound();
            }

            return View(formularioAvaliacao);
        }

        // GET: FormularioAvaliacoes/Create
        public IActionResult Create(int chamadoId) {
            var model = new FormularioAvaliacao {
                ChamadoId = chamadoId
            };

            return View(model);
        }


        // POST: FormularioAvaliacoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FormularioAvaliacao formularioAvaliacao) {
            if (ModelState.IsValid) {
                formularioAvaliacao.DataAvaliacao = DateTime.Now;

                _context.Add(formularioAvaliacao);

                // Ao enviar a avaliação → fechar automaticamente o chamado
                var chamado = await _context.Chamados.FindAsync(formularioAvaliacao.ChamadoId);
                if (chamado != null) {
                    chamado.Status = "Fechado";
                }

                await _context.SaveChangesAsync();

                // REDIRECIONA PARA HOME DO CLIENTE
                return RedirectToAction("Dashboard", "Cliente");
            }

            return View(formularioAvaliacao);
        }


        // GET: FormularioAvaliacoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var formularioAvaliacao = await _context.FormularioAvaliacao.FindAsync(id);
            if (formularioAvaliacao == null)
            {
                return NotFound();
            }
            ViewData["ChamadoId"] = new SelectList(_context.Chamados, "ChamadoId", "Status", formularioAvaliacao.ChamadoId);
            return View(formularioAvaliacao);
        }

        // POST: FormularioAvaliacoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AvaliacaoId,ChamadoId,Satisfacao,Comentario,DataAvaliacao")] FormularioAvaliacao formularioAvaliacao)
        {
            if (id != formularioAvaliacao.AvaliacaoId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(formularioAvaliacao);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FormularioAvaliacaoExists(formularioAvaliacao.AvaliacaoId))
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
            ViewData["ChamadoId"] = new SelectList(_context.Chamados, "ChamadoId", "Status", formularioAvaliacao.ChamadoId);
            return View(formularioAvaliacao);
        }

        // GET: FormularioAvaliacoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var formularioAvaliacao = await _context.FormularioAvaliacao
                .Include(f => f.Chamado)
                .FirstOrDefaultAsync(m => m.AvaliacaoId == id);
            if (formularioAvaliacao == null)
            {
                return NotFound();
            }

            return View(formularioAvaliacao);
        }

        // POST: FormularioAvaliacoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var formularioAvaliacao = await _context.FormularioAvaliacao.FindAsync(id);
            if (formularioAvaliacao != null)
            {
                _context.FormularioAvaliacao.Remove(formularioAvaliacao);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FormularioAvaliacaoExists(int id)
        {
            return _context.FormularioAvaliacao.Any(e => e.AvaliacaoId == id);
        }


    }
}
