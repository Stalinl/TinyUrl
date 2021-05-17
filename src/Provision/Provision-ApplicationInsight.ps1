# This script is used to provision Application Insight.
param (
    [parameter(Mandatory=$true, Position=0)]
    [string]$ResourceGroupName,
    
    [parameter(Mandatory=$true, Position=1)]
    [string]$Name,

    [parameter(Mandatory=$true, Position=2)]
    [string]$Location,

    [parameter(Mandatory=$true, Position=3)]
    [string]$Kind,

    [parameter(Mandatory=$false, Position=4)]
    [hashtable]$Tags
)

$appInsight = Get-AzureRmApplicationInsights -ResourceGroupName $ResourceGroupName -Name $Name -ErrorAction SilentlyContinue;

if($appInsight -eq $null) {
    New-AzureRmApplicationInsights -ResourceGroupName $ResourceGroupName -Name $Name -Location $Location -Kind $Kind -Tag $Tags;
}
else{
    Write-Host "Application Insight already exists. Name: $Name" -ForegroundColor Yellow;
}