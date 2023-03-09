# CleanArchitecture

![C#](https://img.shields.io/badge/c%23-%23239120.svg?style=for-the-badge&logo=c-sharp&logoColor=white)  ![TypeScript](https://img.shields.io/badge/typescript-%23007ACC.svg?style=for-the-badge&logo=typescript&logoColor=white) ![AWS](https://img.shields.io/badge/AWS-%23FF9900.svg?style=for-the-badge&logo=amazon-aws&logoColor=white) ![Azure](https://img.shields.io/badge/azure-%230072C6.svg?style=for-the-badge&logo=azure-devops&logoColor=white)  ![.Net](https://img.shields.io/badge/.NET-5C2D91?style=for-the-badge&logo=.net&logoColor=white)  ![Bootstrap](https://img.shields.io/badge/bootstrap-%23563D7C.svg?style=for-the-badge&logo=bootstrap&logoColor=white)   ![NPM](https://img.shields.io/badge/NPM-%23000000.svg?style=for-the-badge&logo=npm&logoColor=white) ![NodeJS](https://img.shields.io/badge/node.js-6DA55F?style=for-the-badge&logo=node.js&logoColor=white) ![React](https://img.shields.io/badge/react-%2320232a.svg?style=for-the-badge&logo=react&logoColor=%2361DAFB) ![MySQL](https://img.shields.io/badge/mysql-%2300f.svg?style=for-the-badge&logo=mysql&logoColor=white)  ![Docker](https://img.shields.io/badge/docker-%230db7ed.svg?style=for-the-badge&logo=docker&logoColor=white) ![Postman](https://img.shields.io/badge/Postman-FF6C37?style=for-the-badge&logo=postman&logoColor=white) ![Swagger](https://img.shields.io/badge/-Swagger-%23Clojure?style=for-the-badge&logo=swagger&logoColor=white)

<!-- add image -->

## Description

This is a sample project that implements Clean Architecture in ASP.NET Core by using principles of Tactical DDD (Domain Driver Design).

The RESTful Api implements authentication and authorization using Access Tokens and Silent Authentication. Also implements role based authorization using IdentityDbContext. The React SPA client enables 'Create', 'Update' and 'Delete' operations for Products Admin user and 'Read' for Anonymous users. Cart persists across sessions. Front-end uses React/Typescript for sorting and filtering of products.

<br />
<div align="center">
  <a href="">
    <img src="img/2019-01-26 Clean Architecture Diagram.png" alt="Logo" width="50%" height="auto">
  </a>
</div>

<!-- TABLE OF CONTENTS -->
<details>
  <summary>Table of Contents</summary>
  
  <ol>
    <li>
      <a href="#features">Features</a>
      <ul>
        <li><a href="#authentication-and-authorization">Authentication and Authorization</a></li>
        <li><a href="#presentation-layer">Presentation</a></li>
        <li><a href="#application-layer">Application</a></li>
        <li><a href="#domain-layer">Domain</a></li>
        <li><a href="#infrastructure-layer">Infrastructure</a></li>
      </ul>
    </li>
    <li>
      <a href="#getting-started">Getting Started</a>
      <ul>
        <li><a href="#prerequisites">Prerequisites</a></li>
        <li><a href="#installation">Installation</a></li>
        <li><a href="#usage">Usage</a></li>
        <ul>
          <li><a href="#swagger">Swagger</a></li>
        </ul>
      </ul>
    </li>
  </ol>
</details>

## Features

### Authentication and Authorization

- Access Tokens
  - **JWT  Tokens** are used for authentication and authorization. The tokens are passed in the cookie header and are sent with every request to the API server. The tokens are validated using the ASP.NET Core Identity framework in the Infrastructure Layer. They consist of claims that identify the user and the roles they belong to. The tokens are signed using a secret key that is stored in the appsettings.json file[^secretdotjson]. The tokens are valid for 30 minutes and are refreshed automatically when the user is active. The tokens are invalidated when the user logs out or when the user's password is changed.
- Silent Authentication
  - The user is automatically logged in when the token expires. The user is not prompted to enter their credentials again. The user is redirected to the login page when the token expires and the user is not active. **Refresh tokens** are used to refresh the access tokens. The refresh tokens are stored in the database and are valid for 7 days. The refresh tokens are invalidated when the user logs out or when the user's password is changed.

[^secretdotjson]: The secret key should be stored in a secure location and not in the appsettings.json file. The secret key is used to sign the tokens and should be kept secret. The secret key can be stored in the Azure Key Vault or in the Azure App Configuration service.

### Presentation layer

- Controllers
  - handle the incoming HTTP requests and return the appropriate HTTP responses and status codes conforming to the specified **Contracts**.
  - Validate the incoming requests.
  - Call Application Layer services to perform the business logic.
  - The responses conform to the **Problem Details** specification.
- React SPA
  - The client is hosted in the same project as the API server[^samesite]. The client is served by the API server when the user navigates to the root URL. The client uses the React Bootstrap library to implement the UI.

[^samesite]: SameSite attribute is set as Strict to prevent CSRF attacks.

### Application Layer

- Services
  - The services direct the expressive Domain objects in the Domain Layer to carry out business logic. Invoke repositories for persistence needs. Called by the controllers in **Presentation Layer**. The services are injected with the required dependencies using the **IoC Container**. The services handle all their persistence through **Repositories**.
- Mediator
  - The Mediator pattern is used to implement the CQRS pattern. The Mediator is injected into the services and is used to send commands and queries to the Command and Query Handlers. The Command and Query Handlers are injected with the required dependencies using the **IoC Container**. The Command and Query Handlers handle all their persistence through **Repositories**. The Command and Query Handlers implement the **Unit of Work** pattern to handle transactional behavior. The **Entity Framework Core** is used as the ORM.
- Mappings
  - The **Mapster** library is used to map the domain entities to DTOs and vice versa.
- Validation
  - The **FluentValidation** library is used to implement the validation of incoming requests. The validation rules are specified in the DTOs. The validation rules are executed automatically by the pipeline and the appropriate HTTP status codes are returned when the validation fails[^goto].

[^goto]: Exceptions need to be caught is try-catch blocks, so they behave like a goto statement which is considered bad practice and should be avoided.

### Domain Layer

- Responsible for representing the business logic and the business rules. State that reflects the business situation is controlled here and the technical details of storing it are delegated to the Infrastructure Layer. The Domain Layer is not aware of the other layers. The Domain Layer is not aware of
  - the database or the ORM (Infrastructure).
  - the UI or the API (Presentation).
  - the Application services.

- Entities
  - Have a thread of continuity and identity.
  - Has a lifecycle and passes through multiple forms.
  - Examples: Product, Order, Customer, Transaction, etc.
- Value Objects
  - They describe the state of an entity.
  - They are immutable.
- Aggregates
  - They are a cluster of associated objects that we treat as a unit for the purpose of data changes.
    - Define clear ownership boundaries.
    - They are designed to minimize the amount of interference in concurrent transactional operations.
  - They are the root of the object graph.
  - They mark the scope within which invariants are enforced.

### Infrastructure Layer

- The Infrastructure Layer is responsible for the implementation details of the application. It is the lowest layer in the application. It is responsible for the following:
  - Persistence
  - Logging
  - Caching
  - Messaging
  - Configuration
  - Authentication and Authorization
  - External Services
- The Infrastructure Layer is aware of the other layers. It is aware of the Domain Layer and the Application Layer. It is not aware of the Presentation Layer.

- Dependency Inversion
  - High-level modules should not depend on low-level modules. Both should depend on abstractions.
  - Abstractions should not depend on details. Details should depend on abstractions.

<div align="middle">
    <img src="img/Screenshot 2023-03-08 163827.png" width="300" />
    <p>Fig.1 - Traditional Layered Architecture.</p>
</div>
<div align="middle">
    <img src="img/Screenshot 2023-03-08 163946.png" width="300" />
    <p>Fig.2 - Dependency Inverison.</p>
</div>

# Getting Started

## Prerequisites

- [.NET Core 7](https://dotnet.microsoft.com/download/dotnet-core/7.0)
- [Visual Studio 2022](https://visualstudio.microsoft.com/downloads/) or [Visual Studio Code](https://code.visualstudio.com/download)

## Installation

1. Clone the repo

```sh
git clone
```

2. Install NuGet packages

```sh
dotnet restore
```

3. Add dev secret to WebApp Secret.json

```json
{
    JwtTokenOptions: {
        Secret: "super-secret-32-characters-longg"
    }
}
```

4. Run the project

```sh
dotnet run
```

## Usage

### Swagger

- Open the browser and navigate to `https://localhost:<PORT>/swagger/index.html` (Development environment)
