# FiapCloudGames.Users

## 📌 Objetivos
Microsserviço de usuários do Monólito [FiapCloudGames](https://github.com/MarioGuilherme/FiapCloudGames) que trata todas as regras e lógicas pertinente ao escopo de usuários como login, cadastro e gerenciamento de perfil, juntamente com o sua base de dados;

## 🚀 Instruções de uso
Faça o clone do projeto e já acesse a pasta do projeto clonado:
```
git clone https://github.com/MarioGuilherme/FiapCloudGames.Users && cd .\FiapCloudGames.Users
```

### ▶️ Iniciar Projeto
  1 - Navegue até o diretório da camada API da aplicação:
  ```
  cd .\FiapCloudGames.Users.API\
  ```
  2 - Insira o comando de execução do projeto:
  
  _(O BANCO DE DADOS É CRIADO AUTOMATICAMENTE QUANDO O PROJETO É INICIADO, SEM PRECISAR EXECUTAR O ```Database-Update```)_:
  ```
  dotnet run
  ```
  3 - Acesse https://localhost:7246/swagger/index.html

### 🧪 Executar testes
  1 - Navegue até o diretório dos testes:
  ```
  cd .\FiapCloudGames.Users.Tests\
  ```
  2 - E insira o comando de execução de testes:
  ```
  dotnet test
  ```

## 🛠️ Tecnologias e Afins
- .NET 8 com C# 12;
- ASP.NET Core;
- Uso de Middlewares e IActionFilters;
- EntityFrameworkCore;
- SQL SERVER;
- FluentValidation;
- Swagger;
- xUnit junto com Moq;
- Autenticação JWT;
- Segurança de Criptografia com BCrypt;
