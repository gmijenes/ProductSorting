# Usar la imagen oficial de .NET 6 SDK como la base
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /app

# Copiar el archivo .csproj y restaurar las dependencias
COPY *.csproj ./
RUN dotnet restore

# Copiar todo el resto del código y construir la aplicación
COPY . ./
RUN dotnet publish -c Release -o out

# Usar la imagen oficial de .NET 6 Runtime como base para ejecutar la app
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build-env /app/out .

# Exponer el puerto que usará la aplicación (por defecto 80)
EXPOSE 80

# Ejecutar la aplicación
ENTRYPOINT ["dotnet", "ProductSorting.dll"]