# Stage 1: Build — compile the source code
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY PartnerTransactionAPI.csproj .
RUN dotnet restore
COPY . .
RUN dotnet publish -c Release -o /app/publish

# Stage 2: Final — lightweight runtime only image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
RUN mkdir -p /app/logs
COPY --from=build /app/publish .
EXPOSE 5000
ENV ASPNETCORE_URLS=http://+:5000
ENTRYPOINT ["dotnet", "PartnerTransactionAPI.dll"]