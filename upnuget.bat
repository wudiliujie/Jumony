.\.Nuget\nuget pack .\Ivony.Html\Ivony.Html.csproj -Build -Prop Configuration=Release -o .\.Nuget\Temp
.\.Nuget\nuget pack .\Ivony.Html.Parser\Ivony.Html.Parser.csproj -Build -Prop Configuration=Release -o .\.Nuget\Temp


.\.Nuget\nuget  push .\.Nuget\Temp\*.nupkg -s https://www.nuget.org/ d821f6ef-c0ac-4f38-8c01-55268678b048 
del .\.Nuget\Temp\*.nupkg