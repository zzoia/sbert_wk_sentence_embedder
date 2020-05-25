SET ASPNETCORE_ENVIRONMENT=Development
SET LAUNCHER_PATH=d:\Study\11 semester\TextClustering\TextClustering.Web\bin\Debug\netcoreapp3.1\TextClustering.Web.exe
cd /d "C:\Program Files\IIS Express\"
iisexpress.exe /config:"D:\Study\11 semester\TextClustering\.vs\TextClustering\config\applicationhost.config" /site:"TextClustering.Web" /apppool:"TextClustering.Web AppPool"