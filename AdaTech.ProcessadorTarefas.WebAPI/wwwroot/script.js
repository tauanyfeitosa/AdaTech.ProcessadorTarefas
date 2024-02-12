let paginaAtual = 0;
let processosPorPagina;
let totalProcessos;

async function iniciarProcessamento() {
    document.getElementById('resultadoIniciar').textContent = 'Iniciando processamento...';
    try {
        const response = await fetch('https://localhost:7147/api/Processamento/iniciar', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            }
        });

        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }

        const resultado = await response.text();
        document.getElementById('resultadoIniciar').textContent = resultado;
    } catch (erro) {
        console.error('Falha ao iniciar processamento:', erro);
        document.getElementById('resultadoIniciar').textContent = 'Falha ao iniciar processamento.';
    }
}

async function buscarProcessoCount() {
    try {
        const response = await fetch('https://localhost:7147/api/Processamento/getProcessorCount');
        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }
        processosPorPagina = await response.json();
    } catch (erro) {
        console.error('Falha ao buscar contagem de processadores:', erro);
    }
}

async function buscarProgresso() {
    if (processosPorPagina === undefined) {
        await buscarProcessoCount();
        return;
    }
    try {
        const response = await fetch('https://localhost:7147/api/Processamento/progresso');
        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }
        const dadosProcessos = await response.json();
        totalProcessos = dadosProcessos.length;

        const inicio = paginaAtual * processosPorPagina;
        const fim = inicio + processosPorPagina;
        const processosPagina = dadosProcessos.slice(inicio, fim);

        const container = document.getElementById('processos');
        container.innerHTML = '';

        processosPagina.forEach(processo => {
            let progressoHTML;
            if (processo.status === 5) {
                progressoHTML = `
            <div>
                <h3>${processo.titulo} (Cancelado)</h3>
                <div class="progress-container">
                    <div class="progress-bar-cancelled">
                        Cancelado
                    </div>
                </div>
                <p>Processo cancelado</p>
            </div>
        `;
            } else {
                progressoHTML = `
            <div>
                <h3>${processo.titulo}</h3>
                <div class="progress-container">
                    <div class="progress-bar" style="width: ${processo.porcentagemConcluida}%;">
                        ${processo.porcentagemConcluida}%
                    </div>
                </div>
                <p>Faltam ${processo.totalTarefas - processo.tarefasProcessadas} tasks, ${processo.tarefasProcessadas}/${processo.totalTarefas}</p>
                <button onclick="cancelarProcesso('${processo.id}')">Cancelar Processo</button>
            </div>
        `;
            }
            container.innerHTML += progressoHTML;
        });
        atualizarBotoesPaginacao();
    } catch (erro) {
        console.error('Falha ao buscar progresso:', erro);
    }
}

function atualizarBotoesPaginacao() {
    const btnAnterior = document.getElementById('btnAnterior');
    const btnProximo = document.getElementById('btnProximo');

    btnAnterior.disabled = paginaAtual === 0;
    btnProximo.disabled = ((paginaAtual + 1) * processosPorPagina) >= totalProcessos;
}

async function cancelarProcesso(id) {
    try {
        const response = await fetch(`https://localhost:7147/api/Processamento/cancelarById?id=${id}`, {
            method: 'POST'
        });
        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }
        await buscarProgresso();
    } catch (erro) {
        console.error('Falha ao cancelar processo:', erro);
    }
}


document.getElementById('btnProximo').addEventListener('click', function () {
    paginaAtual++;
    buscarProgresso();
});

document.getElementById('btnAnterior').addEventListener('click', function () {
    if (paginaAtual > 0) paginaAtual--;
    buscarProgresso();
});

window.onload = async function () {
    await buscarProcessoCount();
    buscarProgresso();
    setInterval(buscarProgresso, 3000);
};