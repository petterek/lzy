param([switch]$Force = $false,[string]$Configuration = "Release", $Output = "..\nuget\",[string] $SymbolServer = "http://nuget.infotjenester.no:8080/nuget/Default" , [string]$Repo = "lzy\")

$projects = @(
    @{Path = '.\LazyFramework.Reflection\'; Project = "LazyFramework.Reflection"}
   ,@{Path = '.\Framework\'; Project = 'LazyFramework'}
   ,@{Path = '.\LazyFramework.ClassFactory\'; Project = 'LazyFramework.ClassFactory'}
   ,@{Path = '.\LazyFramework.Logging\'; Project = 'LazyFramework.Logging'}
   ,@{Path = '.\Lazyframework.Data\'; Project = 'LazyFramework.Data'}
   ,@{Path = '.\SqlServer\'; Project = 'LazyFramework.MSSqlServer'}
   ,@{Path = '.\LazyFramework.EventHandling\'; Project = 'LazyFramework.EventHandling'}
   ,@{Path = '.\LazyFramework.CQRS\'; Project = 'LazyFramework.CQRS'}
)

if($force){ "C:\Program Files (x86)\MSBuild\14.0\Bin\MSBuild.exe .\LazyFramework.sln /p:Configuration=$configuration /t:Clean,Rebuild /nologo /v:q"}

$output = $output + $repo


if(!(Test-Path $output)){ New-Item $output -ItemType Directory}

$saveHash = $output+"lastbuild.log"
#Skal vi slette alle versionene inne i denne katalogen før vi bygger????

$lastRev = ""
$currRev = git log -1 --format=%H

if(Test-Path $saveHash) {
    $lastRev = Get-Content($saveHash)
}

$packed = New-Object System.Collections.ArrayList
$toBuild = New-Object System.Collections.ArrayList


$projects | % {

    $outstanding = (git status $_.Path --porcelain) | Out-String
    $msg =  ((git log $lastRev`.`.$currRev --format=%B $_.Path) | Out-String )
    $add = $true;

    $msg

    if(!($force)) {
            if (!($outstanding -eq "")){
                Write-Host $outstanding
                Write-Host "Commit all changes before building"
                $add = $false
            }

            if (($currRev -eq $lastRev) -or ($msg -eq ""))   {
                ": nothing to build "
                $add = $false
            }

            if($add){
                $toBuild.Add($_)
            }


        }else {
            $toBuild = $projects
        }

    #Update authors
	$specFile = (Resolve-Path $_.Path).Path + $_.Project + ".nuspec"
	[xml]$xml = Get-Content $specFile
    $author = ((git log --all --format='%aN' | sort -u) | Out-String).ToString()
	$xml.package.metadata.authors =  $author
    $xml.package.metadata.releaseNotes = "`r`n" + ((git log -1 --pretty=oneline) | Out-String) + $msg.ToString()
	$xml.Save($specFile)

}

$toBuild | % {

    $_.Project

    #$match = $_.Project + "\.\d+\.\d+\.\d+\.\d+\..*"

    #Get-ChildItem $output |
    #Where-Object {$_.Name -match "$match"} | % {
    #   del $_.FullName
    #}

	$p = $_.Path + $_.Project + ".*proj"
    $p = (Get-Item $p).FullName

    if($force){
        .\nuget pack $p  -OutputDirectory $output -IncludeReferencedProjects -Symbols
        }
        else{
        .\nuget pack $p  -OutputDirectory $output -IncludeReferencedProjects -Symbols -Build
        }

	$packed.Add($_.Project)


    "Release notes:"
    $msg
}

"Reverting nuspec files"
git checkout *.nuspec

"Writing revison info"
$currRev | Set-Content $saveHash


"Pushing symbols"
$output = "..\nuget\" + $repo

$packed | % {
     $match = $_ + "\.\d+\.\d+\.\d+\.\d+\.symbols"
     Get-ChildItem $output |
     Where-Object {$_.Name -match "$match"} | % {
        .\nuget push $_.FullName  Admin:Admin -source $symbolServer
    }

}