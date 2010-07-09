#!/bin/bash

V="$(head -n 1 ../VERSIONI.txt | cut -d' ' -f1)"
echo "Versione: $V"
TARBALL="la-storia-in-giallo_${V}_sorgenti.tar.bz2"
echo "Tarball: $TARBALL"

chmod -x *
chmod +x *.cmd *.sh

cd ..
rm -f 'La storia in giallo.exe'
chmod -x *.*

cd apps
rm -fr mplayer
chmod -x *
chmod +x *.exe

cd ../data
chmod -x *

cd ../doc
chmod -x *

cd ../nsis
chmod -x *

cd ../..

tar cvjf "$TARBALL" 'La storia in giallo'
mv "$TARBALL" 'La storia in giallo'
cd 'La storia in giallo'/src

