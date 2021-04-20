#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["Services/Properties/Properties.csproj", "Services/Properties/"]
COPY ["Libraries/Utilities/Utilities.csproj", "Libraries/Utilities/"]
COPY ["Libraries/Models/Models.csproj", "Libraries/Models/"]
RUN dotnet restore "Services/Properties/Properties.csproj"
COPY . .
WORKDIR "/src/Services/Properties"
RUN dotnet build "Properties.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Properties.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Properties.dll"]