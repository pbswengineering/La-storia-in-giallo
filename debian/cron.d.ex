#
# Regular cron jobs for the la-storia-in-giallo package
#
0 4	* * *	root	[ -x /usr/bin/la-storia-in-giallo_maintenance ] && /usr/bin/la-storia-in-giallo_maintenance
