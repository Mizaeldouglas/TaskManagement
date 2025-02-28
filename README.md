# Task Management Application

Uma aplicação completa de gerenciamento de tarefas construída com .NET 9, composta por uma API Web ASP.NET Core backend seguindo os princípios da Arquitetura Limpa e um frontend desktop WPF usando o padrão MVVM.

## Visão Geral do Projeto

Esta aplicação permite aos usuários:
- Criar, ler, atualizar e excluir tarefas
- Filtrar tarefas por status (Pendente, Em Andamento, Concluída)
- Gerenciar detalhes das tarefas através de uma interface amigável

## Arquitetura

### Backend (ASP.NET Core Web API)
O backend segue os princípios da Arquitetura Limpa com as seguintes camadas:
1. **Camada de Domínio**: Contém entidades, enums, exceções, interfaces, tipos e lógica
2. **Camada de Aplicação**: Contém lógica de negócios e interfaces
3. **Camada de Infraestrutura**: Contém configurações de banco de dados, migrações e implementações
4. **Camada de Apresentação (API)**: Contém controladores e endpoints da API

### Frontend (WPF)
O frontend segue o padrão MVVM (Model-View-ViewModel):
1. **Models**: Estruturas de dados que representam tarefas
2. **ViewModels**: Classes que manipulam a lógica e o estado da UI
3. **Views**: Componentes de UI XAML
4. **Services**: Classes que lidam com a comunicação com a API

## Começando

### Pré-requisitos

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [SQL Server](https://www.microsoft.com/pt-br/sql-server/sql-server-downloads)
- [Visual Studio 2022](https://visualstudio.microsoft.com/pt-br/vs/) ou [Visual Studio Code](https://code.visualstudio.com/)

## Configuração e Instalação

### Configurando o Banco de Dados

#### Opção 1: Configuração via appsettings.json
1. Navegue até o projeto `TaskManagement.Api` e abra o arquivo `appsettings.json`
2. Localize a seção `ConnectionStrings` e atualize a string de conexão conforme necessário:
```
"ConnectionStrings": 
{ 
"DefaultConnection": "Server=SEU_SERVIDOR;Database=TaskManagementDb;User Id=SEU_USUARIO;Password=SUA_SENHA;TrustServerCertificate=True;MultipleActiveResultSets=true" 
}
```
#### Opção 2: Migração via Visual Studio
1. Abra a solução no Visual Studio
2. No Console do Gerenciador de Pacotes (Menu: Ferramentas > Gerenciador de Pacotes NuGet > Console do Gerenciador de Pacotes):
   - Defina o projeto padrão como `TaskManagement.Infrastructure` 
   - Execute o comando: `Update-Database`

#### Opção 3: Migração via CLI .NET
1. Abra um terminal no diretório raiz da solução
2. Execute o comando:`dotnet ef database update --project src/TaskManagement.Infrastructure --startup-project src/TaskManagement.Api`

### Executando a API Backend

#### Usando Visual Studio
1. Defina `TaskManagement.Api` como o projeto de inicialização (clique com o botão direito no projeto > Definir como Projeto de Inicialização)
2. Pressione F5 para executar a aplicação em modo de depuração ou Ctrl+F5 para executar sem depuração
3. A API estará disponível em `https://localhost:7001` 
4. Você pode acessar a documentação Swagger em `https://localhost:7001/swagger`

#### Usando CLI .NET
1. Navegue até o diretório do projeto API:´cd src/TaskManagement.Api´
2. Execute o comando: `dotnet run`

### Configurando a Conexão do Frontend com a API
1. Abra o projeto `TaskManagement.WPF`
2. Navegue até o arquivo `appsettings.json` e verifique a configuração da URL da API: 
```
"ApiSettings": "https://localhost:7001/api/tasks"
```
3. Se a API estiver rodando em um endereço diferente, atualize o valor de `BaseUrl` adequadamente

### Executando o Frontend WPF

#### Usando Visual Studio
1. Defina `TaskManagement.WPF` como o projeto de inicialização
2. Pressione F5 para executar a aplicação
3. Certifique-se de que a API backend esteja em execução antes de iniciar a aplicação WPF

#### Usando CLI .NET
1. Navegue até o diretório do projeto WPF: `cd src/TaskManagement.WPF`
2. Execute o comando: `dotnet run`


### Verificando a Instalação
1. Após iniciar a API, acesse o Swagger em `https://localhost:7001/swagger` para verificar se a API está funcionando corretamente
2. No aplicativo WPF, você deve ser capaz de:
   - Ver a lista de tarefas (vazia inicialmente)
   - Criar novas tarefas
   - Editar e excluir tarefas existentes
   - Filtrar tarefas por status

### Solução de Problemas Comuns

#### Erro de Conexão com o Banco de Dados
- Verifique se a string de conexão em `appsettings.json` está correta
- Certifique-se de que o SQL Server está em execução
- Verifique se o usuário definido na string de conexão tem permissões adequadas

#### API não está acessível
- Confirme se o projeto da API está em execução
- Verifique se não há outro serviço usando a mesma porta (7001)
- Caso use HTTPS, confie no certificado de desenvolvimento quando solicitado

#### WPF não consegue se conectar à API
- Verifique se a URL da API no `appsettings.json` do projeto WPF corresponde à URL onde a API está sendo executada
- Certifique-se de que não há regras de firewall bloqueando a conexão

   
## Endpoints da API

| Método | Endpoint                 | Descrição                             |
|--------|--------------------------|---------------------------------------|
| GET    | /api/tasks               | Obter todas as tarefas                |
| GET    | /api/tasks/{id}          | Obter tarefa por ID                   |
| GET    | /api/tasks/status/{status}| Obter tarefas por status             |
| POST   | /api/tasks               | Criar uma nova tarefa                 |
| PUT    | /api/tasks/{id}          | Atualizar uma tarefa existente        |
| DELETE | /api/tasks/{id}          | Excluir uma tarefa                    |
| PATCH  | /api/tasks/{id}/status   | Atualizar o status de uma tarefa      |

### Exemplos de Requisição/Resposta

#### Criar Tarefa (POST /api/tasks)
Requisição:
```
{ 
	"title": "Nova Tarefa",
	"description": "Descrição da nova tarefa",
	"status": 0
}
```
Resposta:
```
{
	"id": 1,
	"title": "Nova Tarefa",
	"description": "Descrição da nova tarefa",
	"status": 0
}
```
#### Atualizar Tarefa (PUT /api/tasks/1)
Requisição:
```
{
	"id": 1, 
	"title": "Completar documentação do projeto", 
	"description": "Finalizada a escrita da arquitetura e instruções de configuração", 
	"status": "Completed"
}
```
Resposta:
```
{
	 "id": 1, 
	 "title": "Completar documentação do projeto",
	 "description": "Finalizada a escrita da arquitetura e instruções de configuração", 
	 "creationDate": "2023-05-15T11:25:07Z", 
	 "completionDate": "2023-05-15T11:35:22Z", 
	 "status": "Completed"
}
```
#### Atualizar Status da Tarefa (PATCH /api/tasks/1/status)
Requisição:
```
{
	"id": 1
	"status": "InProgress"
}
```
Resposta:
```
{
	"id": 1, "title": "Completar documentação do projeto", 
	"description": "Finalizar a escrita da arquitetura e instruções de configuração", 
	"creationDate": "2023-05-15T11:25:07Z", 
	"completionDate": null,
	"status": "InProgress"
}
```

## Estrutura do Projeto
```
TaskManagement/ 
├── src/    
├── TaskManagement.Domain/           # Entidades, interfaces, regras de negócio 
│   ├── TaskManagement.Application/      # Casos de uso, DTOs, interfaces 
│   ├── TaskManagement.Infrastructure/   # Acesso a dados, repositórios 
│   ├── TaskManagement.Api/              # Controladores de API, configurações 
│   └── TaskManagement.WPF/              # Aplicação frontend WPF 
└── tests/                               # Projetos de teste (adição futura)
```

## Tecnologias Utilizadas

### Backend
- ASP.NET Core 9.0
- Entity Framework Core 8.0
- SQL Server
- MediatR (padrão CQRS)
- AutoMapper
- FluentValidation
- Swagger/OpenAPI

### Frontend
- WPF (.NET 9.0)
- Padrão MVVM
- CommunityToolkit.Mvvm
- Microsoft.Extensions.DependencyInjection
- HttpClient


## Tratamento de Exceções

A aplicação inclui um middleware personalizado para tratamento de exceções que:
- Captura e processa diferentes tipos de exceções
- Retorna códigos de status HTTP apropriados (400, 404, 500)
- Registra detalhes da exceção para depuração
- Formata respostas de erro consistentes para o cliente

## Autor

- **Mizael Douglas**