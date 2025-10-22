# Create Backend .NET Project

### create sln file and .net projects
> dotnet new sln
> dotnet new webapi -o API -controllers
> dotnet new classlib -o Core
> dotnet new classlib -o Infrastructure

### add projects to solution
> dotnet sln add API/
> dotnet sln add Core/
> dotnet sln add Infrastructure/

### add project references
API > dotnet add reference ../Infrastructure

> dotnet restore
> dotnet build

> docker compose up -d
> docker compose down

 install dotnet-ef tool

> dotnet tool install --global dotnet-ef

### initiate db migration
`(-s= startup project, -p= dbcontext project)`
> dotnet ef migrations add InitialCreate -s API -p Infrastructure

remove migration
> dotnet ef migrations remove -s API -p Infrastructure

create db
> dotnet ef database update -s API -p Infrastructure

create .net gitignore file

> dotnet new gitignore

delete database
> dotnet ef database drop -p Infrastructure -s API



# -----------------  client app - angular  -----------------  

> ng new client

> ng serve -o


###  install mkcert 
Steps to install mkcert without admin rights

1. Go to the official mkcert releases page:   👉 https://github.com/FiloSottile/mkcert/releases
2. Download the Windows binary: File name will look like: mkcert-vX.X.X-windows-amd64.exe
3. Rename it to `mkcert.exe` for convenience.
4. Put it somewhere in your user space, e.g.: ` C:\Users\<your-username>\mkcert\`
5. Add that folder to your PATH (user scope):
6. Press `Win + R → type`
    > rundll32 sysdm.cpl,EditEnvironmentVariables. 
    `Under User variables → select Path → Edit → New → add:`
7. open cmd/terminal in same directory and run command
    > `.\mkcert.exe -install`
8. after install , run command to general localhost certificate
    > `mkcert localhost`

9. put the certficiates as desired in app

install angular material
> ng add @angular/material

install tailwind css
> npm install tailwindcss @tailwindcss/postcss

-Create componnet

- ng g c layout/header --dry-run `(--dry-run = tells what will be created)`
> ng g c layout/header --skip-tests

* vs code settings > compact folders (uncheck)
* vs code settings > brackets auto close tag

> ng g s core/services/shop --skip-tests

install nano id
> npm install nanoid

create environemtn config files
> ng g environments


--------------add identity ------
update the dbcotext

add migration
> dotnet ef migrations add IdentityAdded -s API -p Infrastructure

add migration for Address entity
> dotnet ef migrations add AddressAdded -s API -p Infrastructure