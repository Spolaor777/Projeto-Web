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
    public class HistoricoChamadosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HistoricoChamadosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: HistoricoChamados
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.HistoricoChamados.Include(h => h.Chamado);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: HistoricoChamados/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var historicoChamado = await _context.HistoricoChamados
                .Include(h => h.Chamado)
                .FirstOrDefaultAsync(m => m.HistoricoId == id);
            if (historicoChamado == null)
            {
                return NotFound();
            }

            return View(historicoChamado);
        }

        // GET: HistoricoChamados/Create
        public IActionResult Create()
        {
            ViewData["ChamadoId"] = new SelectList(_context.Chamados, "ChamadoId", "Status");
            return View();
        }

        // POST: HistoricoChamados/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("HistoricoId,ChamadoId,Acao,DataRegistro")] HistoricoChamado historicoChamado)
        {
            if (ModelState.IsValid)
            {
                _context.Add(historicoChamado);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ChamadoId"] = new SelectList(_context.Chamados, "ChamadoId", "Status", historicoChamado.ChamadoId);
            return View(historicoChamado);
        }

        // GET: HistoricoChamados/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var historicoChamado = await _context.HistoricoChamados.FindAsync(id);
            if (historicoChamado == null)
            {
                return NotFound();
            }
            ViewData["ChamadoId"] = new SelectList(_context.Chamados, "ChamadoId", "Status", historicoChamado.ChamadoId);
            return View(historicoChamado);
        }

        // POST: HistoricoChamados/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("HistoricoId,ChamadoId,Acao,DataRegistro")] HistoricoChamado historicoChamado)
        {
            if (id != historicoChamado.HistoricoId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(historicoChamado);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HistoricoChamadoExists(historicoChamado.HistoricoId))
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
            ViewData["ChamadoId"] = new SelectList(_context.Chamados, "ChamadoId", "Status", historicoChamado.ChamadoId);
            return View(historicoChamado);
        }

        // GET: HistoricoChamados/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var historicoChamado = await _context.HistoricoChamados
                .Include(h => h.Chamado)
                .FirstOrDefaultAsync(m => m.HistoricoId == id);
            if (historicoChamado == null)
            {
                return NotFound();
            }

            return View(historicoChamado);
        }

        // POST: HistoricoChamados/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var historicoChamado = await _context.HistoricoChamados.FindAsync(id);
            if (historicoChamado != null)
            {
                _context.HistoricoChamados.Remove(historicoChamado);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HistoricoChamadoExists(int id)
        {
            return _context.HistoricoChamados.Any(e => e.HistoricoId == id);
        }
    }
}
