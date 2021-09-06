<#
.SYNOPSIS
	Build a .env.production configuration file
.DESCRIPTION
	.env.production can be useful for building front-end assets
	if passing variables and arguments to build tools does not work
	(this is the case in some build environments)
.PARAMETER ApiUrl
	URL of Application API
#>

param (
	[string] $ApiUrl
)

Write-Output "ApiUrl: $ApiUrl"

$dotEnv = Get-Content -Path .env
$dotEnvForProd = $dotEnv.Replace("VITE_API_URL=", "VITE_API_URL=$ApiUrl")

Set-Content -Path .env.production -Value $dotEnvForProd
