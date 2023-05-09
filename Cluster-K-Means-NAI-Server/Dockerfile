FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Cluster-K-Means-NAI-Server.csproj", "./"]
RUN dotnet restore "Cluster-K-Means-NAI-Server.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "Cluster-K-Means-NAI-Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Cluster-K-Means-NAI-Server.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Cluster-K-Means-NAI-Server.dll"]
