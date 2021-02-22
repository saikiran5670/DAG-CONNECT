#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["portalserver/net.atos.daf.ct2.portalservicepoc/net.atos.daf.ct2.portalservicepoc.csproj", "portalserver/net.atos.daf.ct2.portalservicepoc/"]
RUN dotnet restore "portalserver/net.atos.daf.ct2.portalservicepoc/net.atos.daf.ct2.portalservicepoc.csproj"
COPY . .
WORKDIR "/src/portalserver/net.atos.daf.ct2.portalservicepoc"
RUN dotnet build "net.atos.daf.ct2.portalservicepoc.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "net.atos.daf.ct2.portalservicepoc.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "net.atos.daf.ct2.portalservicepoc.dll"]