# escape=`
FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src

# optimize cache by performing restore with against csproj file(s)
COPY NuGet.config NuGet.config
COPY Marvin.IDP/Marvin.IDP.csproj Marvin.IDP/
RUN dotnet restore Marvin.IDP/Marvin.IDP.csproj --configfile NuGet.config

# copy in remaining sources
COPY Marvin.IDP/ Marvin.IDP/

WORKDIR /src/Marvin.IDP
RUN dotnet build Marvin.IDP.csproj --no-restore -c Release

FROM build AS publish
RUN dotnet publish Marvin.IDP.csproj --no-build -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Marvin.IDP.dll"]
