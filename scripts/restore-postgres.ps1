param(
    [Parameter(Mandatory)][string]$BackupFile,
    [switch]$Force
)

if ([string]::IsNullOrWhiteSpace($env:DATABASE_URL)) {
    throw 'Set DATABASE_URL to the target PostgreSQL connection string before running this script.'
}
if (-not $Force) { throw 'Restore replaces matching database objects. Re-run with -Force after verifying the target.' }

$resolvedBackup = (Resolve-Path -LiteralPath $BackupFile).Path
& pg_restore --dbname=$env:DATABASE_URL --clean --if-exists --no-owner $resolvedBackup
if ($LASTEXITCODE -ne 0) { throw "pg_restore failed with exit code $LASTEXITCODE." }
Write-Output "Restore completed from: $resolvedBackup"
