FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY Poc.MediumWorker/Poc.MediumWorker.csproj Poc.MediumWorker/
RUN dotnet restore Poc.MediumWorker/Poc.MediumWorker.csproj

COPY . .
RUN dotnet publish Poc.MediumWorker/Poc.MediumWorker.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 5000 5001
ENTRYPOINT ["dotnet", "Poc.MediumWorker.dll"]