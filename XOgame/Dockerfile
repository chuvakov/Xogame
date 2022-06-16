FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["XOgame/XOgame.csproj", "XOgame/"]
RUN dotnet restore "XOgame/XOgame.csproj"
COPY . .
WORKDIR "/src/XOgame"
RUN dotnet build "XOgame.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "XOgame.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "XOgame.dll"]
