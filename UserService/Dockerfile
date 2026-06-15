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
