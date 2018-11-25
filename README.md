# ASP.NET Core MVC Docker Sample

This sample demonstrates how to use ASP.NET Core MVC and Docker together.

This sample requires [Docker 17.06](https://docs.docker.com/release-notes/docker-ce) or later of the [Docker client](https://store.docker.com/editions/community/docker-ce-desktop-windows).

Note: original source code copied (with permission) from [SecuringAspNetCore2WithOAuth2AndOIDC](https://github.com/KevinDockx/SecuringAspNetCore2WithOAuth2AndOIDC).
Projects then dockerized by [Christian Crowhurst](https://github.com/christianacca)


## Try a pre-built ASP.NET Docker Image

You can quickly run a container with a pre-built [sample ASP.NET Core Docker image](https://hub.docker.com/r/christianacca/aspnetappcore-sample/).

1. Download / browse to directory containing [docker-compose.yml](docker-compose.yml)
2. Open a powershell prompt in the directory containing the `docker-compose.yml`
3. In the same powershell prompt run: `docker-compose -p aspnetcore-sample up -d`

To browse to the home page of the web app now running in a container:
1. Get the IP address of the container `docker inspect -f '{{range .NetworkSettings.Networks}}{{.IPAddress}}{{end}}' (docker ps -f name=aspnetcore-sample_web-app_1 -q)`
2. Open a browser and navigate to the IP address

To cleanup:

* `docker-compose -p aspnetcore-sample down`


## Getting the sample

The easiest way to get the sample is by cloning the samples repository with [git](https://git-scm.com/downloads), using the following instructions:

```console
git clone https://github.com/christianacca/docker-aspnetcore-sample.git
```

## Build and run the sample with Docker

You can build and run the sample in Docker using the following commands. The instructions assume that you are in the root of the repository.

```powershell
.\build.ps1 Build, UpDev
```
The above command ultimately:
1. builds a new image `christianacca/aspnetappcore-sample` from code in the `src/` directory
2. uses `docker-compose up` to start the web app and it's associated database inside containers
3. open a browser window (chrome) and navigates to the home page of the web app
