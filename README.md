# SQLNotes

## K-anonymity

> *True privacy in K-anonymity is not just about hiding in a crowd; it’s about ensuring no individual can be singled out, even when the data is combined, probed, or deconstructed.*

[K-anonymity](https://en.wikipedia.org/wiki/K-anonymity)

## Overview

This repository contains various SQL scripts, T-SQL scripts, and documentation files. Below is a summary of each file and its purpose:

- `.Net/quick_setup.tsql`: SQL scripts for creating and managing a database named `MySchool`.
- `ADONET/SqlInjectionPrevention.cs`: C# class for preventing SQL injection using parameterized queries.
- `README.md`: Provides links to various resources related to SQL, ORM, and SQL linters/parsers.
- `T-Sql/gg.tsql`: Script for dropping all user databases in SQL Server.
- `T-Sql/gg2.tsql`: Script for dropping all user databases in SQL Server.
- `T-Sql/OPENROWSET_example.tsql`: Examples of using the `OPENROWSET` function in SQL Server.

## Setting Up and Using SQL Scripts

### .Net/quick_setup.tsql

This file includes SQL scripts for creating and managing a database named `MySchool`. To set up the database, follow these steps:

1. Open SQL Server Management Studio (SSMS).
2. Connect to your SQL Server instance.
3. Open the `quick_setup.tsql` file.
4. Execute the script to create the `MySchool` database and its associated objects.

### T-Sql/gg.tsql and T-Sql/gg2.tsql

These files contain scripts for dropping all user databases in SQL Server. To use these scripts:

1. Open SQL Server Management Studio (SSMS).
2. Connect to your SQL Server instance.
3. Open the `gg.tsql` or `gg2.tsql` file.
4. Execute the script to drop all user databases.

### T-Sql/OPENROWSET_example.tsql

This file provides examples of using the `OPENROWSET` function in SQL Server. To use these examples:

1. Open SQL Server Management Studio (SSMS).
2. Connect to your SQL Server instance.
3. Open the `OPENROWSET_example.tsql` file.
4. Execute the script to see examples of using the `OPENROWSET` function.

## SQL Injection Prevention

The `ADONET/SqlInjectionPrevention.cs` file contains a C# class for preventing SQL injection using parameterized queries. It demonstrates how to use parameterized queries to avoid SQL injection vulnerabilities. Here is an example of how to use the `SqlInjectionPrevention` class:

```csharp
using System;
using System.Data.SqlClient;

string connectionString = "your-database-connection-string";
SqlInjectionPrevention prevention = new SqlInjectionPrevention(connectionString);
prevention.GetUserDetails("testuser");

public class SqlInjectionPrevention
{
    private string connectionString;

    public SqlInjectionPrevention(string connectionString)
    {
        this.connectionString = connectionString;
    }

    public void GetUserDetails(string username)
    {
        string query = "SELECT * FROM Users WHERE Username = @Username";

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            SqlCommand command = new SqlCommand(query, connection);
            // Use parameterized query to prevent SQL injection
            command.Parameters.AddWithValue("@Username", username);

            connection.Open();
            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                Console.WriteLine($"Username: {reader["Username"]}, Email: {reader["Email"]}");
            }
        }
    }
}
```

## Using TSqlParser for Parsing and Analyzing SQL Scripts

The `T-Sql/TSqlParser.md` file provides a detailed explanation of the `TSqlParser` class and its usage. The `TSqlParser` class from the Microsoft.Data.Tools.Schema.Sql library is used to parse T-SQL scripts. This can be useful for analyzing SQL scripts, including detecting potential SQL injection vulnerabilities by examining the structure and content of the queries.

For more details and examples, refer to the `T-Sql/TSqlParser.md` file.

## ORM

https://www.learndapper.com/

https://learn.microsoft.com/en-us/ef/core/

## SDK

https://learn.microsoft.com/en-us/javascript/api/overview/azure/sql?view=azure-node-latest

## JavaScript ORM

https://medium.com/@dikibhuyan/how-to-make-your-own-oracle-orm-in-javascript-node-42f97751b10

JavaScript Object-Relational Mapping (ORM) is a technique that allows developers to interact with databases using JavaScript objects rather than raw SQL queries. ORMs simplify database operations, enhance productivity, and improve code readability.

Here are some popular JavaScript ORMs:

### 1. **Sequelize**
   - **Database Support**: MySQL, PostgreSQL, SQLite, MariaDB, Microsoft SQL Server
   - **Features**:
     - Schema migrations
     - Model validation
     - Query chaining
     - Associations (one-to-one, one-to-many, many-to-many)
   - **Example**:
     ```javascript
     const { Sequelize, DataTypes } = require('sequelize');
     const sequelize = new Sequelize('database', 'username', 'password', {
       host: 'localhost',
       dialect: 'mysql',
     });

     const User = sequelize.define('User', {
       username: DataTypes.STRING,
       birthday: DataTypes.DATE,
     });

     sequelize.sync().then(() => {
       return User.create({
         username: 'JohnDoe',
         birthday: new Date(1990, 1, 1),
       });
     });
     ```

---

### 2. **TypeORM**
   - **Database Support**: MySQL, PostgreSQL, SQLite, MariaDB, Microsoft SQL Server, Oracle, CockroachDB
   - **Features**:
     - Fully typed models and queries
     - Active Record and Data Mapper patterns
     - Built-in CLI for database migrations
   - **Example**:
     ```javascript
     import { Entity, PrimaryGeneratedColumn, Column, createConnection } from "typeorm";

     @Entity()
     class User {
       @PrimaryGeneratedColumn()
       id: number;

       @Column()
       name: string;

       @Column()
       age: number;
     }

     createConnection({
       type: "sqlite",
       database: "test.db",
       entities: [User],
       synchronize: true,
     }).then(async (connection) => {
       const user = new User();
       user.name = "John";
       user.age = 30;
       await connection.manager.save(user);
     });
     ```

---

### 3. **Prisma**
   - **Database Support**: PostgreSQL, MySQL, SQLite, MongoDB, Microsoft SQL Server
   - **Features**:
     - Declarative data modeling
     - Auto-generated queries
     - Works well with modern frameworks like Next.js
   - **Example**:
     ```javascript
     const { PrismaClient } = require('@prisma/client');
     const prisma = new PrismaClient();

     async function main() {
       const user = await prisma.user.create({
         data: {
           name: 'John Doe',
           email: 'john.doe@example.com',
         },
       });
       console.log(user);
     }

     main().catch((e) => {
       throw e;
     }).finally(async () => {
       await prisma.$disconnect();
     });
     ```

---

### 4. **Objection.js**
   - **Database Support**: Any SQL-based database supported by [Knex.js](http://knexjs.org/)
   - **Features**:
     - Schema migrations via Knex.js
     - Complex query building
     - Support for GraphQL and JSON APIs
   - **Example**:
     ```javascript
     const { Model } = require('objection');
     const Knex = require('knex');

     const knex = Knex({
       client: 'sqlite3',
       useNullAsDefault: true,
       connection: {
         filename: './mydb.sqlite',
       },
     });

     Model.knex(knex);

     class User extends Model {
       static get tableName() {
         return 'users';
       }
     }

     async function main() {
       await knex.schema.createTable('users', (table) => {
         table.increments('id').primary();
         table.string('name');
       });

       await User.query().insert({ name: 'John Doe' });
       const users = await User.query();
       console.log(users);
     }

     main();
     ```

---

### 5. **Waterline**
   - **Database Support**: MySQL, MongoDB, PostgreSQL, Redis, and others
   - **Features**:
     - Part of the Sails.js framework
     - Designed for use with multiple database systems
   - **Example**:
     ```javascript
     const Waterline = require('waterline');
     const orm = new Waterline();

     const User = Waterline.Collection.extend({
       identity: 'user',
       datastore: 'default',
       primaryKey: 'id',
       attributes: {
         id: { type: 'number', autoMigrations: { autoIncrement: true } },
         name: { type: 'string' },
       },
     });

     orm.registerModel(User);

     orm.initialize({
       adapters: { default: require('sails-disk') },
       datastores: { default: { adapter: 'default' } },
     }, (err, models) => {
       if (err) throw err;

       models.collections.user.create({ name: 'John Doe' }).exec(console.log);
     });
     ```

---

### Choosing the Right ORM
- **For complex and scalable projects**: Use **Prisma** or **TypeORM**.
- **For simplicity and quick setup**: Use **Sequelize** or **Objection.js**.
- **For Sails.js integration**: Use **Waterline**.

Each ORM has its strengths and weaknesses. Your choice depends on factors like database type, project size, and development style.

# SQL/Sql/Sequl Linter/Parser

https://github.com/microsoft/DacFx

https://github.com/sqlfluff/sqlfluff

https://www.sqlparser.com/snowflake-sql-parser.php
