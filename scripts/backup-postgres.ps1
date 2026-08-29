param([string]$OutputDirectory = (Join-Path $PSScriptRoot '..\backups'))

if ([string]::IsNullOrWhiteSpace($env:DATABASE_URL)) {
    throw 'Set DATABASE_URL to the PostgreSQL connection string before running this script.'
}

New-Item -ItemType Directory -Force -Path $OutputDirectory | Out-Null
$backup = Join-Path $OutputDirectory ("workouttracker-{0}.dump" -f (Get-Date -Format 'yyyyMMdd-HHmmss'))
& pg_dump --dbname=$env:DATABASE_URL --format=custom --no-owner --file=$backup
if ($LASTEXITCODE -ne 0) { throw "pg_dump failed with exit code $LASTEXITCODE." }
Write-Output "Backup created: $backup"
