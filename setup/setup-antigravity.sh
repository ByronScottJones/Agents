#!/usr/bin/env bash
# Configures Google Antigravity (~/.gemini) to load shared agent guidance
# and skills from ~/.agents on startup.

set -euo pipefail

die() {
	printf 'Error: %s\n' "$1" >&2
	exit 1
}

resolve_agents_dir() {
	local script_dir repo_dir

	if [[ -d "${HOME}/.agents" ]]; then
		printf '%s\n' "${HOME}/.agents"
		return 0
	fi

	script_dir=$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")" && pwd)
	repo_dir=$(cd -- "${script_dir}/.." && pwd)
	if [[ -f "${repo_dir}/AGENTS.md" ]]; then
		printf '%s\n' "${repo_dir}"
		return 0
	fi

	die "Shared agents directory not found at ${HOME}/.agents."
}

ensure_symlink() {
	local target="$1"
	local link_path="$2"
	local current_target

	mkdir -p "$(dirname -- "${link_path}")"

	if [[ -L "${link_path}" ]]; then
		current_target=$(readlink "${link_path}")
		if [[ "${current_target}" == "${target}" ]]; then
			printf 'Already linked: %s -> %s\n' "${link_path}" "${target}"
			return 0
		fi
		die "Symlink already exists with different target: ${link_path} -> ${current_target} (expected ${target})"
	fi

	if [[ -e "${link_path}" ]]; then
		die "Path already exists and is not a symlink: ${link_path}"
	fi

	ln -s "${target}" "${link_path}"
	printf 'Created: %s -> %s\n' "${link_path}" "${target}"
}

main() {
	local agents_dir gemini_dir gemini_config_dir gemini_agents_link

	agents_dir=$(resolve_agents_dir)
	[[ -f "${agents_dir}/AGENTS.md" ]] || die "Missing ${agents_dir}/AGENTS.md"
	[[ -d "${agents_dir}/skills" ]] || die "Missing ${agents_dir}/skills"

	gemini_dir="${HOME}/.gemini"
	gemini_config_dir="${gemini_dir}/config"
	gemini_agents_link="${gemini_dir}/agents"

	mkdir -p "${gemini_dir}" "${gemini_config_dir}"

	# 1. Link shared ~/.agents repository into ~/.gemini/agents
	ensure_symlink "${agents_dir}" "${gemini_agents_link}"

	# 2. Link AGENTS.md into global startup rule locations
	ensure_symlink "${gemini_agents_link}/AGENTS.md" "${gemini_dir}/AGENTS.md"
	ensure_symlink "${gemini_agents_link}/AGENTS.md" "${gemini_config_dir}/AGENTS.md"

	# 3. Link shared skills directory into ~/.gemini/config/skills
	ensure_symlink "${gemini_agents_link}/skills" "${gemini_config_dir}/skills"
}

main "$@"
