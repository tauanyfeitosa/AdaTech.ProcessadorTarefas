using AdaTech.ProcessadorTarefas.Library.Models;
using System.Collections.Concurrent;

namespace AdaTech.ProcessadorTarefas.Library.Services
{
    public class ProcessamentoService
    {
        private readonly SemaphoreSlim _semaforoProcessos;
        private static Random _random = new Random();
        private List<Task> _tasks = new List<Task>();
        private List<Processo> _listaDeProcessos = new List<Processo>();
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        public ProcessamentoService()
        {
            _semaforoProcessos = new SemaphoreSlim(Environment.ProcessorCount);
            _listaDeProcessos = CriarListaDeProcessos();
        }

        private void AdicionarProcesso(Processo processo)
        {
            _listaDeProcessos.Add(processo);
        }

        private int ObterNumeroProcesso()
        {
            int numeroProcesso = _listaDeProcessos.Count() + 1;
            return numeroProcesso;
        }

        public int CriarProcesso()
        {
            var numeroProcesso = ObterNumeroProcesso();
            var processo = new Processo
            {
                Titulo = $"Processo {numeroProcesso}",
                Tarefas = CriarListaDeTarefas(numeroProcesso),
                Status = StatusProcessoTarefa.Agendado
            };
            AdicionarProcesso(processo);

            return numeroProcesso;
        }

        public async Task IniciarProcessoAsync()
        {
            CancellationToken cancellationToken = _cancellationTokenSource.Token;

            var processosAgendados = _listaDeProcessos
                .Where(p => p.Status == StatusProcessoTarefa.Agendado)
                .ToList();

            foreach (var processo in processosAgendados)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                var processoTask = Task.Run(async () =>
                {
                    await _semaforoProcessos.WaitAsync(cancellationToken);
                    try
                    {
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

                _tasks.Add(processoTask);
            }

            await Task.WhenAll(_tasks);
        }


        public Processo ObterProcessoPorId(int processoId)
        {
            return _listaDeProcessos.FirstOrDefault(p =>
            {
                var idPart = p.Titulo.Replace("Processo ", "");
                if (int.TryParse(idPart, out int id))
                {
                    return id == processoId;
                }
                return false;
            });
        }

        public List<Processo> ObterTodosProcessos()
        {
            return _listaDeProcessos;
        }

        private async Task ExecutarProcesso(Processo processo)
        {
            if (processo.Status == StatusProcessoTarefa.Concluido || processo.Status == StatusProcessoTarefa.Cancelado)
            {
                return;
            }

            processo.CancellationTokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = processo.CancellationTokenSource.Token;
            SemaphoreSlim semaforo = new SemaphoreSlim(5);
            List<Task> tarefasAssincronas = processo.Tarefas.ConvertAll(tarefa =>
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

        private List<Processo> CriarListaDeProcessos()
        {
            var processos = new List<Processo>();
            for (int i = 1; i <= 100; i++)
            {
                processos.Add(new Processo
                {
                    Titulo = $"Processo {i}",
                    Tarefas = CriarListaDeTarefas(i),
                    Status = StatusProcessoTarefa.Agendado
                });
            }
            return processos;
        }

        private List<Tarefa> CriarListaDeTarefas(int numeroProcesso)
        {
            int quantidadeTarefas = _random.Next(10, 100);
            var tarefas = new List<Tarefa>();
            for (int i = 1; i <= quantidadeTarefas; i++)
            {
                tarefas.Add(new Tarefa { Titulo = $"Tarefa {i} do Processo {numeroProcesso}", Status = StatusProcessoTarefa.Agendado });
            }
            return tarefas;
        }

        private async Task ProcessarTarefa(Tarefa tarefa, CancellationToken cancellationToken)
        {
            if (tarefa.Status == StatusProcessoTarefa.Concluido || cancellationToken.IsCancellationRequested)
            {
                return;
            }

            int tempoProcessamento = _random.Next(3000, 60000);

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


        public void CancelarProcesso()
        {
            _cancellationTokenSource.Cancel();

            foreach (var processo in _listaDeProcessos)
            {
                if (processo.Status != StatusProcessoTarefa.Concluido)
                {
                    processo.Status = StatusProcessoTarefa.Cancelado;
                    processo.ResetTarefasProcessadas();
                }
            }

            _tasks.Clear();
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public void CancelarProcesso(Processo processo)
        {
            if (processo.CancellationTokenSource != null && !processo.CancellationTokenSource.IsCancellationRequested)
            {
                processo.CancellationTokenSource.Cancel();
                processo.Status = StatusProcessoTarefa.Cancelado;
                processo.ResetTarefasProcessadas();
                processo.CancellationTokenSource.Dispose();
                processo.CancellationTokenSource = null;
            }

            IniciarProcessosAgendados();
        }

        private async Task IniciarProcessosAgendados()
        {
            var processosAgendados = _listaDeProcessos
                .Where(p => p.Status == StatusProcessoTarefa.Agendado)
                .ToList();

            foreach (var processo in processosAgendados)
            {
                await _semaforoProcessos.WaitAsync();
                try
                {
                    processo.Status = StatusProcessoTarefa.EmAndamento;
                    await ExecutarProcesso(processo);
                }
                finally
                {
                    _semaforoProcessos.Release();
                }
            }
        }

    }
}
