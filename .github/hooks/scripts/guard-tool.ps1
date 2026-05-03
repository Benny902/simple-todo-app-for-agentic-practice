$raw = [Console]::In.ReadToEnd()
$ts  = (Get-Date).ToString("HH:mm:ss")

try {
    $payload  = $raw | ConvertFrom-Json
    $combined = "$($payload.toolName) $($payload.toolArgs)"
} catch {
    $combined = $raw
}

if ($combined -match 'rm\s+-rf|sudo\s|git\s+push\s+(--force|-f)|DROP\s+TABLE|Remove-Item.*-Recurse.*-Force') {
    Add-Content -Path ".github\hooks\hooks.log" -Value "$ts [guard]   BLOCKED: $combined"
    @{
        permissionDecision       = "deny"
        permissionDecisionReason = "Blocked dangerous operation by guard-tool.ps1: $combined"
    } | ConvertTo-Json -Compress
    exit 0
}

exit 0