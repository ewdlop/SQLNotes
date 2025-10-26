# Freudian Psychology Database Schema

## Overview

This database schema demonstrates the Freudian structural model of the psyche (Id, Ego, and Superego) as an educational example of database design with primary and foreign key relationships.

## Freudian Theory Background

Sigmund Freud's structural model divides the psyche into three components:

1. **Id**: The primitive, instinctual part of the mind containing sexual and aggressive drives. It operates on the pleasure principle, seeking immediate gratification.

2. **Superego**: The moral conscience incorporating societal rules and parental values. It strives for perfection and judges our behavior.

3. **Ego**: The realistic part that mediates between the desires of the id and the moral constraints of the superego. It operates on the reality principle, finding realistic ways to satisfy id's desires while considering superego's moral standards.

## Database Structure

### Tables

1. **ID_COMPONENT**
   - Primary Key: `idComponentId`
   - Represents primitive drives and instincts
   - Types: Life Instinct, Death Instinct

2. **SUPEREGO_COMPONENT**
   - Primary Key: `superegoComponentId`
   - Represents moral rules and ideals
   - Types: Conscience, Ego Ideal

3. **EGO_DECISION**
   - Primary Key: `egoDecisionId`
   - Foreign Keys: `idComponentId`, `superegoComponentId`
   - **Demonstrates the key relationship**: The Ego mediates between Id and Superego

4. **PSYCHOLOGICAL_CONFLICT**
   - Primary Key: `conflictId`
   - Foreign Key: `egoDecisionId`
   - Tracks conflicts between psychic components

5. **DEFENSE_MECHANISM_CATALOG**
   - Primary Key: `mechanismId`
   - Catalogs psychological defense mechanisms by maturity level

## Key Concepts Demonstrated

### Primary Keys
Each table has a unique identifier (primary key):
- `idComponentId`, `superegoComponentId`, `egoDecisionId`, etc.

### Foreign Key Relationships
The `EGO_DECISION` table demonstrates complex relationships:
```sql
FOREIGN KEY (idComponentId) REFERENCES ID_COMPONENT(idComponentId)
FOREIGN KEY (superegoComponentId) REFERENCES SUPEREGO_COMPONENT(superegoComponentId)
```

This structure mirrors the psychological theory where the ego must reference and mediate between id and superego components.

## Example Queries

The schema includes various query examples:

1. **Basic Joins**: View ego decisions with their associated id and superego components
2. **Conflict Analysis**: Find and analyze psychological conflicts
3. **Defense Mechanisms**: Analyze usage patterns of different coping mechanisms
4. **CTEs (Common Table Expressions)**: Complex queries for psychodynamic analysis
5. **Window Functions**: Ranking and temporal pattern analysis

## Educational Value

This schema serves as a practical example for learning:
- Database normalization
- Primary and foreign key relationships
- Complex joins across multiple tables
- Common Table Expressions (CTEs)
- Window functions
- CHECK constraints for data validation
- DEFAULT values and computed fields

## Usage

```sql
-- Execute the freudian-psychology.tsql file in SQL Server Management Studio
-- The script will:
-- 1. Create all tables with appropriate constraints
-- 2. Insert sample data
-- 3. Provide example queries to explore the relationships
```

## Real-World Analogy

While this example uses psychology concepts, the same pattern applies to many real-world scenarios:
- **Order Processing**: Orders reference both Products (what) and Customers (who)
- **Project Management**: Tasks reference both Projects (context) and Employees (who does it)
- **Mediation Systems**: Any scenario where one entity mediates between two others

The Id-Ego-Superego model provides an intuitive way to understand how foreign keys can represent complex relationships where one entity bridges or mediates between two others.
