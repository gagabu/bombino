FROM mcr.microsoft.com/dotnet/core/aspnet:3.1.4 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1.202 AS build
WORKDIR /src
COPY ["BombinoBomberBot.csproj", "./"]
RUN dotnet restore "BombinoBomberBot.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "BombinoBomberBot.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "BombinoBomberBot.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "BombinoBomberBot.dll"]
