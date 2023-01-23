#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Headlines.RSSProcessingMicroService/Headlines.RSSProcessingMicroService.csproj", "Headlines.RSSProcessingMicroService/"]
COPY ["Headlines.DependencyResolution/Headlines.DependencyResolution.csproj", "Headlines.DependencyResolution/"]
COPY ["Headlines.ORM.Core/Headlines.ORM.Core.csproj", "Headlines.ORM.Core/"]
COPY ["Headlines.Enums/Headlines.Enums.csproj", "Headlines.Enums/"]
COPY ["Headlines.DTO/Headlines.DTO.csproj", "Headlines.DTO/"]
COPY ["Headlines.BL/Headlines.BL.csproj", "Headlines.BL/"]
RUN dotnet restore "Headlines.RSSProcessingMicroService/Headlines.RSSProcessingMicroService.csproj"
COPY . .
WORKDIR "/src/Headlines.RSSProcessingMicroService"
RUN dotnet build "Headlines.RSSProcessingMicroService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Headlines.RSSProcessingMicroService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Headlines.RSSProcessingMicroService.dll"]