# syntax=docker/dockerfile:1.4

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

COPY --from=infra . /Infra.Core

COPY Coacher.sln ./
COPY global.json ./
COPY Coacher.Shared/Coacher.Shared.csproj Coacher.Shared/
COPY Coacher.Lib/Coacher.Lib.csproj Coacher.Lib/
COPY Coacher.WebApi/Coacher.WebApi.csproj Coacher.WebApi/
COPY .config/dotnet-tools.json .config/

RUN dotnet restore Coacher.WebApi/Coacher.WebApi.csproj
RUN dotnet tool restore

COPY . ./
RUN dotnet publish Coacher.WebApi/Coacher.WebApi.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

COPY --from=build /app/publish ./
ENTRYPOINT ["dotnet", "Coacher.WebApi.dll"]
