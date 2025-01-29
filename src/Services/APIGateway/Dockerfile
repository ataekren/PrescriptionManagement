FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/Services/APIGateway/APIGateway.csproj", "src/Services/APIGateway/"]
COPY ["src/Shared/SharedKernel/SharedKernel.csproj", "src/Shared/SharedKernel/"]
RUN dotnet restore "src/Services/APIGateway/APIGateway.csproj"
COPY . .
WORKDIR "/src/src/Services/APIGateway"
RUN dotnet build "APIGateway.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "APIGateway.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "APIGateway.dll"]
