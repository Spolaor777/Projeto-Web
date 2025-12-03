using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaSuporte.Data;
using SistemaSuporte.Models;
using SistemaSuporte.Services;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SistemaSuporte.Controllers
{
    public class RespostasIAController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly OpenAIService _openAIService;

        public RespostasIAController(ApplicationDbContext context, OpenAIService openAIService)
        {
            _context = context;
            _openAIService = openAIService;
        }

        [HttpPost]
        public async Task<IActionResult> GerarRespostaIA(int chamadoId)
        {
            try
            {
                var chamado = await _context.Chamados
                    .Include(c => c.Categoria)
                    .FirstOrDefaultAsync(c => c.ChamadoId == chamadoId);

                if (chamado == null)
                {
                    return Json(new
                    {
                        sucesso = false,
                        mensagem = "Chamado não encontrado."
                    });
                }

                // Construir prompt com contexto
                var prompt = $"Categoria: {chamado.Categoria?.NomeCategoria}\n" +
                             $"Título: {chamado.Titulo}\n" +
                             $"Descrição: {chamado.Descricao}\n\n" +
                             "Forneça uma resposta de suporte útil.";

                var contextoSistema =
                    "Você é um assistente de suporte técnico especializado. " +
                    "Analise o problema e forneça uma solução clara.";

                // Chamar OpenAI
                var respostaIA = await _openAIService.GerarRespostaAsync(prompt, contextoSistema);

                // Salvar no banco
                var novaResposta = new RespostaIA
                {
                    ChamadoId = chamadoId,
                    Prompt = prompt,
                    RespostaGerada = respostaIA,
                    DataGeracao = DateTime.Now
                };

                _context.RespostasIA.Add(novaResposta);
                chamado.Status = "Respondido pela IA";
                await _context.SaveChangesAsync();

                // Registrar histórico
                var historico = new HistoricoChamado
                {
                    ChamadoId = chamadoId,
                    Acao = "Resposta gerada automaticamente pela IA",
                    DataRegistro = DateTime.Now
                };

                _context.HistoricoChamados.Add(historico);
                await _context.SaveChangesAsync();

                return Json(new
                {
                    sucesso = true,
                    resposta = respostaIA,
                    mensagem = "Resposta gerada com sucesso!"
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    sucesso = false,
                    mensagem = $"Erro: {ex.Message}"
                });
            }
        }

        // Manter outros métodos existentes (Index, Details, Create, etc.)
    }
}
