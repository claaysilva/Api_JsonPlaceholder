FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["JsonPlaceholderApi.csproj", "./"]
RUN dotnet restore "./JsonPlaceholderApi.csproj"
COPY . .
WORKDIR "/src"
RUN dotnet publish "JsonPlaceholderApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "JsonPlaceholderApi.dll"]
