$username = "test"
$password = $env:SYSTEM_ACCESSTOKEN
$userpass  = "$($username):$($password)"
$bytes= [System.Text.Encoding]::UTF8.GetBytes($userpass)
$encodedlogin=[Convert]::ToBase64String($bytes)
$authheader = "Basic " + $encodedlogin
$headers = New-Object "System.Collections.Generic.Dictionary[[String],[String]]"
$headers.Add("Authorization",$authheader)
$headers.Add("Accept","application/json")
$headers.Add("Content-Type","application/json")
$uri = ${env:ACRBuildURL}
$status= $null

do
{
   Start-Sleep -s 15
   $response = Invoke-RestMethod -Uri $uri -Headers $headers -Method Get -ContentType "application/json"
   $status = $response.value.result[0]
   Write-host $status
}while($status -eq $null)

Write-Output "##vso[task.setvariable variable=ACRBuildStatus;]$status"

if($status -eq 'failed') {
  Write-Output "##vso[task.logissue type=error] Docker Image Build Failed"
}