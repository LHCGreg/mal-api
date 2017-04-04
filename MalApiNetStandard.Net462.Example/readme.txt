To use the netstandard version of MalApi from a regular .NET program, you need to do two things to work around current issues with netstandard libraries:

1) Install the System.Net.Http NuGet package. It might have to be package version 4.3.1. I am not sure if a later version will work or not.

2) (may not be necessary) Edit your .csproj file. In an <ItemGroup> add the following to ensure that transitive references (references of your references) get copied to the bin folder:

<PackageReference Include="Legacy2CPSWorkaround" Version="1.0.0">
    <PrivateAssets>All</PrivateAssets>
</PackageReference>
