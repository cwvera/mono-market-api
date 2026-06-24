FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY mono.market/MonoMarket.Commons/MonoMarket.Commons.csproj MonoMarket.Commons/
COPY mono.market/MonoMarket.Domain/MonoMarket.Domain.csproj MonoMarket.Domain/
COPY mono.market/MonoMarket.Application/MonoMarket.Application.csproj MonoMarket.Application/
COPY mono.market/MonoMarket.Infrastructure/MonoMarket.Infrastructure.csproj MonoMarket.Infrastructure/
COPY mono.market/MonoMarket.WebApi/MonoMarket.WebApi.csproj MonoMarket.WebApi/
RUN dotnet restore MonoMarket.WebApi/MonoMarket.WebApi.csproj

COPY mono.market/MonoMarket.Commons/ MonoMarket.Commons/
COPY mono.market/MonoMarket.Domain/ MonoMarket.Domain/
COPY mono.market/MonoMarket.Application/ MonoMarket.Application/
COPY mono.market/MonoMarket.Infrastructure/ MonoMarket.Infrastructure/
COPY mono.market/MonoMarket.WebApi/ MonoMarket.WebApi/

RUN dotnet publish MonoMarket.WebApi/MonoMarket.WebApi.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "MonoMarket.WebApi.dll"]
