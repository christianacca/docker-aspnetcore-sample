# escape=`
FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src

# optimize cache by performing restore with against csproj file(s)
COPY NuGet.config NuGet.config
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
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "ImageGallery.API.dll"]
