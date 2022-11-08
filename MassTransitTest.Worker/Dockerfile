#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["MassTransitTest.Worker/MassTransitTest.Worker.csproj", "MassTransitTest.Worker/"]
RUN dotnet restore "MassTransitTest.Worker/MassTransitTest.Worker.csproj"
COPY . .
WORKDIR "/src/MassTransitTest.Worker"
RUN dotnet build "MassTransitTest.Worker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MassTransitTest.Worker.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MassTransitTest.Worker.dll"]