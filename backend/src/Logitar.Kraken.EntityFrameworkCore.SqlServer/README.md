# Logitar.Kraken.EntityFrameworkCore.SqlServer

Provides an implementation of a relational event store to be used with Kraken application management tool suite, Entity Framework Core and Microsoft SQL Server.

## Migrations

This project is setup to use migrations. All the commands below must be executed in the solution directory.

### Create a migration

To create a new migration, execute the following command. Do not forget to provide a migration name!

```sh
dotnet ef migrations add <YOUR_MIGRATION_NAME> --context KrakenContext --project src/Logitar.Kraken.EntityFrameworkCore.SqlServer --startup-project src/Logitar.Kraken
```

### Remove a migration

To remove the latest unapplied migration, execute the following command.

```sh
dotnet ef migrations remove --context KrakenContext --project src/Logitar.Kraken.EntityFrameworkCore.SqlServer --startup-project src/Logitar.Kraken
```

### Generate a script

To generate a script, execute the following command. Do not forget to provide a source migration name!

```sh
dotnet ef migrations script <SOURCE_MIGRATION> --context KrakenContext --project src/Logitar.Kraken.EntityFrameworkCore.SqlServer --startup-project src/Logitar.Kraken
```
