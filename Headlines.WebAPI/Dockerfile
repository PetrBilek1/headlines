FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

COPY ["Headlines.WebAPI/Headlines.WebAPI.csproj", "Headlines.WebAPI/"]
RUN dotnet restore "Headlines.WebAPI/Headlines.WebAPI.csproj"
COPY . .
WORKDIR "/src/Headlines.WebAPI"
RUN dotnet build "Headlines.WebAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Headlines.WebAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Headlines.WebAPI.dll"]