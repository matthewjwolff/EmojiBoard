# This is an example PKGBUILD file. Use this as a start to creating your own,
# and remove these comments. For more information, see 'man PKGBUILD'.
# NOTE: Please fill out the license field for your package! If it is unknown,
# then please put 'unknown'.

# The following guidelines are specific to BZR, GIT, HG and SVN packages.
# Other VCS sources are not natively supported by makepkg yet.

# Maintainer: Matthew Wolff <matthewjwolff@gmail.com>
pkgname=emojiboard-git
pkgver=0
pkgrel=1
pkgdesc="Helper for entering emoji"
arch=('any')
url="https://github.com/matthewjwolff/EmojiBoard"
license=('GPL3')
depends=('libuiohook' 'mono' 'gtk-sharp-2' 'notify-sharp')
makedepends=('git' 'msbuild' 'nuget')
source=('emojiboard-git::git+https://github.com/matthewjwolff/EmojiBoard')
md5sums=('SKIP')

prepare() {
    cd "$srcdir/${pkgname%-VCS}"
    git submodule init
    git submodule update
    nuget restore
}

build() {
	cd "$srcdir/${pkgname%-VCS}"
	msbuild /p:Configuration=Release
}

package() {
    mkdir -p "$pkgdir/usr/lib"
    cp -R "$srcdir/${pkgname%-VCS}/EmojiBoard/bin/Release/" "$pkgdir/usr/lib/emojiboard"
    mkdir -p "$pkgdir/usr/bin"
    echo "cd /usr/lib/emojiboard; mono EmojiBoard.exe" >> "$pkgdir/usr/bin/emojiboard"
    chmod a+x "$pkgdir/usr/bin/emojiboard"
}
