%define _topdir		.
%define name		la-storia-in-giallo
%define release		1
%define version		2.0
%define buildroot	%{_topdir}/%{name}-%{version}-root

BuildRoot:		../BUILDROOT
Summary:		La storia in giallo
License:		MIT/X11
Name:			%{name}
Version:		%{version}
Release:		%{release}
Group:			Network/Tools
URL:			http://paolobernardi.wordpress.com

Requires:		mono-winforms mplayer lame

%description
Scarica le puntate de La storia in giallo e Cuore di Tenebra

%build
make

%install
DESTDIR=$RPM_BUILD_ROOT make install

%files
%defattr(-,root,root)
/usr/bin/lstg
/usr/share/applications/lstg.desktop
/usr/share/lstg/data/episodes-cdtd.txt
/usr/share/lstg/data/episodes-cdto.txt
/usr/share/lstg/data/episodes-lstg.txt
/usr/share/lstg/data/icon.ico
/usr/share/lstg/data/lstg.svg
/usr/share/lstg/lstg.exe
/usr/share/lstg/taglib-sharp.dll

