FROM mcr.microsoft.com/dotnet/sdk:8.0 AS compilacion
WORKDIR /src
COPY ["IMT_Reservas.Server.csproj", "./"]
RUN dotnet restore "./IMT_Reservas.Server.csproj"
COPY src/ ./src/
COPY Properties/ ./Properties/
COPY appsettings*.json ./
RUN dotnet publish "IMT_Reservas.Server.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
USER app
WORKDIR /app
COPY --from=compilacion /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "IMT_Reservas.Server.dll"]
