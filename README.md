 # Agents
 
 This repository stores my `$HOME/.agents` folder for AI agent use.
 
 The agent files should be kept in the repository's `.agents/` directory.

## Clone into `$HOME/.agents`

```bash
# Optional: back up any existing ~/.agents directory
mv "$HOME/.agents" "$HOME/.agents.backup.$(date +%Y%m%d%H%M%S)"

# Clone this repository directly into ~/.agents
git clone -b main https://github.com/ByronScottJones/Agents.git "$HOME/.agents"
```

If you already cloned the repo elsewhere, you can link it instead:

```bash
ln -s /path/to/Agents "$HOME/.agents"
```
