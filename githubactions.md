### GitHub Actions CI/CD Agent Instructions

**Workflow Structure & Execution**
* **Granular Triggers:** Use descriptive workflow names and explicit, granular triggers (e.g., path or branch-specific `push` or `pull_request` events).
* **Modular Jobs:** Define distinct, independent jobs. Control execution flow and dependencies using `needs`, `outputs`, and `if` conditions.
* **Artifact Passing:** Transfer build outputs between jobs or workflows securely using `actions/upload-artifact` and `actions/download-artifact`.

**Security & Permissions (Critical)**
* **Immutable Actions:** Always pin third-party actions to full, immutable commit SHAs (e.g., `@34e11487... # v4.3.1`) to prevent supply chain attacks. Never use mutable tags (like `@v4`) or branches.
* **Least Privilege:** Set the `GITHUB_TOKEN` default permissions to `contents: read` at the workflow level, elevating access only for specific jobs that require it.
* **Authentication:** Mandate OpenID Connect (OIDC) for authenticating with cloud providers (AWS, Azure, GCP) to avoid storing long-lived, static credentials.
* **Secret Management:** Manage all sensitive data exclusively through GitHub Secrets or Environment Secrets. Mask secrets and never hardcode them.

**Performance & Optimization**
* **Shallow Checkouts:** Default to `fetch-depth: 1` when using `actions/checkout` to save time and bandwidth, unless full history is explicitly required.
* **Dependency Caching:** Accelerate workflows using `actions/cache` with `hashFiles` keys mapped to dependency lock files (e.g., `package-lock.json`).
* **Parallelization:** Utilize `strategy.matrix` to run tests and builds concurrently across different operating systems, environments, or language versions.

**Quality Assurance & Deployment**
* **Testing Pipeline:** Enforce a comprehensive testing hierarchy (Unit → Integration → E2E). Provision temporary external dependencies (like databases or message queues) using workflow `services`.
* **Security Scanning:** Integrate Dependency Review (SCA) and Static Application Security Testing (SAST) tools into the CI pipeline to block vulnerable PRs.
* **Deployment Gates:** Guard production deployments using GitHub Environments with required manual approvals, strict branch protections, and documented rollback mechanisms. Deploy to staging environments that strictly mirror production first.