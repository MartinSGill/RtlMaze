FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["RtlMazeApp/RtlMazeApp.csproj", "RtlMazeApp/"]
RUN dotnet restore "RtlMazeApp/RtlMazeApp.csproj"
COPY . .
WORKDIR "/src/RtlMazeApp"
RUN dotnet build "RtlMazeApp.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "RtlMazeApp.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "RtlMazeApp.dll"]