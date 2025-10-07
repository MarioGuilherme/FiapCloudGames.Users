# Acesse https://aka.ms/customizecontainer para saber como personalizar seu contêiner de depuração e como o Visual Studio usa este Dockerfile para criar suas imagens para uma depuração mais rápida.

# Esta fase é usada durante a execução no VS no modo rápido (Padrão para a configuração de Depuração)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# Esta fase é usada para compilar o projeto de serviço
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["FiapCloudGames.Users.API/FiapCloudGames.Users.API.csproj", "FiapCloudGames.Users.API/"]
COPY ["FiapCloudGames.Users.Application/FiapCloudGames.Users.Application.csproj", "FiapCloudGames.Users.Application/"]
COPY ["FiapCloudGames.Users.Domain/FiapCloudGames.Users.Domain.csproj", "FiapCloudGames.Users.Domain/"]
COPY ["FiapCloudGames.Users.Infrastructure/FiapCloudGames.Users.Infrastructure.csproj", "FiapCloudGames.Users.Infrastructure/"]
RUN dotnet restore "./FiapCloudGames.Users.API/FiapCloudGames.Users.API.csproj"
COPY . .
WORKDIR "/src/FiapCloudGames.Users.API"
RUN dotnet build "./FiapCloudGames.Users.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Install the agent
RUN apt-get update && apt-get install -y wget ca-certificates gnupg \
&& echo 'deb http://apt.newrelic.com/debian/ newrelic non-free' | tee /etc/apt/sources.list.d/newrelic.list \
&& wget https://download.newrelic.com/548C16BF.gpg \
&& apt-key add 548C16BF.gpg \
&& apt-get update \
&& apt-get install -y 'newrelic-dotnet-agent' \
&& rm -rf /var/lib/apt/lists/*

# Enable the agent
ENV CORECLR_ENABLE_PROFILING=1 \
CORECLR_PROFILER={36032161-FFC0-4B61-B559-F6C5D41BAE5A} \
CORECLR_NEWRELIC_HOME=/usr/local/newrelic-dotnet-agent \
CORECLR_PROFILER_PATH=/usr/local/newrelic-dotnet-agent/libNewRelicProfiler.so \
NEW_RELIC_LICENSE_KEY=7a51751afd870f032bf67a4ad3600db4FFFFNRAL \
NEW_RELIC_APP_NAME="fiapusers"


# Esta fase é usada para publicar o projeto de serviço a ser copiado para a fase final
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./FiapCloudGames.Users.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Esta fase é usada na produção ou quando executada no VS no modo normal (padrão quando não está usando a configuração de Depuração)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FiapCloudGames.Users.API.dll"]