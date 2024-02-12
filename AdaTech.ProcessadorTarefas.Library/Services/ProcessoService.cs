using AdaTech.ProcessadorTarefas.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdaTech.ProcessadorTarefas.Library.Services
{
    public class ProcessoService
    {
        private List<Processo> _listaDeProcessos = new List<Processo>();
        private readonly TarefaService _tarefaService;

        public ProcessoService(TarefaService tarefaService)
        {
            _tarefaService = tarefaService;
            _listaDeProcessos = CriarListaDeProcessos();
        }

        public List<Processo> ObterTodosProcessos()
        {
            return _listaDeProcessos;
        }

        public void AdicionarProcesso(Processo processo)
        {
            _listaDeProcessos.Add(processo);
        }

        public int ObterNumeroProcesso()
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
                Tarefas = _tarefaService.CriarListaDeTarefas(numeroProcesso),
                Status = StatusProcessoTarefa.Agendado
            };
            AdicionarProcesso(processo);

            return numeroProcesso;
        }

        private List<Processo> CriarListaDeProcessos()
        {
            var processos = new List<Processo>();
            for (int i = 1; i <= 100; i++)
            {
                processos.Add(new Processo
                {
                    Titulo = $"Processo {i}",
                    Tarefas = _tarefaService.CriarListaDeTarefas(i),
                    Status = StatusProcessoTarefa.Agendado
                });
            }
            return processos;
        }

        public Processo ObterProcessoPorId(int processoId)
        {
            var lista = _listaDeProcessos.FirstOrDefault(p =>
            {
                if (p == null)
                {
                    return false;
                }
                var idPart = p.Titulo.Replace("Processo ", "");
                if (int.TryParse(idPart, out int id))
                {
                    return id == processoId;
                }
                return false;
            });


            if (lista != null)
            {

                return lista;
            }

            throw new InvalidOperationException("A lista de processos não foi inicializada.");
        }

    }
}
