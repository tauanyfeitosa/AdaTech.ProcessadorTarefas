using AdaTech.ProcessadorTarefas.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AdaTech.ProcessadorTarefas.Library.Services
{
    public class ProcessoExecutorService
    {
        public readonly ProcessoService ProcessoService;
        public CancellationTokenSource CancellationTokenSource = new CancellationTokenSource();
        public List<Task> Tasks = new List<Task>();

        private readonly SemaphoreSlim _semaforoProcessos;
        private static Random _random = new Random();
        


        public ProcessoExecutorService(ProcessoService processoService)
        {
            _semaforoProcessos = new SemaphoreSlim(Environment.ProcessorCount);
            ProcessoService = processoService;
        }

        public async Task IniciarProcessoAsync()
        {
            CancellationToken cancellationToken = CancellationTokenSource.Token;

            var processosAgendados = ProcessoService.ObterTodosProcessos()
                .Where(p => p.Status == StatusProcessoTarefa.Agendado)
                .ToList();

            foreach (var processo in processosAgendados)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                var processoTask = Task.Run(async () =>
                {
                    try
                    {
                        await _semaforoProcessos.WaitAsync(cancellationToken);

                        if (!cancellationToken.IsCancellationRequested)
                        {
                            processo.Status = StatusProcessoTarefa.EmAndamento;
                            await ExecutarProcesso(processo);
                            if (processo.TarefasProcessadas == processo.Tarefas.Count)
                            {
                                processo.Status = StatusProcessoTarefa.Concluido;
                            }
                        }
                    }
                    catch
                    {
                        return;
                    }
                    finally
                    {
                        _semaforoProcessos.Release();
                    }
                }, cancellationToken);

                Tasks.Add(processoTask);
            }

            await Task.WhenAll(Tasks);
        }

        public async Task ExecutarProcesso(Processo processo)
        {
            if (processo.Status == StatusProcessoTarefa.Concluido || processo.Status == StatusProcessoTarefa.Cancelado || processo.Status == StatusProcessoTarefa.Pausado)
            {
                return;
            }

            processo.CancellationTokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = processo.CancellationTokenSource.Token;
            SemaphoreSlim semaforo = new SemaphoreSlim(5);
            List<Task> tarefasAssincronas = processo.Tarefas.Where(t => t.Status == StatusProcessoTarefa.Agendado).ToList().ConvertAll(tarefa =>
            {
                return Task.Run(async () =>
                {
                    await semaforo.WaitAsync(cancellationToken);
                    try
                    {
                        if (cancellationToken.IsCancellationRequested)
                        {
                            return;
                        }

                        tarefa.Status = StatusProcessoTarefa.EmAndamento;

                        await ProcessarTarefa(tarefa, cancellationToken);
                        tarefa.Status = StatusProcessoTarefa.Concluido;

                        int porcentagemConcluida = processo.TarefasProcessadas * 100 / processo.Tarefas.Count;
                        Console.WriteLine($"Processo {processo.Titulo}: {porcentagemConcluida}% concluído.");
                    }
                    finally
                    {
                        semaforo.Release();
                    }
                }, cancellationToken);
            });

            await Task.WhenAll(tarefasAssincronas);

            if (processo.Tarefas.All(t => t.Status == StatusProcessoTarefa.Concluido))
            {
                processo.Status = StatusProcessoTarefa.Concluido;
                processo.CancellationTokenSource.Dispose();
            }
        }

        private async Task ProcessarTarefa(Tarefa tarefa, CancellationToken cancellationToken)
        {
            if (tarefa.Status == StatusProcessoTarefa.Concluido || cancellationToken.IsCancellationRequested)
            {
                return;
            }

            int tempoProcessamento = _random.Next(10000, 11000);

            try
            {
                await Task.Delay(tempoProcessamento, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine($"Cancelado: {tarefa.Titulo}");
                return;
            }

            Console.WriteLine($"Processado: {tarefa.Titulo}, Tempo: {tempoProcessamento / 1000} s");
        }
    }
}
