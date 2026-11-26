# 1. Bouwfase: met de .NET SDK de API publiceren
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY . .
RUN dotnet publish wpfw_opdract3/wpfw_opdracht3.csproj -c Release -o /out /p:UseAppHost=false

# 2. Eindfase: alleen de ASP.NET Core-runtime + de gepubliceerde API
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /out .
# De API luistert in de container op poort 8080
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "wpfw_opdracht3.dll"]