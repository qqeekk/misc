# package: zsh
# default shell: chsh -s $(which zsh)
# oh-my-zsh install: sh -c "$(curl -fsSL https://raw.githubusercontent.com/ohmyzsh/ohmyzsh/master/tools/install.sh)" 
# zsh-autosuggestions: git clone https://github.com/zsh-users/zsh-autosuggestions ${ZSH_CUSTOM:-~/.oh-my-zsh/custom}/plugins/zsh-autosuggestions

# Path to your oh-my-zsh installation.
export ZSH="/home/ivan/.oh-my-zsh"

# See https://github.com/ohmyzsh/ohmyzsh/wiki/Themes
ZSH_THEME="robbyrussell"
HIST_STAMPS="yyyy-mm-dd"

source $ZSH/oh-my-zsh.sh
plugins=(git git-flow vscode zsh-autosuggestions)
source ~/.oh-my-zsh/custom/plugins/zsh-autosuggestions/zsh-autosuggestions.zsh

# Kubernetes
[[ /usr/local/bin/kubectl ]] && source <(kubectl completion zsh)

PROMPT='%(?:%{%}➜ :%{%}➜ ) %{$fg[cyan]%}%c%{$reset_color%} $(git_prompt_info)$ '
export EDITOR='vim'
export PATH="${KREW_ROOT:-$HOME/.krew}/bin:$PATH"

function git_prompt_info() {
  ref=$(git symbolic-ref HEAD 2> /dev/null) || return
  echo "$ZSH_THEME_GIT_PROMPT_PREFIX${ref#refs/heads/}${ZSH_THEME_GIT_PROMPT_CLEAN}$ZSH_THEME_GIT_PROMPT_SUFFIX"
}

