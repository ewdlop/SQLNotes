#!/bin/sh
# Pre-commit hook to lint SQL files with SQLFluff

# Get a list of all staged SQL files
sql_files=$(git diff --cached --name-only --diff-filter=ACM | grep -E '\.sql$')

if [ -z "$sql_files" ]; then
    echo "No SQL files to lint."
    exit 0
fi

# Run SQLFluff lint on each staged SQL file
echo "Linting SQL files..."
for file in $sql_files; do
    sqlfluff lint "$file"
    if [ $? -ne 0 ]; then
        echo "Linting failed for $file. Please fix the issues before committing."
        exit 1
    fi
done

echo "All SQL files passed linting."
exit 0
