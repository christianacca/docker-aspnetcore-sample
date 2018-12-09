# escape=`
FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src

# optimize cache by performing restore with against csproj file(s)
COPY NuGet.config NuGet.config
COPY ImageGallery.Model/ImageGallery.Model.csproj ImageGallery.Model/
COPY ImageGallery.Client/ImageGallery.Client.csproj ImageGallery.Client/
RUN dotnet restore ImageGallery.Client/ImageGallery.Client.csproj --configfile NuGet.config

# copy in remaining sources
COPY ImageGallery.Model/ ImageGallery.Model/
COPY ImageGallery.Client/ ImageGallery.Client/

WORKDIR /src/ImageGallery.Client
RUN dotnet build ImageGallery.Client.csproj --no-restore -c Release

FROM build AS publish
RUN dotnet publish ImageGallery.Client.csproj --no-build -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "ImageGallery.Client.dll"]
