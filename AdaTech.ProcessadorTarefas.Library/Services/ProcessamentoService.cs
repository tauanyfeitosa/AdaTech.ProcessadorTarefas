using AdaTech.ProcessadorTarefas.Library.Models;
using System.Collections.Concurrent;

namespace AdaTech.ProcessadorTarefas.Library.Services
{
    public class ProcessamentoService
    {
        private readonly ProcessoExecutorService _processoExecutorService;

        public ProcessamentoService(ProcessoExecutorService processoExecutorService)
        {
            _processoExecutorService = processoExecutorService;
        }

        public int CriarProcesso()
        {
            return _processoExecutorService.ProcessoService.CriarProcesso();
        }

        public async Task IniciarProcessoAsync()
        {
            await _processoExecutorService.IniciarProcessoAsync();
        }


        public Processo ObterProcessoPorId(int processoId)
        {
            return _processoExecutorService.ProcessoService.ObterProcessoPorId(processoId);
        }


        public List<Processo> ObterTodosProcessos()
        {
            return _processoExecutorService.ProcessoService.ObterTodosProcessos();
        }

        public void CancelarProcesso()
        {
            _processoExecutorService.CancellationTokenSource.Cancel();

            foreach (var processo in _processoExecutorService.ProcessoService.ObterTodosProcessos())
            {
                if (processo.Status != StatusProcessoTarefa.Concluido)
                {
                    processo.Status = StatusProcessoTarefa.Cancelado;
                    processo.ResetTarefasProcessadas();
                    if (processo.CancellationTokenSource != null)
                    {
                        processo.CancellationTokenSource.Cancel();
                        processo.CancellationTokenSource.Dispose();
                        processo.CancellationTokenSource = null;
                    }
                }
            }

            if (!_processoExecutorService.ProcessoService.ObterTodosProcessos().All(p => p.Status == StatusProcessoTarefa.Cancelado || p.Status == StatusProcessoTarefa.Concluido))
            {
                CancelarProcesso();
            }

            _processoExecutorService.Tasks.Clear();
            _processoExecutorService.CancellationTokenSource.Dispose();
            _processoExecutorService.CancellationTokenSource = new CancellationTokenSource();
        }


        public async void CancelarProcesso(Processo processo)
        {
            if (processo.CancellationTokenSource != null && !processo.CancellationTokenSource.IsCancellationRequested)
            {
                processo.CancellationTokenSource.Cancel();
                processo.Status = StatusProcessoTarefa.Cancelado;
                processo.ResetTarefasProcessadas();
                processo.CancellationTokenSource.Dispose();
                processo.CancellationTokenSource = null;
            }

            await _processoExecutorService.IniciarProcessoAsync();
        }

        public void PausarProcesso()
        {
            foreach (var processo in _processoExecutorService.ProcessoService.ObterTodosProcessos().Where(p => p.Status == StatusProcessoTarefa.EmAndamento))
            {
                processo.Status = StatusProcessoTarefa.Pausado;
                processo.CancellationTokenSource.Cancel();
            }

            _processoExecutorService.Tasks.Clear();
            _processoExecutorService.CancellationTokenSource.Dispose();
            _processoExecutorService.CancellationTokenSource = new CancellationTokenSource();
        }

        public async void RetomarProcesso()
        {
            bool temProcessosParaRetomar = false;

            foreach (var processo in _processoExecutorService.ProcessoService.ObterTodosProcessos().Where(p => p.Status == StatusProcessoTarefa.Pausado))
            {
                processo.Status = StatusProcessoTarefa.Agendado;
                temProcessosParaRetomar = true;
            }

            if (temProcessosParaRetomar)
            {
                await IniciarProcessoAsync();
            }
        }
    }
}
