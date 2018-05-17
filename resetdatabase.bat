cd .\Project-Hexagonal-Astral-Islands\Project-Hexagonal-Astral-Islands
dotnet ef database drop
dotnet ef migrations remove
dotnet ef migrations add InitialCreate
dotnet ef database update