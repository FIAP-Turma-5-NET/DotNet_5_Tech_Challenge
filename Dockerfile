FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

COPY . ./

RUN dotnet publish "./FIAP_Contato.API" -c Release -o /app/out/api
RUN dotnet publish "./WorkerContato/FIAP_Contato.Consumer.csproj" -c Release -o /app/out/console

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

COPY --from=build-env /app/out/api .
COPY --from=build-env /app/out/console ./console

ENV ASPNETCORE_ENVIRONMENT="Development"

EXPOSE 8080
EXPOSE 443

ENTRYPOINT ["dotnet", "FIAP_Contato.API.dll"]