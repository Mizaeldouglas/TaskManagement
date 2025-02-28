# Task Management Application

[Uma aplicação completa de gerenciamento de tarefas construída com .NET 9](#task-management-application)

## Índice
- [Visão Geral do Projeto](#visão-geral-do-projeto)
- [Arquitetura](#arquitetura)
  - [Backend](#backend-aspnet-core-web-api)
  - [Frontend](#frontend-wpf)
- [Testes Unitários](#testes-unitários)
- [Começando](#começando)
  - [Pré-requisitos](#pré-requisitos)
  - [Configuração e Instalação](#configuração-e-instalação)
- [Endpoints da API](#endpoints-da-api)
- [Estrutura do Projeto](#estrutura-do-projeto)
- [Tecnologias Utilizadas](#tecnologias-utilizadas)
- [Tratamento de Exceções](#tratamento-de-exceções)
- [Screenshots e Videos](#screenshots-e-videos)

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

## Testes Unitários

O projeto inclui testes unitários abrangentes para validar o comportamento das diferentes camadas da aplicação:

1. **Testes da Camada de Domínio**: Validam as regras de negócio e comportamentos das entidades
2. **Testes da Camada de Aplicação**: Verificam a lógica dos handlers de comandos e consultas
3. **Testes da Camada de Infraestrutura**: Testam os repositórios e acesso a dados
4. **Testes da API**: Validam os controladores e endpoints da API


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


### Executando os Testes

#### Usando Visual Studio

1. Abra a solução no Visual Studio
2. Abra o Gerenciador de Testes (Menu: Teste > Gerenciador de Testes)
3. Clique em "Executar Todos os Testes" ou selecione testes específicos para executar
4. Para ver a cobertura de código, vá para Menu: Teste > Analisar Cobertura de Código > Todos os Testes

#### Usando CLI .NET

Para executar todos os testes da solução:
```
dotnet test
```
Para executar testes de um projeto específico:
```
dotnet test tests/TaskManagement.Domain.Tests/TaskManagement.Domain.Tests.csproj
```
Para executar com relatório de cobertura de código:
```
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
```
   
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
├── TaskManagement.Domain/						# Entidades, interfaces, regras de negócio 
│   ├── TaskManagement.Application/					# Casos de uso, DTOs, interfaces 
│   ├── TaskManagement.Infrastructure/					# Acesso a dados, repositórios 
│   ├── TaskManagement.Api/						# Controladores de API, configurações 
│   └── TaskManagement.WPF/						# Aplicação frontend WPF 
└── tests/							# Projetos de teste
	 ├── TaskManagement.Domain.Tests/				# Testes das entidades e regras de domínio 
	 ├── TaskManagement.Application.Tests/				# Testes dos handlers e serviços 
	 ├── TaskManagement.Infrastructure.Tests/			# Testes de repositórios 
	 └── TaskManagement.Api.Tests/					# Testes de integração da API
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

### Teste Unitários

- **xUnit**: Framework principal de testes
- **Moq**: Biblioteca para criação de mocks
- **FluentAssertions**: Para asserções expressivas
- **Coverlet**: Para geração de relatórios de cobertura de código


## Tratamento de Exceções

A aplicação inclui um middleware personalizado para tratamento de exceções que:
- Captura e processa diferentes tipos de exceções
- Retorna códigos de status HTTP apropriados (400, 404, 500)
- Registra detalhes da exceção para depuração
- Formata respostas de erro consistentes para o cliente

### Práticas de Teste Implementadas

- Testes isolados com mocks para dependências externas
- Padrão AAA (Arrange-Act-Assert) para organização de testes
- Dados de teste parametrizados para casos de teste diversos
- Fixtures compartilhadas para configuração de testes relacionados

## Screenshots e Videos


### App WPF
https://github.com/user-attachments/assets/197f2fd9-ed25-4c76-8ff1-a0abf837d4c0

### Swagger
![Swagger UI - Opera 28_02_2025 13_34_58](https://github.com/user-attachments/assets/c8f7e1fa-6f4e-408c-81fa-9a5c99463260)

### Tests


![TaskManagement - Microsoft Visual Studio 28_02_2025 13_38_36](https://github.com/user-attachments/assets/1d4cf6dc-17d9-40b6-9c99-22d079130627)



