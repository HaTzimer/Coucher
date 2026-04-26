# syntax=docker/dockerfile:1.4

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

COPY --from=infra . /Infra.Core

COPY Coucher.sln ./
COPY global.json ./
COPY Coucher.Shared/Coucher.Shared.csproj Coucher.Shared/
COPY Coucher.Lib/Coucher.Lib.csproj Coucher.Lib/
COPY Coucher.WebApi/Coucher.WebApi.csproj Coucher.WebApi/
COPY .config/dotnet-tools.json .config/

RUN dotnet restore Coucher.sln
RUN dotnet tool restore

COPY . ./
RUN dotnet publish Coucher.WebApi/Coucher.WebApi.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

COPY --from=build /app/publish ./
ENTRYPOINT ["dotnet", "Coucher.WebApi.dll"]
