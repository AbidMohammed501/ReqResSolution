# ReqRes API Integration (.NET 8+)

This project demonstrates a clean, testable .NET 6 integration with the public [ReqRes API](https://reqres.in), built as part of a coding assignment.

---

## ✅ Project Structure
ReqResSolution/
├── ReqResApiIntegration/ # Class Library (Core Models + Services)
├── ReqResApi.WebApi/ # ASP.NET Core Web API Host
├── ReqResApi.Tests/ # Unit Test Project (xUnit + Moq)
├── ReqResApi.ConsoleDemo/ # Optional Console App for demo
└── README.md # Instructions & Design

1. Clone the repository
git clone <your-repo-url>
cd ReqResSolution

2. Restore & build:
dotnet restore
dotnet build

3. Run Web API:
cd ReqResApi.WebApi
dotnet run
Access Swagger UI at https://localhost:<port>/swagger.

4. Run Console Demo:
cd ReqResApi.ConsoleDemo
dotnet run

Configuration
Set the following in appsettings.json:
"ReqResApi": {
  "BaseUrl": "https://reqres.in/api",
  "ApiKey": "reqres-free-v1",
  "CacheExpirationSeconds": 60
}

5. Running Tests
cd ReqResApi.Tests
dotnet test




