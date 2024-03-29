FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["MyAdhan/MyAdhan.Scheduler.csproj", "MyAdhan/"]
RUN dotnet restore "MyAdhan/MyAdhan.Scheduler.csproj"
COPY . .
WORKDIR "/src/MyAdhan"
RUN dotnet build "MyAdhan.Scheduler.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "MyAdhan.Scheduler.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT [ "dotnet", "MyAdhan.Scheduler.dll" ]