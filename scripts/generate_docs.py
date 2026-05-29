import argparse
import json
import os
from datetime import datetime
from openai import OpenAI


def load_swagger(path):
    with open(path, 'r', encoding='utf-8') as f:
        return json.load(f)


def render_markdown(swagger):
    title = swagger.get('info', {}).get('title', 'API')
    version = swagger.get('info', {}).get('version', '')
    md = [f"# {title} {version}\n", f"_Generated: {datetime.utcnow().isoformat()}Z_\n\n"]

    paths = swagger.get('paths', {})
    for path, methods in paths.items():
        for method, meta in methods.items():
            summary = meta.get('summary') or meta.get('description') or ''
            md.append(f"## {method.upper()} {path}\n\n")
            if summary:
                md.append(f"{summary}\n\n")
            params = meta.get('parameters', [])
            if params:
                md.append("**Parameters**:\n\n")
                for p in params:
                    md.append(f"- `{p.get('name')}` ({p.get('in')}): {p.get('description', '')}\n")
                md.append('\n')
            responses = meta.get('responses', {})
            if responses:
                md.append("**Responses**:\n\n")
                for code, resp in responses.items():
                    md.append(f"- **{code}**: {resp.get('description','') }\n")
                md.append('\n')

            md.append('---\n\n')

    return ''.join(md)


def ai_enhance(markdown):
    # If OPENAI_API_KEY is available, ask the model to enhance the docs.
    key = os.environ.get('OPENAI_API_KEY')
    if not key:
        return markdown

    try:
        client = OpenAI(api_key=key)
        prompt = (
            "You are a helpful assistant that converts terse API reference into readable developer documentation.\n"
            "Improve and expand the following API reference, adding short examples and clarifying descriptions where helpful:\n\n" + markdown
        )

        resp = client.chat.completions.create(
            model="gpt-4o-mini",
            messages=[{"role": "user", "content": prompt}],
            max_tokens=1500,
            temperature=0.2,
        )

        return resp.choices[0].message.content
    except Exception as e:
        print(f"Warning: AI enhancement failed ({type(e).__name__}: {str(e)}). Using basic markdown instead.")
        return markdown


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument('--swagger', required=True)
    parser.add_argument('--out', required=True)
    args = parser.parse_args()

    swagger = load_swagger(args.swagger)
    base_md = render_markdown(swagger)
    enhanced = ai_enhance(base_md)

    os.makedirs(args.out, exist_ok=True)
    out_file = os.path.join(args.out, 'API_DOCUMENTATION.md')
    with open(out_file, 'w', encoding='utf-8') as f:
        f.write(enhanced)

    print(f'Wrote: {out_file}')


if __name__ == '__main__':
    main()
