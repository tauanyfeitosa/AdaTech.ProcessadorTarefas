using AdaTech.ProcessadorTarefas.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdaTech.ProcessadorTarefas.Library.Services
{
    public class TarefaService
    {
        private static Random _random = new Random();
        public List<Tarefa> CriarListaDeTarefas(int numeroProcesso)
        {
            int quantidadeTarefas = _random.Next(10, 100);
            var tarefas = new List<Tarefa>();
            for (int i = 1; i <= quantidadeTarefas; i++)
            {
                tarefas.Add(new Tarefa { Titulo = $"Tarefa {i} do Processo {numeroProcesso}", Status = StatusProcessoTarefa.Agendado });
            }
            return tarefas;
        }
    }
}
