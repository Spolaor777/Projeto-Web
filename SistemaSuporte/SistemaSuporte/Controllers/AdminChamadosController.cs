using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaSuporte.Data;
using SistemaSuporte.Filters;
using SistemaSuporte.Models; // se ainda não tiver
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.AspNetCore.Mvc.Rendering;
using ClosedXML.Excel;



namespace SistemaSuporte.Controllers {
    [UsuarioAutenticado]
    public class AdminChamadosController : Controller {
        private readonly ApplicationDbContext _context;

        public AdminChamadosController(ApplicationDbContext context) {
            _context = context;
        }

        // ============== LISTA ====================
        public async Task<IActionResult> Index() {
            var chamados = await _context.Chamados
                .Include(c => c.Categoria)
                .Include(c => c.Usuario)
                .OrderByDescending(c => c.DataAbertura)
                .ToListAsync();

            return View(chamados);
        }

        // ============== DETALHES =================
        public async Task<IActionResult> Details(int id) {
            var chamado = await _context.Chamados
                .Include(c => c.Categoria)
                .Include(c => c.Usuario)
                .FirstOrDefaultAsync(c => c.ChamadoId == id);

            if (chamado == null)
                return NotFound();

            return View(chamado);
        }

        // ============== EDIT (GET) ===============
        public async Task<IActionResult> Edit(int id) {
            var chamado = await _context.Chamados
                .Include(c => c.Categoria)
                .Include(c => c.Usuario)
                .FirstOrDefaultAsync(c => c.ChamadoId == id);

            if (chamado == null)
                return NotFound();

            CarregarCombos(chamado.CategoriaId, chamado.Status);
            return View(chamado);
        }

        // ============== EDIT (POST) ==============
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Chamado form) {
            if (id != form.ChamadoId)
                return NotFound();

            var chamadoDb = await _context.Chamados
                .FirstOrDefaultAsync(c => c.ChamadoId == id);

            if (chamadoDb == null)
                return NotFound();

            if (!ModelState.IsValid) {
                CarregarCombos(form.CategoriaId, form.Status);
                return View(form);
            }

            // Só atualiza o que o admin pode mexer
            chamadoDb.Titulo = form.Titulo;
            chamadoDb.Descricao = form.Descricao;
            chamadoDb.CategoriaId = form.CategoriaId;
            chamadoDb.Status = form.Status;

            // se marcar como resolvido e não tiver data de fechamento ainda
            if (form.Status == "Resolvido" && chamadoDb.DataFechamento == null) {
                chamadoDb.DataFechamento = DateTime.Now;
            }

            _context.Update(chamadoDb);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // ============== DELETE (GET) =============
        public async Task<IActionResult> Delete(int id) {
            var chamado = await _context.Chamados
                .Include(c => c.Categoria)
                .Include(c => c.Usuario)
                .FirstOrDefaultAsync(c => c.ChamadoId == id);

            if (chamado == null)
                return NotFound();

            return View(chamado);
        }

        // ============== DELETE (POST) ============
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id) {
            var chamado = await _context.Chamados.FindAsync(id);
            if (chamado == null)
                return NotFound();

            // 1️⃣ Remover todos os históricos relacionados
            var historicos = _context.HistoricoChamados
                .Where(h => h.ChamadoId == id);
            _context.HistoricoChamados.RemoveRange(historicos);

            // 2️⃣ Remover todas as respostas IA relacionadas
            var respostasIa = _context.RespostasIA
                .Where(r => r.ChamadoId == id);
            _context.RespostasIA.RemoveRange(respostasIa);

            // 3️⃣ Remover avaliações (se existirem)
            var avaliacoes = _context.FormularioAvaliacao
                .Where(a => a.ChamadoId == id);
            _context.FormularioAvaliacao.RemoveRange(avaliacoes);

            // 4️⃣ Agora pode remover o chamado
            _context.Chamados.Remove(chamado);

            // 5️⃣ Salvar tudo
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // ============== EXPORTAR PDF =============
        public IActionResult ExportarPDF() {
            var chamados = _context.Chamados
                .Include(c => c.Categoria)
                .Include(c => c.Usuario)
                .OrderByDescending(c => c.DataAbertura)
                .ToList();

            using var ms = new MemoryStream();

            PdfWriter writer = new PdfWriter(ms);
            PdfDocument pdf = new PdfDocument(writer);
            Document doc = new Document(pdf);

            doc.Add(new Paragraph("RELATÓRIO DE CHAMADOS")
                .SetFontSize(18)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetMarginBottom(20));

            Table tabela = new Table(5).UseAllAvailableWidth();

            tabela.AddHeaderCell("Título");
            tabela.AddHeaderCell("Categoria");
            tabela.AddHeaderCell("Usuário");
            tabela.AddHeaderCell("Status");
            tabela.AddHeaderCell("Data");

            foreach (var c in chamados) {
                tabela.AddCell(c.Titulo ?? "—");
                tabela.AddCell(c.Categoria?.NomeCategoria ?? "—");
                tabela.AddCell(c.Usuario?.Nome ?? "—");
                tabela.AddCell(c.Status);
                tabela.AddCell(c.DataAbertura.ToString("dd/MM/yyyy HH:mm"));
            }

            doc.Add(tabela);
            doc.Close();

            return File(ms.ToArray(), "application/pdf", "RelatorioChamados.pdf");
        }

        // ============== EXPORTAR XML =============
        public IActionResult ExportarExcel()
        {
            var chamados = _context.Chamados
                .Include(c => c.Categoria)
                .Include(c => c.Usuario)
                .OrderByDescending(c => c.DataAbertura)
                .ToList();

            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Chamados");

            // Cabeçalho
            ws.Cell(1, 1).Value = "Título";
            ws.Cell(1, 2).Value = "Categoria";
            ws.Cell(1, 3).Value = "Usuário";
            ws.Cell(1, 4).Value = "Status";
            ws.Cell(1, 5).Value = "Data Abertura";

            ws.Range("A1:E1").Style.Font.Bold = true;

            int row = 2;
            foreach (var c in chamados)
            {
                ws.Cell(row, 1).Value = c.Titulo ?? "—";
                ws.Cell(row, 2).Value = c.Categoria?.NomeCategoria ?? "—";
                ws.Cell(row, 3).Value = c.Usuario?.Nome ?? "—";
                ws.Cell(row, 4).Value = c.Status;
                ws.Cell(row, 5).Value = c.DataAbertura.ToString("dd/MM/yyyy HH:mm");
                row++;
            }

            ws.Columns().AdjustToContents();

            using var ms = new MemoryStream();
            workbook.SaveAs(ms);

            return File(ms.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "Chamados.xlsx");
        }

        // ============== HELPERS ==================
        private void CarregarCombos(int? categoriaIdSelecionada, string? statusAtual) {
            ViewBag.Categorias = new SelectList(
                _context.Categorias.OrderBy(c => c.NomeCategoria),
                "CategoriaId",
                "NomeCategoria",
                categoriaIdSelecionada
            );

            ViewBag.StatusLista = new List<string> {
                "Aberto",
                "Fechado"
            };
            ViewBag.StatusAtual = statusAtual;
        }
    }
}

