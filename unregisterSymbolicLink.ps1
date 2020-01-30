$AOSMetadataPath = "K:\AOSService\PackagesLocalDirectory"
$RepoPath = "."
$RepoMetadataPath = $RepoPath + "\Metadata"
$RepoModelFolders = Get-ChildItem $RepoMetadataPath -Directory
foreach ($ModelFolder in $RepoModelFolders)
{
	cmd /c rmdir "$AOSMetadataPath\$ModelFolder"
}