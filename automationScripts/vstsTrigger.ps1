Param(
    [Parameter (Mandatory = $true)]
    [string]$vstsAccount = "reddobowen",

    
    [Parameter (Mandatory = $true)]
    [string]$projectName = "Phantom",

    
    [Parameter (Mandatory = $true)]
    [string]$user,

    
    [Parameter (Mandatory = $true)]
    [string]$token,
    
    [Parameter (Mandatory = $true)]
    [string]$releaseDefinitionName
)

# Base64-encodes the Personal Access Token (PAT) appropriately
$base64AuthInfo = [Convert]::ToBase64String([Text.Encoding]::ASCII.GetBytes(("{0}:{1}" -f $user, $token)))

 
# Construct the REST URL to obtain Release Definition ID
$findReleaseDefinitionURI = "https://$($vstsAccount).vsrm.visualstudio.com/$($projectName)/_apis/release/definitions?api-version=4.1-preview.3&searchText=$($releaseDefinitionName)"
$result = Invoke-RestMethod -Uri $findReleaseDefinitionURI -Method Get -ContentType "application/json" -Headers @{Authorization = ("Basic {0}" -f $base64AuthInfo)} -UseBasicParsing

## Assign the definition ID to a variable, and use that in the trigger release URL

try {
    $releaseDefinitionId = $result[0].value.id
}
catch {
    if (!$releaseDefinitionId) {
        $ErrorMessage = "There are no Release Definitions with that name"
        throw $ErrorMessage
    }
    else {
        Write-Error -Message $_.Exception
        throw $_.Exception
    }
}
 
# Construct the REST URL to obtain Build ID
$triggerReleaseURI = "https://$($vstsAccount).vsrm.visualstudio.com/$($projectName)/_apis/release/releases?api-version=4.1-preview.6"


$JSON = "
{
    `"definitionId`": ${releaseDefinitionId},
    `"description`": `"Creating Sample Release`",
    `"isDraft`": false,
    `"reason`": `"none`",
    `"manualEnvironments`": null
}"

Invoke-WebRequest -Uri $triggerReleaseURI -Method POST -Body $JSON -ContentType "application/json" -Headers @{Authorization = ("Basic {0}" -f $base64AuthInfo)} -UseBasicParsing