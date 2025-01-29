FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["src/Services/PrescriptionService/PrescriptionService.csproj", "src/Services/PrescriptionService/"]
COPY ["src/Shared/SharedKernel/SharedKernel.csproj", "src/Shared/SharedKernel/"]
RUN dotnet restore "src/Services/PrescriptionService/PrescriptionService.csproj"
COPY . .
WORKDIR "/src/src/Services/PrescriptionService"
RUN dotnet build "PrescriptionService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PrescriptionService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PrescriptionService.dll"]
