param(
  [string]$folder = './',
  [string] $outputFile = 'TestRun.xml'
)

#Register-PSRepository -Default -InstallationPolicy Trusted
#Install-PackageProvider -Name NuGet -Force -Scope CurrentUser
Install-Module -Name Pester -Force -Verbose -Scope CurrentUser
 
Import-Module Pester
Invoke-Pester -OutputFile $outputFile -OutputFormat NUnitXml
Get-ChildItem "$folder" -Filter *.json | Foreach-Object {
    Write-Output "=> $($_.Name)"

    
    $templateProperties = (get-content "$_." -ErrorAction SilentlyContinue | ConvertFrom-Json -ErrorAction SilentlyContinue)
    
    Describe "JSON Structure" {   
  
  
        It "$($_.Name) should be less than 1 Mb" {
            Get-Item $_.Name | Select-Object -ExpandProperty Length | Should -BeLessOrEqual 1073741824
        }
        
        It "Converts from JSON" {
            $templateProperties | Should -Not -BeNullOrEmpty
        }

    
        It "should have a `$schema section" {
            $templateProperties."`$schema" | Should -Not -BeNullOrEmpty
        }

        It "should have a contentVersion section" {
            $templateProperties.contentVersion | Should -Not -BeNullOrEmpty
        }

        It "should have a parameters section" {
            $templateProperties.parameters | Should -Not -BeNullOrEmpty
        }

        It "should have less than 256 parameters" {
            $templateProperties.parameters.Length | Should -BeLessOrEqual 256
        }

        It "might have a variables section" {
            $result = $null -eq $templateProperties.variables

            if ($result) {
                $result | Should -Be $true
            }
            else {
                Set-TestInconclusive -Message "Section isn't mandatory, however it's a group practice to have it defined"
            }
        } 

        It "should not have a variable for apiVersion" {
            $result = $null -eq $templateProperties.variables.apiVersion
            $result | Should -Be $true
        } 

        It "should have a default value of resource group location or no location parameter" {
            $result = $null -eq $templateProperties.parameters.location

            if ($result) {
                $result | Should -Be $true
            }
            else {
                $result = "[ResourceGroup().location]" -eq $templateProperties.parameters.location.defaultValue 
                $result | Should -Be $true
            }
        }

    
        It "should not use allowedValues for location" {
            $result = $null -eq $templateProperties.parameters.location

            if ($result) {
                $result | Should -Be $true
            }
            else {
                $result = $null -eq $templateProperties.parameters.location.allowedValues 
                $result | Should -Be $true
            }
        }    

        It "should not define a default value for secure strings" {
            ForEach ($parameterSet in $templateProperties.parameters.psobject.Members | where-object membertype -like 'noteproperty') {
                Foreach ($parameter in $parameterSet.Value) {
                    $result = $parameter.type -eq "securestring" -and $parameter.defaultValue -ne $null
                    $result | Should -Be $false
                }

            }
        }

        It "should have a metadata description for each parameter" {
            ForEach ($parameterSet in $templateProperties.parameters.psobject.Members | where-object membertype -like 'noteproperty') {
                Foreach ($parameter in $parameterSet.Value) {
                    $result = $parameter.metadata.description -eq $null
                    $result | Should -Be $false
                }
            }
        }

        It "should have a comment for each resource" {
            ForEach ($resource in $templateProperties.resources) {
                $result = $resource.comments -eq $null
                $result | Should -Be $false
            }
        }
    }
}