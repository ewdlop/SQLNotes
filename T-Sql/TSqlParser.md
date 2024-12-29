The `TSqlParser` class from the Microsoft.Data.Tools.Schema.Sql library is used to parse T-SQL scripts. This can be useful for analyzing SQL scripts, including detecting potential SQL injection vulnerabilities by examining the structure and content of the queries.

### Example: Using `TSqlParser` to Parse SQL Scripts

Here is an example of how to use the `TSqlParser` class in a .NET application to parse a SQL script:

1. **Install the required package**: Ensure you have the `Microsoft.SqlServer.TransactSql.ScriptDom` package installed. You can install it via NuGet Package Manager:

    ```shell
    Install-Package Microsoft.SqlServer.TransactSql.ScriptDom
    ```

2. **Example Code**: Below is a C# example that demonstrates how to parse a T-SQL script using the `TSqlParser` class:

    ```csharp
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Microsoft.SqlServer.TransactSql.ScriptDom;

    public class SqlParserExample
    {
        public static void Main(string[] args)
        {
            // Example T-SQL script to be parsed
            string sqlScript = "SELECT * FROM Users WHERE Username = 'admin';";

            // Parse the SQL script
            ParseSqlScript(sqlScript);
        }

        private static void ParseSqlScript(string sqlScript)
        {
            // Create a TSql parser
            TSqlParser parser = new TSql150Parser(true);

            // Parse the script
            IList<ParseError> errors;
            TSqlFragment fragment;
            using (TextReader reader = new StringReader(sqlScript))
            {
                fragment = parser.Parse(reader, out errors);
            }

            // Check for parse errors
            if (errors.Count > 0)
            {
                Console.WriteLine("Errors found while parsing the script:");
                foreach (var error in errors)
                {
                    Console.WriteLine($"Line {error.Line}: {error.Message}");
                }
            }
            else
            {
                Console.WriteLine("Parsing completed successfully.");
            }

            // Analyze the parsed script (example: detect potential SQL injection)
            AnalyzeSqlFragment(fragment);
        }

        private static void AnalyzeSqlFragment(TSqlFragment fragment)
        {
            // Example analysis: detect potential SQL injection (this is a simple example, real detection would be more complex)
            var visitor = new SqlInjectionDetectionVisitor();
            fragment.Accept(visitor);

            if (visitor.PotentialInjectionDetected)
            {
                Console.WriteLine("Potential SQL injection detected.");
            }
            else
            {
                Console.WriteLine("No SQL injection detected.");
            }
        }
    }

    // Custom visitor to detect potential SQL injection
    public class SqlInjectionDetectionVisitor : TSqlFragmentVisitor
    {
        public bool PotentialInjectionDetected { get; private set; }

        public override void Visit(TSqlBatch node)
        {
            base.Visit(node);

            // Example check: look for string concatenations in WHERE clauses
            foreach (var statement in node.Statements)
            {
                if (statement is TSqlStatement sqlStatement)
                {
                    sqlStatement.Accept(new SqlInjectionCheckVisitor(this));
                }
            }
        }
    }

    public class SqlInjectionCheckVisitor : TSqlFragmentVisitor
    {
        private readonly SqlInjectionDetectionVisitor _parentVisitor;

        public SqlInjectionCheckVisitor(SqlInjectionDetectionVisitor parentVisitor)
        {
            _parentVisitor = parentVisitor;
        }

        public override void Visit(BooleanComparisonExpression node)
        {
            base.Visit(node);

            // Check for string concatenations (simplified example)
            if (node.FirstExpression is StringLiteral || node.SecondExpression is StringLiteral)
            {
                _parentVisitor.PotentialInjectionDetected = true;
            }
        }
    }
    ```

### Explanation:

- **TSql150Parser**: This class provides a parser for T-SQL scripts based on the SQL Server 2019 (version 15.0) syntax.
- **ParseSqlScript**: This method parses the provided SQL script and checks for errors.
- **AnalyzeSqlFragment**: This method uses a custom visitor to analyze the parsed SQL fragment.
- **SqlInjectionDetectionVisitor**: A custom visitor class that checks for potential SQL injection patterns, such as string concatenations in WHERE clauses.

### Notes:

- **Security**: This example provides a basic approach to detecting SQL injection. In a real-world application, you would need more sophisticated analysis and validation.
- **Visitor Pattern**: The visitor pattern is used to traverse the parsed SQL script and perform custom analysis.

By using the `TSqlParser` class, you can analyze and understand the structure of T-SQL scripts, making it easier to detect potential vulnerabilities and improve the security of your database interactions.

### Detailed Explanation of the `TSqlParser` Class

The `TSqlParser` class is part of the Microsoft.SqlServer.TransactSql.ScriptDom namespace and is used to parse T-SQL scripts. It provides methods for parsing T-SQL scripts and generating a syntax tree that represents the structure of the script. This syntax tree can be used for various purposes, such as analyzing the script, detecting potential vulnerabilities, and generating modified versions of the script.

#### Key Methods and Properties

- **Parse(TextReader, out IList<ParseError>)**: Parses the T-SQL script from the provided `TextReader` and returns a `TSqlFragment` representing the syntax tree. Any parse errors are returned in the `IList<ParseError>` parameter.
- **ParseFragment(TextReader, out IList<ParseError>)**: Similar to the `Parse` method, but returns a `TSqlFragment` that represents a fragment of a T-SQL script.
- **InitialQuotedIdentifiers**: Gets or sets a value indicating whether quoted identifiers are initially enabled.

#### Example Usage

Here is an example of how to use the `TSqlParser` class to parse a T-SQL script and analyze its structure:

```csharp
using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.SqlServer.TransactSql.ScriptDom;

public class TSqlParserExample
{
    public static void Main(string[] args)
    {
        // Example T-SQL script to be parsed
        string sqlScript = "SELECT * FROM Users WHERE Username = 'admin';";

        // Parse the SQL script
        ParseSqlScript(sqlScript);
    }

    private static void ParseSqlScript(string sqlScript)
    {
        // Create a TSql parser
        TSqlParser parser = new TSql150Parser(true);

        // Parse the script
        IList<ParseError> errors;
        TSqlFragment fragment;
        using (TextReader reader = new StringReader(sqlScript))
        {
            fragment = parser.Parse(reader, out errors);
        }

        // Check for parse errors
        if (errors.Count > 0)
        {
            Console.WriteLine("Errors found while parsing the script:");
            foreach (var error in errors)
            {
                Console.WriteLine($"Line {error.Line}: {error.Message}");
            }
        }
        else
        {
            Console.WriteLine("Parsing completed successfully.");
        }

        // Analyze the parsed script (example: detect potential SQL injection)
        AnalyzeSqlFragment(fragment);
    }

    private static void AnalyzeSqlFragment(TSqlFragment fragment)
    {
        // Example analysis: detect potential SQL injection (this is a simple example, real detection would be more complex)
        var visitor = new SqlInjectionDetectionVisitor();
        fragment.Accept(visitor);

        if (visitor.PotentialInjectionDetected)
        {
            Console.WriteLine("Potential SQL injection detected.");
        }
        else
        {
            Console.WriteLine("No SQL injection detected.");
        }
    }
}

// Custom visitor to detect potential SQL injection
public class SqlInjectionDetectionVisitor : TSqlFragmentVisitor
{
    public bool PotentialInjectionDetected { get; private set; }

    public override void Visit(TSqlBatch node)
    {
        base.Visit(node);

        // Example check: look for string concatenations in WHERE clauses
        foreach (var statement in node.Statements)
        {
            if (statement is TSqlStatement sqlStatement)
            {
                sqlStatement.Accept(new SqlInjectionCheckVisitor(this));
            }
        }
    }
}

public class SqlInjectionCheckVisitor : TSqlFragmentVisitor
{
    private readonly SqlInjectionDetectionVisitor _parentVisitor;

    public SqlInjectionCheckVisitor(SqlInjectionDetectionVisitor parentVisitor)
    {
        _parentVisitor = parentVisitor;
    }

    public override void Visit(BooleanComparisonExpression node)
    {
        base.Visit(node);

        // Check for string concatenations (simplified example)
        if (node.FirstExpression is StringLiteral || node.SecondExpression is StringLiteral)
        {
            _parentVisitor.PotentialInjectionDetected = true;
        }
    }
}
```

In this example, the `TSqlParser` class is used to parse a T-SQL script and generate a syntax tree. The `SqlInjectionDetectionVisitor` class is a custom visitor that traverses the syntax tree and checks for potential SQL injection patterns. The `SqlInjectionCheckVisitor` class is used to check for specific patterns, such as string concatenations in WHERE clauses.

By using the `TSqlParser` class and custom visitors, you can analyze the structure of T-SQL scripts and detect potential vulnerabilities, making it easier to improve the security of your database interactions.
