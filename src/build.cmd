del "../La storia in giallo.exe"
csc -out:../"La storia in giallo.exe" -target:winexe -win32icon:../data/icon.ico -r:../taglib-sharp.dll -r:System.Windows.Forms.dll Main.cs Episode.cs EpisodeDB.cs Crawler.cs ExternalSoftware.cs
