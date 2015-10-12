param([bool]$force = $true,[string]$configuration = "Debug", $output = "..\nuget\", [string]$repo = "lzy\")

$projects = @(
    @{Path = '.\Framework\'; Project = 'LazyFramework'}
   ,@{Path = '.\LazyFramework.Logging\'; Project = 'LazyFramework.Logging'}
   ,@{Path = '.\Lazyframework.Data\'; Project = 'LazyFramework.Data'}
   ,@{Path = '.\SqlServer\'; Project = 'LazyFramework.MSSqlServer'}
   ,@{Path = '.\LazyFramework.EventHandling\'; Project = 'LazyFramework.EventHandling'}
   ,@{Path = '.\LazyFramework.CQRS\'; Project = 'LazyFramework.CQRS'}
)

$output = $output + $repo


if(!(Test-Path $output)){ New-Item $output -ItemType Directory}

$saveHash = $output+"lastbuild.log"
#Skal vi slette alle versionene inne i denne katalogen før vi bygger????

$remove = $output + "*.nupkg"
del $remove

$lastRev = ""
$currRev = git log -1 --format=%H

if(Test-Path $saveHash) {
    $lastRev = Get-Content($saveHash)
}


#Build solution
C:\Windows\Microsoft.NET\Framework64\v4.0.30319\MSBuild.exe .\LazyFramework.sln /p:Configuration=$configuration /t:Clean,Rebuild /nologo /v:q



$projects | % {

    $_.Project

    $outstanding = (git status $_.Path --porcelain) | Out-String
    $msg = ((git log $lastRev`.`.$currRev --format=%B $_.Path) | Out-String )

    if(!($force)) {
        if (!($outstanding -eq "")){
            Write-Host $outstanding
            Write-Host "Commit all changes before building"
            return
        }


        if (($currRev -eq $lastRev) -or ($msg -eq ""))   {
            ": nothing to build "
            return
        }
    }

    #Updating spec file with release notes.
     $specFile = (Resolve-Path $_.Path).Path + $_.Project + ".nuspec"
     [xml]$xml = Get-Content $specFile
     $xml.package.metadata.releaseNotes = $msg.ToString()
     #$xml.package.metadata.version = $xml.package.metadata.version + "-alpha"
     $xml.Save($specFile)
    #End

    $p = $_.Path+$_.Project+".vbproj"
    
    $p
        
    
    .\nuget pack $p  -OutputDirectory $output -IncludeReferencedProjects -Symbols

    #Reverting spec file
    git checkout $specFile
    "Release notes:"
    $msg
}

"Pushing symbols"

$output = "..\nuget\" + $repo
$symbolServer = "http://symbol.itaslan.infotjenester.no/nuget/Core"

Get-ChildItem $output -Filter *.symbols.nupkg | % {
    .\nuget push $_.FullName p:p  -source $symbolServer
}

$currRev | Set-Content $saveHash


