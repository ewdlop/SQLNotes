# Id-Ego-Superego Composite Key Example

## Quick Reference

This is a practical demonstration of **composite keys** in SQL using Freudian psychology concepts.

## Tables Overview

```
┌─────────────────────────────────────────────────────────────┐
│           PsychologicalState (Parent Table)                 │
│  PRIMARY KEY: (id, ego, superego) - COMPOSITE KEY           │
├─────────────────────────────────────────────────────────────┤
│  id (INT)              - Primitive desires level            │
│  ego (INT)             - Rational mediator level            │
│  superego (INT)        - Moral standards level              │
│  stateName (VARCHAR)   - Name of the state                  │
│  description (VARCHAR) - Description                        │
│  conflictLevel (DEC)   - How much internal conflict (0-5)   │
│  resolutionStrategy    - Strategy to resolve conflicts      │
└─────────────────────────────────────────────────────────────┘
                          │
                          │ Referenced by (Foreign Keys)
                          │
        ┌─────────────────┼─────────────────┬───────────────────┐
        │                 │                 │                   │
        ▼                 ▼                 ▼                   ▼
┌──────────────┐  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐
│Psychological │  │ Behavioral   │  │   Psyche     │  │              │
│  Conflict    │  │   Outcome    │  │   Metrics    │  │    ...       │
├──────────────┤  ├──────────────┤  ├──────────────┤  ├──────────────┤
│ FK: (id, ego,│  │ FK: (id, ego,│  │ FK: (id, ego,│  │              │
│   superego)  │  │   superego)  │  │   superego)  │  │              │
│              │  │              │  │              │  │              │
│ Tracks       │  │ Records      │  │ Measures     │  │              │
│ conflicts    │  │ behaviors    │  │ strengths    │  │              │
└──────────────┘  └──────────────┘  └──────────────┘  └──────────────┘
```

## Example Data

### Sample Psychological State
```
id=2, ego=2, superego=2
├─ State Name: "High-Intensity Conflict"
├─ Conflict Level: 4.50
└─ Description: All components strong, creating intense struggle
```

### What Makes This a Composite Key?

Each component alone is **NOT unique**:
- id=1 appears in multiple rows
- ego=2 appears in multiple rows  
- superego=1 appears in multiple rows

But the **combination** (id, ego, superego) is **UNIQUE**:
- (1, 1, 1) ✓ Unique
- (1, 2, 1) ✓ Unique
- (2, 1, 1) ✓ Unique
- (1, 1, 2) ✓ Unique

## Key SQL Operations

### Creating the Table
```sql
CREATE TABLE PsychologicalState (
    id INT NOT NULL,
    ego INT NOT NULL,
    superego INT NOT NULL,
    stateName VARCHAR(100),
    -- Composite primary key
    CONSTRAINT PK_PsychologicalState PRIMARY KEY (id, ego, superego)
);
```

### Inserting Data
```sql
INSERT INTO PsychologicalState (id, ego, superego, stateName, conflictLevel)
VALUES (1, 1, 1, 'Balanced Harmony', 1.00);
```

### Querying with Composite Key
```sql
-- Must specify all key columns for exact match
SELECT * 
FROM PsychologicalState
WHERE id = 1 AND ego = 1 AND superego = 1;
```

### Joining with Composite Key
```sql
-- All columns must be matched in JOIN
SELECT ps.stateName, pc.conflictType
FROM PsychologicalState ps
JOIN PsychologicalConflict pc 
    ON ps.id = pc.id 
    AND ps.ego = pc.ego 
    AND ps.superego = pc.superego;
```

### Foreign Key Reference
```sql
CREATE TABLE PsychologicalConflict (
    conflictId INT PRIMARY KEY IDENTITY(1,1),
    id INT NOT NULL,
    ego INT NOT NULL,
    superego INT NOT NULL,
    -- Foreign key must reference ALL composite key columns
    CONSTRAINT FK_Conflict_State 
        FOREIGN KEY (id, ego, superego)
        REFERENCES PsychologicalState(id, ego, superego)
);
```

## Why This Example?

### 1. Natural Multi-Dimensional Model
Just as Freud's model needs **all three** components to describe personality:
- **Id** alone = incomplete (just desires)
- **Ego** alone = incomplete (just logic)
- **Superego** alone = incomplete (just morals)
- **All three together** = complete psychological state ✓

### 2. Real-World Complexity
Different combinations create different states:
- (1,1,1) = Balanced
- (2,1,1) = Id-dominant (impulsive)
- (1,1,2) = Superego-dominant (overly moral)
- (3,2,1) = Extreme desires (critical)

### 3. Demonstrates Composite Key Benefits
- ✓ Enforces uniqueness across multiple dimensions
- ✓ Maintains referential integrity
- ✓ Self-documenting (meaningful column names)
- ✓ Natural relationship representation

## Common Queries

### Find High-Conflict States
```sql
SELECT id, ego, superego, stateName, conflictLevel
FROM PsychologicalState
WHERE conflictLevel >= 4.0
ORDER BY conflictLevel DESC;
```

### Analyze Component Dominance
```sql
SELECT 
    CASE 
        WHEN id > ego AND id > superego THEN 'Id-Dominant'
        WHEN ego > id AND ego > superego THEN 'Ego-Dominant'
        WHEN superego > id AND superego > ego THEN 'Superego-Dominant'
        ELSE 'Balanced'
    END as DominantComponent,
    COUNT(*) as StateCount,
    AVG(conflictLevel) as AvgConflict
FROM PsychologicalState
GROUP BY 
    CASE 
        WHEN id > ego AND id > superego THEN 'Id-Dominant'
        WHEN ego > id AND ego > superego THEN 'Ego-Dominant'
        WHEN superego > id AND superego > ego THEN 'Superego-Dominant'
        ELSE 'Balanced'
    END;
```

### Unresolved Conflicts
```sql
SELECT ps.stateName, pc.conflictType, pc.severity
FROM PsychologicalState ps
JOIN PsychologicalConflict pc 
    ON ps.id = pc.id 
    AND ps.ego = pc.ego 
    AND ps.superego = pc.superego
WHERE pc.resolvedDate IS NULL
ORDER BY ps.conflictLevel DESC;
```

## Files

- **[T-Sql/id-ego-superego-composite-key.tsql](T-Sql/id-ego-superego-composite-key.tsql)** - Full SQL implementation
- **[CompositeKeys.md](CompositeKeys.md)** - Comprehensive documentation on composite keys
- **[README.md](README.md)** - Main repository documentation

## Learning Points

1. **Composite keys** use multiple columns to ensure uniqueness
2. **Foreign keys** must reference ALL columns in a composite key
3. **JOINs** require matching ALL key columns
4. **Queries** can filter on individual columns or the full combination
5. **Business logic** should determine when composite keys are appropriate

## Next Steps

1. Execute the SQL script: `T-Sql/id-ego-superego-composite-key.tsql`
2. Read the documentation: `CompositeKeys.md`
3. Experiment with queries and modifications
4. Consider when composite keys fit your database design needs

---

*For complete details and advanced examples, see the full SQL script and documentation.*
