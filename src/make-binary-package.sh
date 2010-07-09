#!/bin/bash

V="$(head -n 1 ../VERSIONI.txt | cut -d' ' -f1)"
echo "Versione: $V"
TARBALL="la-storia-in-giallo_${V}_eseguibili_linux.tar.bz2"
echo "Tarball: $TARBALL"

cd ..
make

mkdir la-storia-in-giallo

cp src/lstg.exe la-storia-in-giallo/'La storia in giallo.exe'
cp *.dll la-storia-in-giallo
cp -fr doc la-storia-in-giallo
cp -fr data la-storia-in-giallo

tar cvjf "$TARBALL" la-storia-in-giallo
rm -fr la-storia-in-giallo
