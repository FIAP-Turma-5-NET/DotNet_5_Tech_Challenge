FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

COPY . ./

RUN dotnet publish "./src/FIAP_Contato.API" -c Release -o /app/out/api
RUN dotnet publish "./src/WorkerContato/FIAP_Contato.Consumer.csproj" -c Release -o /app/out/worker

# Imagem final da API
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS api
WORKDIR /app
COPY --from=build-env /app/out/api ./
ENV ASPNETCORE_ENVIRONMENT="Development"
EXPOSE 8080
EXPOSE 443
ENTRYPOINT ["dotnet", "FIAP_Contato.API.dll"]

# Imagem final do Worker
FROM mcr.microsoft.com/dotnet/runtime:8.0 AS worker
WORKDIR /app
COPY --from=build-env /app/out/worker ./
ENV DOTNET_ENVIRONMENT="Development"
ENTRYPOINT ["dotnet", "FIAP_Contato.Consumer.dll"]


