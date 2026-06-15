FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# copy csproj and restore
COPY ./UserService.csproj ./
RUN dotnet restore ./UserService.csproj

# copy rest and publish
COPY . .
RUN dotnet publish ./UserService.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/publish ./
EXPOSE 80
ENTRYPOINT ["dotnet", "UserService.dll"]

# Notes:
# - This Dockerfile targets .NET 10 as specified in the project file (net10.0).
# - For Render deployment: push this Dockerfile's image and configure the
#   ConnectionStrings__DefaultConnection environment variable in the Render dashboard
#   with your production Postgres connection string. Do NOT store secrets in source.
