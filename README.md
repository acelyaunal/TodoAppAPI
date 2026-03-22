#  TodoAppAPI

Production-ready Todo Management API built with ASP.NET Core using Clean Architecture principles.

---

##  Overview

A secure and scalable RESTful API that allows users to manage their personal todo items.  
Each user can only access and modify their own data.

---

##  Tech Stack

- .NET 9 / ASP.NET Core Web API  
- Entity Framework Core  
- SQLite  
- JWT Authentication  
- FluentValidation  
- xUnit (Unit & Integration Tests)  

---

## Features

- JWT-based authentication (Register / Login)  
- User-based authorization (data isolation)  
- CRUD operations for Todos  
- Clean Architecture (Domain, Application, Infrastructure, WebAPI)  
- Request validation (FluentValidation)  
- Global error handling (ProblemDetails)  
- Rate limiting  
- Logging & health check endpoint  

---

##  Run Locally

```bash
git clone https://github.com/YOUR_USERNAME/TodoAppAPI.git
cd TodoAppAPI

dotnet restore

dotnet user-secrets init --project WebAPI
dotnet user-secrets set "Jwt:Key" "SuperSecretKey123456789SuperSecretKey123456789" --project WebAPI
dotnet user-secrets set "Jwt:Issuer" "TodoAppAPI" --project WebAPI
dotnet user-secrets set "Jwt:Audience" "TodoAppAPIUsers" --project WebAPI

dotnet ef database update --project Infrastructure --startup-project WebAPI

dotnet run --project WebAPI
