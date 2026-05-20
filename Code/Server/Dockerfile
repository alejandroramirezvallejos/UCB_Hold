FROM mcr.microsoft.com/dotnet/sdk:8.0 AS compilacion
WORKDIR /src
COPY ["IMT_Reservas.Server.csproj", "./"]
RUN dotnet restore "./IMT_Reservas.Server.csproj"
COPY . .
RUN dotnet publish "IMT_Reservas.Server.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=compilacion /app/publish .
EXPOSE 80
ENTRYPOINT ["dotnet", "IMT_Reservas.Server.dll"]