FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG package_pass

WORKDIR /src

RUN echo "<configuration><packageSources><add key=\"github\" value=\"https://nuget.pkg.github.com/PetrBilek1/index.json\" /></packageSources> <packageSourceCredentials><github><add key=\"Username\" value=\"PetrBilek1\" /><add key=\"ClearTextPassword\" value=\"${package_pass}\" /></github></packageSourceCredentials></configuration>" > nuget.config

COPY ["Headlines.WebAPI/Headlines.WebAPI.csproj", "Headlines.WebAPI/"]
RUN dotnet restore "Headlines.WebAPI/Headlines.WebAPI.csproj"
COPY . .
WORKDIR "/src/Headlines.WebAPI"
RUN dotnet build "Headlines.WebAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Headlines.WebAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
RUN addgroup -S nonroot \
    && adduser -S nonroot -G nonroot
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Headlines.WebAPI.dll"]