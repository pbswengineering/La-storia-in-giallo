#!/bin/bash
mkdir BUILDROOT RPMS SOURCES SPECS SRPMS
ln -s .. BUILD
mv *.spec SPECS
rpmbuild -v -bb --clean --target=noarch SPECS/*.spec
mv SPECS/*.spec .
rm BUILD
rmdir BUILDROOT SOURCES SPECS SRPMS
mv RPMS/noarch/*.rpm .
rm -fr RPMS
( cd .. && make clean )
