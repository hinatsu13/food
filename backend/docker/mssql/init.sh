#!/usr/bin/env bash
set -e

SQLCMD="/opt/mssql-tools18/bin/sqlcmd -C"

EXISTS=$($SQLCMD -S mssql -U sa -P "$MSSQL_SA_PASSWORD" -h -1 -W \
    -Q "SET NOCOUNT ON; SELECT COUNT(*) FROM sys.databases WHERE name='HRIS'" \
    | head -1 | tr -d '[:space:]')

if [ "$EXISTS" -gt 0 ]; then
    echo "Database HRIS already exists; skipping init."
    exit 0
fi

echo "Initializing HRIS database..."
for f in /init/*.sql; do
    echo "Applying $f..."
    $SQLCMD -S mssql -U sa -P "$MSSQL_SA_PASSWORD" -i "$f" -b
done
echo "Initialization complete."