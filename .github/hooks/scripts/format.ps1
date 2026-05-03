$raw = [Console]::In.ReadToEnd()
$ts  = (Get-Date).ToString("HH:mm:ss")

try {
    $payload = $raw | ConvertFrom-Json
    $tool    = [string]$payload.toolName
} catch {
    $tool = "?"
}

# Only run formatters after edit-class tools.
if ($tool -notmatch '^(edit|write|multiedit|notebookedit|Edit|Write|MultiEdit|NotebookEdit)$') {
    exit 0
}

npx --yes prettier --write "frontend/**/*.{ts,tsx,js,json,md}" *> $null
Push-Location backend
dotnet format *> $null
Pop-Location

Add-Content -Path ".github\hooks\hooks.log" -Value "$ts [format]  repo formatted (trigger=$tool)"
Write-Output "formatted repo"
exit 0