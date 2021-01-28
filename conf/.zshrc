# Package: zsh
# Default shell: chsh -s $(which zsh)
# oh-my-zsh install: sh -c "$(curl -fsSL https://raw.githubusercontent.com/ohmyzsh/ohmyzsh/master/tools/install.sh)" 
# zsh-autosuggestions: git clone https://github.com/zsh-users/zsh-autosuggestions ${ZSH_CUSTOM:-~/.oh-my-zsh/custom}/plugins/zsh-autosuggestions

# Path to your oh-my-zsh installation.
export ZSH="/home/ivan/.oh-my-zsh"

# See https://github.com/ohmyzsh/ohmyzsh/wiki/Themes .
ZSH_THEME="robbyrussell"
HIST_STAMPS="yyyy-mm-dd"

source $ZSH/oh-my-zsh.sh
plugins=(git git-flow vscode zsh-autosuggestions)
source ~/.oh-my-zsh/custom/plugins/zsh-autosuggestions/zsh-autosuggestions.zsh

# Kubernetes.
[[ -f /usr/local/bin/kubectl ]] && source <(kubectl completion zsh)

# Fishy like prompt.
local user_color='green'; [ $UID -eq 0 ] && user_color='red'
PROMPT='%n@%m %{$fg[$user_color]%}$(_fishy_collapsed_wd)%{$reset_color%}%(!.#.>) '
PROMPT2='%{$fg[red]%}\ %{$reset_color%}'
local return_status="%{$fg_bold[red]%}%(?..%?)%{$reset_color%}"
RPROMPT="${RPROMPT}"'${return_status}$(_git_prompt_info)%{$reset_color%}'

export EDITOR='vim'
export PATH="${KREW_ROOT:-$HOME/.krew}/bin:$PATH"

## FUNCTIONS ##

# More optimizied version of "git_prompt_info", it doesn't call "git status".
# Source: https://gist.github.com/msabramo/2355834 .
function _git_prompt_info() {
  ref=$(git symbolic-ref HEAD 2> /dev/null) || return
  echo "$ZSH_THEME_GIT_PROMPT_PREFIX${ref#refs/heads/}$ZSH_THEME_GIT_PROMPT_SUFFIX"
}

# Fishy like path prompt.
# Source: https://github.com/ohmyzsh/ohmyzsh/blob/master/themes/fishy.zsh-theme .
function _fishy_collapsed_wd() {
  local i pwd
  pwd=("${(s:/:)PWD/#$HOME/~}")
  if (( $#pwd > 1 )); then
    for i in {1..$(($#pwd-1))}; do
      if [[ "$pwd[$i]" = .* ]]; then
        pwd[$i]="${${pwd[$i]}[1,2]}"
      else
        pwd[$i]="${${pwd[$i]}[1]}"
      fi
    done
  fi
  echo "${(j:/:)pwd}"
}

