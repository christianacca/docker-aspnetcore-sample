# escape=`
FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src

# optimize cache by performing restore with against csproj file(s)
COPY ["NuGet.config", "Directory.Build.props", "./"]
COPY ImageGallery.Model/ImageGallery.Model.csproj ImageGallery.Model/
COPY ImageGallery.API/ImageGallery.API.csproj ImageGallery.API/
RUN dotnet restore ImageGallery.API/ImageGallery.API.csproj --configfile NuGet.config

# copy in remaining sources
COPY ImageGallery.Model/ ImageGallery.Model/
COPY ImageGallery.API/ ImageGallery.API/

WORKDIR /src/ImageGallery.API
RUN dotnet build ImageGallery.API.csproj --no-restore -c Release

FROM build AS publish
RUN dotnet publish ImageGallery.API.csproj --no-build -c Release -o /app

FROM base AS final
WORKDIR /app

# start-period of 90s hopefully enough time for db migrations to run during app startup
HEALTHCHECK --interval=30s --timeout=20s --start-period=90s `
	CMD curl --fail http://localhost/api/health || exit
ENTRYPOINT ["dotnet", "ImageGallery.API.dll"]
COPY --from=publish /app .
