# -------- BUILD STAGE --------
    FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
    WORKDIR /src
    
    COPY *.csproj ./
    RUN dotnet restore
    
    COPY . .
    RUN dotnet publish -c Release -o /app/publish
    
    # -------- RUNTIME STAGE --------
    FROM mcr.microsoft.com/dotnet/aspnet:8.0
    WORKDIR /app
    
    COPY --from=build /app/publish .
    
    EXPOSE 8080
    
    # Force HTTP & Development environment
    ENV ASPNETCORE_URLS=http://+:8080
    ENV DOTNET_RUNNING_IN_CONTAINER=true
    ENV ASPNETCORE_ENVIRONMENT=Development
    
    ENTRYPOINT ["dotnet", "TodoApp.API.dll"]