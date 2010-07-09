;; -*-emacs-lisp-*-
;;
;; Emacs startup file, e.g.  /etc/emacs/site-start.d/50la-storia-in-giallo.el
;; for the Debian la-storia-in-giallo package
;;
;; Originally contributed by Nils Naumann <naumann@unileoben.ac.at>
;; Modified by Dirk Eddelbuettel <edd@debian.org>
;; Adapted for dh-make by Jim Van Zandt <jrv@debian.org>

;; The la-storia-in-giallo package follows the Debian/GNU Linux 'emacsen' policy and
;; byte-compiles its elisp files for each 'emacs flavor' (emacs19,
;; xemacs19, emacs20, xemacs20...).  The compiled code is then
;; installed in a subdirectory of the respective site-lisp directory.
;; We have to add this to the load-path:
(let ((package-dir (concat "/usr/share/"
                           (symbol-name flavor)
                           "/site-lisp/la-storia-in-giallo")))
;; If package-dir does not exist, the la-storia-in-giallo package must have
;; removed but not purged, and we should skip the setup.
  (when (file-directory-p package-dir)
    (setq load-path (cons package-dir load-path))
    (autoload 'la-storia-in-giallo-mode "la-storia-in-giallo-mode"
      "Major mode for editing la-storia-in-giallo files." t)
    (add-to-list 'auto-mode-alist '("\\.la-storia-in-giallo$" . la-storia-in-giallo-mode))))

