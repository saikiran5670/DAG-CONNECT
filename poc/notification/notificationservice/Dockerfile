#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
COPY ["notificationservice/notificationservice.csproj", "notificationservice/"]
RUN dotnet restore "notificationservice/notificationservice.csproj"
COPY . .
WORKDIR "/src/notificationservice"
RUN dotnet build "notificationservice.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "notificationservice.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "notificationservice.dll"]