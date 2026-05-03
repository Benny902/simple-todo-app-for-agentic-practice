$ts  = (Get-Date).ToString("HH:mm:ss")
$msg = "Copilot subagent finished"

Add-Content -Path ".github\hooks\hooks.log" -Value "$ts [notify]  subagent finished"

try {
    Add-Type -AssemblyName System.Windows.Forms
    Add-Type -AssemblyName System.Drawing
    $ni = New-Object System.Windows.Forms.NotifyIcon
    $ni.Icon = [System.Drawing.SystemIcons]::Information
    $ni.BalloonTipTitle = "Copilot CLI"
    $ni.BalloonTipText  = $msg
    $ni.Visible = $true
    $ni.ShowBalloonTip(3000)
    Start-Sleep -Milliseconds 1500
    $ni.Dispose()
} catch {
    Write-Host "[Copilot CLI] $msg"
}
exit 0