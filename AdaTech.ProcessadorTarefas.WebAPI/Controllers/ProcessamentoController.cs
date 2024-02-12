using AdaTech.ProcessadorTarefas.Library.Models;
using AdaTech.ProcessadorTarefas.Library.Services;
using Microsoft.AspNetCore.Mvc;

namespace AdaTech.ProcessadorTarefas.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProcessamentoController : ControllerBase
    {
        private readonly ProcessamentoService _processamentoService;

        public ProcessamentoController(ProcessamentoService processamentoService)
        {
            _processamentoService = processamentoService;
        }

        [HttpPost("iniciar")]
        public async Task<ActionResult<string>> IniciarProcessamento()
        {
            try
            {
                if (!_processamentoService.ObterTodosProcessos().Any())
                {
                    return NotFound("Nenhum processo encontrado.");
                }
                else if (_processamentoService.ObterTodosProcessos().All(p => p.Status == StatusProcessoTarefa.Cancelado))
                {
                    return BadRequest("Não existe nenhum processo apto para processamento!");
                }
                else if (_processamentoService.ObterTodosProcessos().Any(p => p.Status == StatusProcessoTarefa.Agendado))
                {
                    await _processamentoService.IniciarProcessoAsync();
                    return Ok("Processamento de tarefas finalizado.");
                }
                else
                {
                    return BadRequest("Não existem processos agendados para serem processados.");
                }
            }
            catch
            {
                return BadRequest("Tarefas canceladas durante o processamento");
            }
        }


        [HttpGet("progresso")]
        public ActionResult<IEnumerable<object>> ConsultarProgresso()
        {
            var processos = _processamentoService.ObterTodosProcessos();
            if (processos == null || !processos.Any())
            {
                return NotFound("Nenhum processo encontrado.");
            }

            var progressoOrdenado = processos
                .OrderByDescending(processo => processo.Status == StatusProcessoTarefa.EmAndamento) 
                .ThenBy(processo => processo.Status == StatusProcessoTarefa.Concluido) 
                .Select(processo => new
                {
                    ProcessoId = processo.Titulo.Replace("Processo ", ""),
                    Titulo = processo.Titulo,
                    TotalTarefas = processo.Tarefas.Count,
                    TarefasProcessadas = processo.TarefasProcessadas,
                    PorcentagemConcluida = (processo.TarefasProcessadas * 100) / processo.Tarefas.Count,
                    Status = processo.Status
                });

            return Ok(progressoOrdenado);
        }

        [HttpPost("criar")]
        public ActionResult<string> CriarProcesso()
        {
            var numeroProcesso = _processamentoService.CriarProcesso();
            return Ok($"Processo {numeroProcesso} criado com sucesso.");
        }

        [HttpPost("cancelar")]
        public ActionResult<string> CancelarProcessoAll()
        {
            _processamentoService.CancelarProcesso();
            return Ok("Processamento de tarefas cancelado.");
        }

        //[HttpPost("cancelarById")]
        //public ActionResult<string> CancelarProcessoById(int processoId)
        //{
        //    var processo = _processamentoService.ObterProcessoPorId(processoId);
        //    if (processo == null)
        //    {
        //        return NotFound("Nenhum processo encontrado.");
        //    }
        //    _processamentoService.CancelarProcesso(processoId);
        //    return Ok($"Processo {processoId} cancelado com sucesso.");
        //}


        [HttpGet("getProcessorCount")]
        public IActionResult GetProcessorCount()
        {
            return Ok(Environment.ProcessorCount);
        }

    }
}
