Param
(
    [Parameter (Mandatory = $true)]
    [string] $secretName,
  
    [Parameter (Mandatory = $true)]
    [string] $vaultName,

    [Parameter (Mandatory = $true)]
    [string] $aadApplicationName
)

## First, connect to Azure using the AzureRunAsConnection
$connectionName = "AzureRunAsConnection"
try
{
    # Get the connection "AzureRunAsConnection "
    $servicePrincipalConnection=Get-AutomationConnection -Name $connectionName         

    "Logging in to Azure..."
    Connect-AzureRmAccount `
        -ServicePrincipal `
        -TenantId $servicePrincipalConnection.TenantId `
        -ApplicationId $servicePrincipalConnection.ApplicationId `
        -CertificateThumbprint $servicePrincipalConnection.CertificateThumbprint
    "Login complete."
}
catch {
    if (!$servicePrincipalConnection)
    {
        $ErrorMessage = "Connection $connectionName not found."
        throw $ErrorMessage
    } else{
        Write-Error -Message $_.Exception
        throw $_.Exception
    }
}

## Check that the AAD Application Exists
try {
    "Getting AD Application..."
    $adApplication = Get-AzureRmADApplication -DisplayNameStartWith $aadApplicationName
    
}
catch {
    if (!$adApplication) {        
        $ErrorMessage = "Application $aadApplicationName not found."
        throw $ErrorMessage
    }
    else {
        Write-Error -Message $_.Exception
        throw $_.Exception
    }
}

## Now check that a Service Principal exists for that AAD Application
try {
    "Getting Service Principal..."
    $servicePrincipal = Get-AzureRmADServicePrincipal -ServicePrincipalName $adApplication.ApplicationId
    
}
catch {
    if (!$servicePrincipal) {        
        $ErrorMessage = "Service Principal belonging to $aadApplication.DisplayName found."
        throw $ErrorMessage
    }
    else {
        Write-Error -Message $_.Exception
        throw $_.Exception
    }
}

## Check whether an existing key exists
$existingKey = Get-AzureRmADSpCredential -ObjectId $servicePrincipal.Id
if ($existingKey) {
    "At least one Service Principal Credential exists"
}
else {
    "No Service Principal credentials exist"
}

## Generate a new key for the Service Principal
$randomString = -join ((65..90) + (97..122) | Get-Random -Count 30 | % {[char]$_})
$secretValue = ConvertTo-SecureString $randomString -AsPlainText -Force

## Create the Service Principal credential with the generated key
try {
    $newKey = New-AzureRmADSpCredential -ObjectId $servicePrincipal.Id -Password $secretValue
    $secret = Set-AzureKeyVaultSecret -VaultName $vaultName -Name $secretName -SecretValue $secretValue
}
catch {
    if ($newKey) {           
        $ErrorMessage = "New Service Principal Credential could not be created"
        throw $ErrorMessage
    }
    elseif (!$secret) {   
        $ErrorMessage = "Secret could not be written to KeyVault"
        throw $ErrorMessage        
    }
    else {
        Write-Error -Message $_.Exception
        throw $_.Exception
    }
}

## Delete any existing keys that were previously aligned to the Service Principal
if($existingKey){
    Foreach ($credential in $existingKey) {
        Remove-AzureRmADSpCredential -ObjectId $ServicePrincipal.Id -KeyId $credential.KeyId -Confirm:$false -Force
    }
}
