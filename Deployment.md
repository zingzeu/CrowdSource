## Deployment Procedures

1. Set Environment Variable `ASPNETCORE_ENVIRONMENT` to `Production`
2. Set Environment Variable `ConnectionSettings:DefaultConnection` to PostgreSQL connection string, such as:
```
Server=127.0.0.1;Port=5432;Database=crowdsource;User Id=<user>;Password=<password>;
```
3. Add Administrator Role to one user
```
dotnet run CrowdSource.dll SetUserRole <email> Administrator
```
4. SeedDb or import SQL
5. (Preferably) Set global options


## Backup

Run within the Postgres container:

```
pg_dump --dbname=crowdsource --username=cs --password -f <file location>
```

## Restore backup

Run within the Postgres container:

```
psql --username=cs crowdsource < backup.sql
```