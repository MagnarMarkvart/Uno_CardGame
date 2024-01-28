# UnoGame

## EF

~~~bash
dotnet tool update --global dotnet-ef

dotnet ef migrations add --project DAL --startup-project ConsoleApp InitialCreate
dotnet ef migrations add --project DAL --startup-project WebApp InitialCreate
dotnet ef database update --project DAL --startup-project WebApp 
~~~

## Live reload server

~~~bash
dotnet tool install -g LiveReloadServer
~~~

## WebPage

***NB! Cd to web app directory for pages scaffolding***

~~~bash
dotnet tool update --global dotnet-aspnet-codegenerator
cd WebApp
dotnet aspnet-codegenerator razorpage \
    -m Domain.Database.Game \
    -dc AppDbContext \
    -udl \
    -outDir Pages/Games \
    --referenceScriptLibraries
    
dotnet aspnet-codegenerator razorpage \
    -m Domain.Database.Player \
    -dc AppDbContext \
    -udl \
    -outDir Pages/Players \
    --referenceScriptLibraries
    
dotnet aspnet-codegenerator razorpage \
    -m Domain.Database.History \
    -dc AppDbContext \
    -udl \
    -outDir Pages/Histories \
    --referenceScriptLibraries    
~~~