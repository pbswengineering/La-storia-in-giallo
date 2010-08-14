.PHONY: clean install uninstall source-archive binary-archive

EXE=lstg.exe
ICON=icon.ico
SVG=lstg.svg
DESKTOP=lstg.desktop
SCRIPT=lstg
SOURCES=$(wildcard src/*.cs)
TAGLIB=taglib-sharp.dll
INSTALL_DIR=$(DESTDIR)/usr/bin
SHARE_DIR=$(DESTDIR)/usr/share/lstg
DESKTOP_DIR=$(DESTDIR)/usr/share/applications

$(EXE): $(SOURCES)
	gmcs -out:src/$(EXE) -target:winexe -win32icon:data/icon.ico -r:taglib-sharp.dll -r:System.Windows.Forms.dll -r:System.Drawing $(SOURCES)
	cp src/lstg.exe .

clean:
	rm -f src/$(EXE) $(EXE) *.tar.bz2
	find . -name *~ -exec rm {} \;

install: $(EXE)
	test -d $(INSTALL_DIR) || mkdir -p $(INSTALL_DIR)
	test -d $(SHARE_DIR)/data || mkdir -p $(SHARE_DIR)/data
	test -d $(DESKTOP_DIR) || mkdir -p $(DESKTOP_DIR)
	cp -f data/$(SCRIPT) $(INSTALL_DIR)/
	cp -f src/$(EXE) $(SHARE_DIR)/
	cp -f $(TAGLIB) $(SHARE_DIR)/
	cp -f data/$(ICON) $(SHARE_DIR)/data/
	cp -f data/$(SVG) $(SHARE_DIR)/data/
	cp -f $(wildcard data/episodes-*.txt) $(SHARE_DIR)/data/
	cp -f data/$(DESKTOP) $(DESKTOP_DIR)/
	update-desktop-database || true

uninstall:
	rm -f $(INSTALL_DIR)/$(SCRIPT)
	rm -f $(SHARE_DIR)/$(EXE)
	rm -f $(SHARE_DIR)/$(TAGLIB)
	rm -f $(SHARE_DIR)/data/$(ICON)
	rm -f $(SHARE_DIR)/data/$(SVG)
	rm -f $(SHARE_DIR)/data/$(EPISODES)
	rmdir $(SHARE_DIR)/data
	rmdir $(SHARE_DIR)
	rm -f $(DESKTOP_DIR)/$(DESKTOP)
	update-desktop-database || true

source-archive:
	( cd src && ./make-source-package.sh )

binary-archive:
	( cd src && ./make-binary-package.sh )

