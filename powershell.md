# PowerShell Coding Standards and Best Practices (Agent Instructions)

When generating, modifying, or analyzing PowerShell scripts, you must strictly adhere strictly to the following best practices and coding standards.

## 1. Naming Conventions & Style
*   **Verb-Noun Format**: Always use standard, approved PowerShell verbs (e.g., `Get`, `Set`, `New`, `Invoke`, `Remove`). Use `Get-Verb` to verify.
*   **PascalCase**: Use PascalCase for function names, parameters, and variables (e.g., `$ComputerName`, `Get-SystemInfo`).
*   **Avoid Aliases in Scripts**: Never use aliases (e.g., `?`, `%`, `cd`, `dir`, `wget`, `curl`, `iex`) in production scripts. Always use the full cmdlet names (e.g., `Where-Object`, `ForEach-Object`, `Set-Location`) for readability and maintainability.

## 2. Error Handling
*   **Terminating Errors**: Always handle errors explicitly. Set `$ErrorActionPreference = 'Stop'` at the top of your scripts to turn non-terminating errors into terminating errors that can be caught.
*   **Try/Catch/Finally**: Wrap external operations (file I/O, network requests, registry changes, Active Directory queries) in `try/catch` blocks.
*   **Example**:
    ```powershell
    $ErrorActionPreference = 'Stop'
    try {
        $data = Get-Content -Path "C:\ImportantFile.txt"
    } catch {
        Write-Error "Failed to read file. Exception: $_"
    }
    ```

## 3. Output and Messaging
*   **Avoid Write-Host**: Do not use `Write-Host` unless you are explicitly building a console UI. `Write-Host` writes directly to the console and bypasses the output stream (in older PS versions), making automation difficult.
*   **Use Proper Streams**: 
    *   `Write-Output`: For returning actual data down the pipeline.
    *   `Write-Verbose`: For detailed debugging and tracing information.
    *   `Write-Warning`: For recoverable issues or deprecation notices.
    *   `Write-Error`: For non-terminating errors.

## 4. Performance & Efficiency
*   **Filter Left, Format Right**: Apply filters (like `Where-Object` or `-Filter` parameters) as early in the pipeline as possible to minimize the amount of data passed down the chain.
*   **Pipelines vs. Foreach**: For small to medium datasets, the pipeline (`| ForEach-Object`) is idiomatic and readable. For large datasets or performance-critical sections, use the `foreach ($item in $collection)` statement, as it performs significantly faster.

## 5. Script Structure & Parameters
*   **Advanced Functions**: Use `[CmdletBinding()]` for all functions. This automatically provides common parameters like `-Verbose`, `-Debug`, and `-ErrorAction`.
*   **Strong Typing**: Strongly type your parameters and cast variables where appropriate (e.g., `[string]`, `[int]`, `[switch]`).
*   **Mandatory Parameters**: Explicitly define mandatory parameters rather than relying on positional arguments.
*   **Example**:
    ```powershell
    function Get-CustomData {
        [CmdletBinding()]
        param (
            [Parameter(Mandatory=$true, HelpMessage="Enter the target directory path.")]
            [ValidateScript({Test-Path $_ -PathType Container})]
            [string]$Path,

            [Parameter(Mandatory=$false)]
            [switch]$Recurse
        )
        
        Write-Verbose "Starting data collection in $Path"
        # Implementation...
    }
    ```

## 6. Security
*   **Execution Policy**: Write scripts that do not require unsafe Execution Policy modifications.
*   **Credential Handling**: NEVER hardcode passwords or secrets. Use `Get-Credential`, `[SecureString]`, or integrate with secret management modules (e.g., `SecretManagement`).
*   **Injection Risks**: Avoid `Invoke-Expression` (`iex`) at all costs, as it is a major vector for script injection attacks. Use the call operator `&` or `Invoke-Command` instead.

## 7. Configuration and Command-Line Arguments
*   **Configuration Files (PSD1 / JSON)**: Prefer `.psd1` (PowerShell Data Files) or `.json` for external configuration. Never dot-source `.ps1` scripts to load variables, as this executes code and poses a security risk. Use `Import-PowerShellDataFile` (PSv5+) to safely parse `.psd1` files without execution.
*   **Splatting**: When passing multiple arguments to cmdlets or functions, use splatting (creating a hashtable of parameters and passing it with `@`) instead of chaining long commands with the backtick (`` ` ``) line continuation character.
*   **Parameter Validation**: Utilize PowerShell's built-in validation attributes (`[ValidateSet()]`, `[ValidateRange()]`, `[ValidateScript()]`, `[ValidateNotNullOrEmpty()]`) within your `param()` block to enforce constraints at the parameter binding stage, reducing the need for manual `if/throw` validation logic inside the function.
*   **Pipeline Support**: Use `ValueFromPipeline` or `ValueFromPipelineByPropertyName` in parameter attributes to allow your functions to accept input directly from the pipeline, mimicking native cmdlet behavior.
*   **Environment Variables**: Read and manage environment configurations idiomatically using the `$env:` provider drive (e.g., `$env:Path`).

*   **Example**:
    ```powershell
    [CmdletBinding()]
    param (
        [Parameter(Mandatory=$true)]
        [ValidateSet('Dev', 'Staging', 'Prod')]
        [string]$Environment,

        [Parameter(ValueFromPipelineByPropertyName=$true)]
        [string]$ServerName = $env:COMPUTERNAME
    )

    process {
        # Idiomatic Configuration Loading
        $configPath = ".\config\$Environment.psd1"
        if (Test-Path $configPath) {
            $config = Import-PowerShellDataFile -Path $configPath
        } else {
            Write-Error "Configuration for $Environment not found."
            return
        }

        # Idiomatic Argument Passing (Splatting)
        $requestArgs = @{
            Uri         = $config.ApiEndpoint
            Method      = 'Post'
            ContentType = 'application/json'
            Body        = (@{ Server = $ServerName } | ConvertTo-Json)
            ErrorAction = 'Stop'
        }
        
        try {
            $response = Invoke-RestMethod @requestArgs
            Write-Output $response
        } catch {
            Write-Error "Failed to invoke API: $_"
        }
    }
    ```

