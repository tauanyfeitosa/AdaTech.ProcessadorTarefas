# AdaTech.ProcessadorTarefas

## Sobre o Projeto
O AdaTech.ProcessadorTarefas é uma aplicação desenvolvida com fins educativos, visando meu apredizado sobre condições de corrida, paralelismo, concorrência controlada e programação assíncrona. Com certeza você já deve ter se perguntado a primeira vez que se deparou com: mas em que isso se aplica?
Como nos foi passado um escorpo bem aberto para estudo (embora restritivo em outras), decidi fazer algo mais visual e me aventurar pela primeira vez em JavaScript. Então, essa aplicação Web concentra sua visualização em algo parecido com downloads de vários arquivos. Como nosso computador ou aparelho processa tudo isso? Por que uns parecem acabar primeiro que outros? Fique a vontade para testar e mandar contribuições se preferir. Abaixo, deixarei instruções e explicações sobre a aplicação.

Basicamente, este projeto funciona com a seguinte lógica: o sistema gera 100 Processos para serem executados, cada processo recebe de 10 a 100 tarefas (totalmente aleatórias). Os processos são executados de acordo com o número de núcleos do seu computador, através do Enviroment.Count().
As Tarefas de cada processo são executadas de 5 em 5, com um delay de execução variando de 3 a 60 segundos (também aleatório).

## Iniciando a Aplicação - Como Configurar
Para começar a usar o AdaTech.ProcessadorTarefas, siga os passos abaixo:

1. Clone o repositório:
    ```bash
    git clone https://github.com/tauanyfeitosa/AdaTech.ProcessadorTarefas.git
    ```
As configurações básicas já estão implementadas, não é necessário banco de dados, os dados estão sendo tratados em memória!

## Como Usar
Para usar o AdaTech.ProcessadorTarefas, siga estas etapas:

1. Inicie a aplicação: Abra a aplicação em uma IDE de sua preferência!

2. Acesse a interface do usuário em um navegador ou utilize a API para enviar tarefas para processamento (detalhes específicos sobre como interagir com a aplicação). Caso esteja utilizando Visual Studio, basta iniciar a aplicação que ela abrirá no navegador.
   
4. Pronto! Bem-vindo ao Swagger!!!

## Swagger - O que é? Como funciona?

Swagger é uma ferramenta de interface para descrever, produzir, consumir e visualizar serviços da Web RESTful. Ele especifica um formato padrão para APIs REST, o que facilita a compreensão e o uso dos endpoints por desenvolvedores e usuários finais.

No ProcessadorTarefas API, utilizamos o Swagger para fornecer uma documentação interativa da API. Através do Swagger UI, é possível visualizar todos os endpoints disponíveis, seus métodos HTTP, parâmetros necessários e os formatos de resposta esperados.

![Swagger UI]([url-da-imagem-1](https://drive.google.com/file/d/1jc_JYsn7X1v42_6AWnsWjTpHrtF5uptZ/view?usp=sharing))

### Como usar um endpoint

Para usar um endpoint do ProcessadorTarefas API via Swagger UI, siga estes passos:

1. Navegue até o endpoint desejado na documentação do Swagger.
2. Clique no método do endpoint para expandir os detalhes.
3. Se o método requer parâmetros, eles serão listados na seção `Parameters`. Caso não haja parâmetros, como no endpoint `/api/Processamento/iniciar`, você pode simplesmente executar o endpoint diretamente.

![Detalhes do Endpoint]([url-da-imagem-2](https://drive.google.com/file/d/1Z0QFcOOjzvIMixiWkhC77NQgR4AcenwV/view?usp=sharing))

4. Clique no botão `Try it out` para ativar a funcionalidade de teste.
5. Se necessário, insira os parâmetros requeridos.
6. Clique no botão `Execute` para fazer uma chamada ao endpoint.

![Executar Endpoint]([url-da-imagem-3](https://drive.google.com/file/d/1a0Qr09F5hs4iCVS8TLN1sXSdchebGtgt/view?usp=sharing))

### Monitorando o progresso com o endpoint de progresso

O endpoint `/api/Processamento/progresso` permite que você monitore o progresso das tarefas em processamento. Ao executar uma chamada GET para este endpoint, você receberá um retorno em formato JSON contendo informações detalhadas sobre as tarefas, incluindo IDs, títulos, status e porcentagem de conclusão.

![Endpoint de Progresso]([url-da-imagem-4](https://drive.google.com/file/d/1jwxnXUry5zHmqsOVuy49oD8ZzaVwvwO5/view?usp=sharing))

Este endpoint é essencial para acompanhar o andamento das tarefas e garantir que tudo está funcionando conforme esperado.

## Funcionalidades
O AdaTech.ProcessadorTarefas inclui várias funcionalidades chave, como:

- Processamento automático de Processos
- Criação de processos
- Pausar e Retomar Processos
- Cancelar unitariamente ou todo o processamento
- Interface de usuário para gerenciamento de Processos
- API para integração com outros sistemas

Teste a vontade e entre em contato se souber alguma melhoria!!!

## Para melhor visualização:

Utilizando JavaScript e Html, fiz uma interface de usuário para facilitar nos testes (embora seja minha primeira vez com FrontEnd e eu tenha muito em que melhorar)!

Ao iniciar a aplicação, acesse no seu navegador e aproveite:
    ```bash
    [git clone https://github.com/tauanyfeitosa/AdaTech.ProcessadorTarefas.git](https://localhost:7147/swagger/index.html)
    ```

Algumas dicas se tiver problemas com o Front:
1. Caso algo não esteja aparecendo, experimente limpar o cash do seu navegador ou entrar em uma guia anônima!
2. Caso o erro persista para você, clique Fn + 12 e experiemnte verificar os erros de console.

## Como Contribuir
Contribuições são muito bem-vindas! Se você tem interesse em melhorar o AdaTech.ProcessadorTarefas, siga estes passos:

1. Fork o repositório.
2. Crie uma branch para sua feature (`git checkout -b feature/NovaFeature`).
3. Faça suas alterações e commit (`git commit -am 'Adiciona alguma NovaFeature'`).
4. Push para a branch (`git push origin feature/NovaFeature`).
5. Abra um Pull Request.

## Agradecimentos
- Agradeço grandemente ao meu parceiro Victor por me apoiar nessa jornada e sempre me incentivar a continuar tentando! Há três meses eu não sabia como imprimir algo no Console do Visual Studio ou como depurar uma aplicação Console. Hoje, não sei muitas coisas também... mas a lista das coisas que sei não vai parar de aumentar se eu sempre estiver tentando!!!

## Contato

Mande-me um email se precisar: [tauanysanttos13@gmail.com](mailto:tauanysanttos13@gmail.com)
