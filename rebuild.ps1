$solutionPath = Join-Path $PSScriptRoot "Congratulator.sln"

Write-Host "Building Congratulator..." -ForegroundColor Cyan

$result = dotnet build $solutionPath --verbosity minimal 2>&1
$exitCode = $LASTEXITCODE

foreach ($line in $result) {
    if ($line -match "error") {
        Write-Host $line -ForegroundColor Red
    } elseif ($line -match "warning") {
        Write-Host $line -ForegroundColor Yellow
    } else {
        Write-Host $line
    }
}

if ($exitCode -eq 0) {
    Write-Host "`nBuild succeeded." -ForegroundColor Green
} else {
    Write-Host "`nBuild FAILED." -ForegroundColor Red
}

exit $exitCode