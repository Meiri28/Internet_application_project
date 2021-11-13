For Mac/Linux users -

Doc -
https://docs.microsoft.com/en-us/sql/linux/quickstart-install-connect-docker?view=sql-server-ver15&pivots=cs1-bash
https://medium.com/geekculture/docker-express-running-a-local-sql-server-on-your-m1-mac-8bbc22c49dc9


let's download docker file -

```sudo docker pull mcr.microsoft.com/mssql/server:2019-latest```

then run docker on local machine -

```sudo docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=<YourStrong@Passw0rd>" \
   -p 1433:1433 --name sql1 -h sql1 \
   -d mcr.microsoft.com/mssql/server:2019-latest```


but if you have M1 CPU arm64 arc you need to run diffrent docker env -
```
docker run -e "ACCEPT_EULA=1" -e "MSSQL_SA_PASSWORD=MyPass@word" -e "MSSQL_PID=Developer" -e "MSSQL_USER=SA" -p 1433:1433 -d --name=sql mcr.microsoft.com/azure-sql-edge
```

local info -
ip - 127.0.0.1
port - 1433
user - sa
pass - MyPass@word

after creating docker container run - 
```CREATE DATABASE DB_website```


After this we need to install 

.net core CLI  - https://docs.microsoft.com/en-us/ef/core/managing-schemas/migrations/?tabs=dotnet-core-cli

Create migration - dotnet ef migrations add InitialCreate
Create schema - dotnet ef database update
Revert migration - dotnet ef migrations remove



git ignore -

need to add folder for example -
Recycle/obj/

then run - 
git rm -r --cached Recycle/obj




icons from -> https://icomoon.io/app/#/select

currency -> currency freaks API
Maps -> Google Maps API



