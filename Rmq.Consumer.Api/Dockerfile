FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Rmq.Consumer.Api/Rmq.Consumer.Api.csproj", "Rmq.Consumer.Api/"]
RUN dotnet restore "./Rmq.Consumer.Api/Rmq.Consumer.Api.csproj"
COPY . .
WORKDIR "/src/Rmq.Consumer.Api"
RUN dotnet build "./Rmq.Consumer.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Rmq.Consumer.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Rmq.Consumer.Api.dll"]