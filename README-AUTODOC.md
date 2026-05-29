AI Auto-Generated Documentation (Overview)

How it works
- On push to `master`, GitHub Actions builds the project and emits `swagger.json`.
- A Python script (`scripts/generate_docs.py`) converts the Swagger file into Markdown.
- If `OPENAI_API_KEY` is configured in repository secrets, the script calls the OpenAI API to enhance the docs.

Setup
1. Add `OPENAI_API_KEY` to repository secrets.
2. The workflow is defined in `.github/workflows/generate-docs.yml`.

Notes
- The workflow commits generated files back to the repository (`docs/` directory). Adjust as needed to publish to `gh-pages` instead.
