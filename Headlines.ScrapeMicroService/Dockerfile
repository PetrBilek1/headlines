#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG package_pass

WORKDIR /src

RUN echo "<configuration><packageSources><add key=\"github\" value=\"https://nuget.pkg.github.com/PetrBilek1/index.json\" /></packageSources> <packageSourceCredentials><github><add key=\"Username\" value=\"PetrBilek1\" /><add key=\"ClearTextPassword\" value=\"${package_pass}\" /></github></packageSourceCredentials></configuration>" > nuget.config

COPY ["Headlines.ScrapeMicroService/Headlines.ScrapeMicroService.csproj", "Headlines.ScrapeMicroService/"]
RUN dotnet restore "Headlines.ScrapeMicroService/Headlines.ScrapeMicroService.csproj"
COPY . .
WORKDIR "/src/Headlines.ScrapeMicroService"
RUN dotnet build "Headlines.ScrapeMicroService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Headlines.ScrapeMicroService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final

WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Headlines.ScrapeMicroService.dll"]