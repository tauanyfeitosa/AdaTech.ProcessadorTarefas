using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdaTech.ProcessadorTarefas.Library.Models
{
    public enum StatusProcessoTarefa
    {
        Agendado = 1,
        EmAndamento = 2,
        Pausado = 3,
        Concluido = 4,
        Cancelado = 5
    }
}
