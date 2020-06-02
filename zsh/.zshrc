# package: zsh
# default shell: chsh -s $(which zsh)
# oh-my-zsh install: sh -c "$(curl -fsSL https://raw.githubusercontent.com/ohmyzsh/ohmyzsh/master/tools/install.sh)" 

# Path to your oh-my-zsh installation.
export ZSH="/home/ivan/.oh-my-zsh"

# See https://github.com/ohmyzsh/ohmyzsh/wiki/Themes
ZSH_THEME="robbyrussell"

HIST_STAMPS="yyyy-mm-dd"

plugins=(git git-flow vscode)

source $ZSH/oh-my-zsh.sh

export EDITOR='vim'

