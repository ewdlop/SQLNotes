# SQLNotes

# ORM

https://www.learndapper.com/

https://learn.microsoft.com/en-us/ef/core/

# SDK

https://learn.microsoft.com/en-us/javascript/api/overview/azure/sql?view=azure-node-latest

# Javascirpt ORM

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


# SqlLinter/Parser

https://github.com/sqlfluff/sqlfluff
