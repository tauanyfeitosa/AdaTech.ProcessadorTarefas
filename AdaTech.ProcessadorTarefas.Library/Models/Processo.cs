namespace AdaTech.ProcessadorTarefas.Library.Models
{
    public class Processo
    {
        public string Titulo { get; set; }
        public List<Tarefa> Tarefas { get; set; }
        public int TarefasProcessadas => Tarefas.FindAll(t => t.Status == StatusProcessoTarefa.Concluido).Count;
        public StatusProcessoTarefa Status { get; set; }

        public CancellationTokenSource CancellationTokenSource { get; set; }

        public void ResetTarefasProcessadas()
        {
            foreach (var tarefa in Tarefas)
            {
                Tarefas.Where(tarefa => tarefa.Status != StatusProcessoTarefa.Concluido).ToList().ForEach(tarefa => tarefa.Status = StatusProcessoTarefa.Cancelado);
            }
        }
    }
}
