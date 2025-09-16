# create sln file and .net projects
> dotnet new sln

> dotnet new webapi -o API -controllers
> dotnet new classlib -o Core
> dotnet new classlib -o Infrastructure

# add projects to solution
> dotnet sln add API/
> dotnet sln add Core/
> dotnet sln add Infrastructure/

- add project references
API > dotnet add reference ../Infrastructure

> dotnet restore

> dotnet build

> docker compose up -d
> docker compose down

 install dotnet-ef tool

> dotnet tool install --global dotnet-ef

# initiate db migration
(-s= startup project, -p= dbcontext project)
> dotnet ef migrations add InitialCreate -s API -p Infrastructure

remove migration
> dotnet ef migrations remove -s API -p Infrastructure

create db
> dotnet ef database update -s API -p Infrastructure

create .net gitignore file

> dotnet new gitignore