FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

COPY . ./

RUN dotnet publish "./FIAP_Contato.API" -c Release -o /app/out/api
RUN dotnet publish "./WorkerContato/FIAP_Contato.Consumer.csproj" -c Release -o /app/out/console

FROM mcr.microsoft.com/dotnet/aspnet:8.0

# Instalar o Supervisor
RUN apt-get update && apt-get install -y supervisor

WORKDIR /app

COPY --from=build-env /app/out/api ./
COPY --from=build-env /app/out/console ./console

COPY supervisord.conf /etc/supervisor/supervisord.conf

EXPOSE 8080
EXPOSE 443

# Comando para iniciar o Supervisor que gerencia a API e o Console
CMD ["/usr/bin/supervisord", "-c", "/etc/supervisor/supervisord.conf"]