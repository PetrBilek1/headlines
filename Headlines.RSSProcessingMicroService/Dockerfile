FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
RUN useradd --create-home nonroot

WORKDIR /app

USER nonroot

EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG package_pass

WORKDIR /src

RUN echo "<configuration><packageSources><add key=\"github\" value=\"https://nuget.pkg.github.com/PetrBilek1/index.json\" /></packageSources> <packageSourceCredentials><github><add key=\"Username\" value=\"PetrBilek1\" /><add key=\"ClearTextPassword\" value=\"${package_pass}\" /></github></packageSourceCredentials></configuration>" > nuget.config

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