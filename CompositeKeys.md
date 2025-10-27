# Composite Keys in SQL

## Overview

A **composite key** (also known as a compound key) is a primary key composed of two or more columns that together uniquely identify each row in a table. Unlike a simple primary key which uses a single column, composite keys leverage multiple columns to establish uniqueness.

## When to Use Composite Keys

Composite keys are particularly useful when:

1. **No Single Natural Key Exists**: When no single column can uniquely identify a record
2. **Natural Relationships**: When multiple attributes naturally combine to form a unique identifier
3. **Junction Tables**: In many-to-many relationships where the combination of foreign keys forms the primary key
4. **Historical Data**: When tracking changes over time (e.g., employee + date combination)
5. **Hierarchical Data**: When representing parent-child relationships with multiple levels

## Syntax

### Creating a Table with Composite Key

```sql
CREATE TABLE TableName (
    Column1 DataType NOT NULL,
    Column2 DataType NOT NULL,
    Column3 DataType,
    CONSTRAINT PK_TableName PRIMARY KEY (Column1, Column2)
);
```

### Adding Composite Key to Existing Table

```sql
ALTER TABLE TableName
ADD CONSTRAINT PK_TableName PRIMARY KEY (Column1, Column2);
```

## Id-Ego-Superego Example

The file `T-Sql/id-ego-superego-composite-key.tsql` demonstrates composite keys using Freudian psychology concepts where `id`, `ego`, and `superego` together form a unique identifier for psychological states.

### Why This Example?

This creative example illustrates several key concepts:

1. **Multiple Component Identity**: Just as Freud's psyche model requires all three components (id, ego, superego) to describe a complete psychological state, the composite key requires all three columns to uniquely identify a record.

2. **Natural Relationship**: The strength or level of each component (represented by integer values) combines to create a unique psychological profile.

3. **Referential Integrity**: Child tables (PsychologicalConflict, BehavioralOutcome, PsycheMetrics) reference the parent table using all three components of the composite key.

### Table Structure

```sql
CREATE TABLE PsychologicalState (
    id INT NOT NULL,           -- Primitive desires
    ego INT NOT NULL,          -- Realistic mediator
    superego INT NOT NULL,     -- Moral standards
    stateName VARCHAR(100),
    description VARCHAR(500),
    conflictLevel DECIMAL(3,2),
    resolutionStrategy VARCHAR(200),
    CONSTRAINT PK_PsychologicalState PRIMARY KEY (id, ego, superego)
);
```

### Foreign Key References

Child tables must reference all components:

```sql
CREATE TABLE PsychologicalConflict (
    conflictId INT PRIMARY KEY IDENTITY(1,1),
    id INT NOT NULL,
    ego INT NOT NULL,
    superego INT NOT NULL,
    conflictType VARCHAR(50),
    -- Foreign key must include all composite key columns
    CONSTRAINT FK_Conflict_State FOREIGN KEY (id, ego, superego)
        REFERENCES PsychologicalState(id, ego, superego)
);
```

## Advantages of Composite Keys

### 1. Natural Representation
- Reflects real-world relationships more accurately
- No need for artificial surrogate keys
- Self-documenting (meaningful column names)

### 2. Data Integrity
- Enforces uniqueness across multiple dimensions
- Prevents duplicate combinations
- Maintains referential integrity across related tables

### 3. Query Optimization
- Can improve query performance when filtering on key columns
- Supports efficient joins on multiple columns

### 4. Semantic Clarity
- Makes the unique identifier meaningful
- Easier to understand data relationships

## Disadvantages and Considerations

### 1. Complexity
- More complex to write JOIN statements
- Foreign key definitions are longer
- More columns to manage in relationships

### 2. Performance
- Larger index size (multiple columns)
- Potentially slower than single-column keys
- More data to transfer in joins

### 3. Updates
- Updating any part of the key affects all referencing tables
- Requires cascading updates if key values change
- More difficult to maintain if business rules change

### 4. Application Code
- More complex in ORM frameworks
- Requires multiple parameters for lookups
- Can complicate API design

## Best Practices

### 1. Choose Immutable Columns
```sql
-- Good: Status codes and dates rarely change
CREATE TABLE OrderStatus (
    orderId INT NOT NULL,
    statusDate DATE NOT NULL,
    status VARCHAR(20),
    PRIMARY KEY (orderId, statusDate)
);

-- Avoid: Email addresses can change
CREATE TABLE UserProfile (
    email VARCHAR(100) NOT NULL,
    profileDate DATE NOT NULL,
    -- Not ideal as composite key
    PRIMARY KEY (email, profileDate)
);
```

### 2. Keep Keys Reasonably Small
```sql
-- Good: 2-3 columns
PRIMARY KEY (customerId, orderDate)

-- Potentially problematic: Too many columns
PRIMARY KEY (region, country, state, city, zipCode)
-- Consider a surrogate key instead
```

### 3. Consider Adding a Surrogate Key
```sql
-- Option: Composite key + surrogate key for easier referencing
CREATE TABLE OrderItem (
    orderItemId INT IDENTITY(1,1) PRIMARY KEY,  -- Surrogate
    orderId INT NOT NULL,
    productId INT NOT NULL,
    quantity INT,
    CONSTRAINT UK_OrderItem UNIQUE (orderId, productId)  -- Natural key
);
```

### 4. Document the Business Logic
Always document why the specific columns form a composite key:

```sql
-- The combination of studentId and courseId uniquely identifies
-- a student's enrollment in a specific course
CREATE TABLE Enrollment (
    studentId INT NOT NULL,
    courseId INT NOT NULL,
    enrollmentDate DATE,
    grade VARCHAR(2),
    PRIMARY KEY (studentId, courseId)
);
```

## Common Use Cases

### 1. Many-to-Many Relationships

```sql
-- Student-Course enrollment
CREATE TABLE StudentCourse (
    studentId INT NOT NULL,
    courseId INT NOT NULL,
    semester VARCHAR(20),
    grade DECIMAL(3,2),
    PRIMARY KEY (studentId, courseId),
    FOREIGN KEY (studentId) REFERENCES Students(studentId),
    FOREIGN KEY (courseId) REFERENCES Courses(courseId)
);
```

### 2. Time-Series Data

```sql
-- Stock prices over time
CREATE TABLE StockPrice (
    symbol VARCHAR(10) NOT NULL,
    priceDate DATETIME NOT NULL,
    openPrice DECIMAL(10,2),
    closePrice DECIMAL(10,2),
    volume BIGINT,
    PRIMARY KEY (symbol, priceDate)
);
```

### 3. Hierarchical Structures

```sql
-- Organization hierarchy
CREATE TABLE OrganizationUnit (
    companyId INT NOT NULL,
    departmentId INT NOT NULL,
    unitId INT NOT NULL,
    unitName VARCHAR(100),
    PRIMARY KEY (companyId, departmentId, unitId)
);
```

### 4. Versioned Records

```sql
-- Product versions
CREATE TABLE ProductVersion (
    productId INT NOT NULL,
    versionNumber INT NOT NULL,
    releaseDate DATE,
    features VARCHAR(1000),
    PRIMARY KEY (productId, versionNumber)
);
```

## Querying with Composite Keys

### Exact Match Query
```sql
SELECT *
FROM PsychologicalState
WHERE id = 1 AND ego = 2 AND superego = 1;
```

### Join with Composite Keys
```sql
SELECT ps.stateName, pc.conflictType
FROM PsychologicalState ps
JOIN PsychologicalConflict pc 
    ON ps.id = pc.id 
    AND ps.ego = pc.ego 
    AND ps.superego = pc.superego;
```

### Partial Key Search
```sql
-- Find all states where id = 2 (partial key)
SELECT *
FROM PsychologicalState
WHERE id = 2;
```

## Composite Keys vs. Surrogate Keys

| Aspect | Composite Key | Surrogate Key |
|--------|--------------|---------------|
| **Meaning** | Business-meaningful columns | System-generated, no business meaning |
| **Stability** | Can change if business rules change | Stable, never changes |
| **Complexity** | More complex joins and foreign keys | Simpler references |
| **Performance** | Larger indexes, more join overhead | Smaller indexes, faster joins |
| **Clarity** | Self-documenting | Requires additional context |
| **Best For** | Natural relationships, junction tables | Arbitrary records, frequently updated keys |

## Conclusion

Composite keys are a powerful database design tool when used appropriately. They provide natural representation of complex relationships and enforce data integrity across multiple dimensions. However, they come with added complexity and should be chosen carefully based on:

- Business requirements
- Data stability
- Query patterns
- Performance needs
- Maintenance considerations

The id-ego-superego example in this repository demonstrates how composite keys can elegantly model complex, multi-dimensional concepts while maintaining referential integrity throughout the database schema.

## References

- [Normalization Forms](Normalization%20Forms.md)
- [Entity Relational Mapping](ERM.md)
- [T-SQL Example: id-ego-superego-composite-key.tsql](T-Sql/id-ego-superego-composite-key.tsql)

## Related Concepts

- **Primary Keys**: Unique identifiers for table rows
- **Foreign Keys**: References to primary keys in other tables
- **Candidate Keys**: Potential primary keys
- **Natural Keys**: Keys derived from real-world attributes
- **Surrogate Keys**: Artificial keys with no business meaning
- **Unique Constraints**: Ensure uniqueness without being primary keys
