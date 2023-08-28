# Użyj obrazu runtime .NET 7.0 jako bazowego obrazu
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 40043

# Ustaw zmienne środowiskowe związane z połączeniem do bazy danych
ENV ConnectionStrings__DefaultConnection="Server=mws02.mikr.us;Port=50005;Database=ReaderPathDb;Uid=root;Pwd=NeFezZxsUW;Charset=utf8mb4;"

# Kopiuj pliki aplikacji do kontenera
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /app
COPY ["Readerpath.csproj", "./"]
RUN dotnet restore "Readerpath.csproj"
COPY . .
WORKDIR "/app"
RUN dotnet build "Readerpath.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Readerpath.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Readerpath.dll"]
