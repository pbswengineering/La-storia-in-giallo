del "../La storia in giallo.exe"
csc -out:../"La storia in giallo.exe" -target:winexe -win32icon:../data/icon.ico -r:../taglib-sharp.dll -r:System.Windows.Forms.dll *.cs
