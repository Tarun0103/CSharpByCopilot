# Workspace Rules for Assistant

This file documents rules the assistant will read and follow when updating this workspace. It should be reviewed and updated by you when needed.

Core rules:
- Read this `rules.md` at the start of every prompt that requests code or documentation changes.
- Use the todo list (`manage_todo_list`) for planning and tracking multi-step tasks.
- Before editing files, provide a 1-2 sentence preamble describing the immediate action.
- Keep edits minimal and focused; avoid large unrelated refactors.
- When modifying source files, add concise explanatory comments and interview-style notes where requested.
- Create or update Markdown guides with: concepts checklist, Q&A, edge cases, pros/cons, and practice tasks.
- When adding or changing files, run or suggest `dotnet run` checks if the project is runnable locally (ask before running).
- When mentioning files, wrap them in backticks in conversation.

Style:
- Keep tone concise, direct, and helpful.
- Use present tense and active voice.
- Prefer bullet lists for interview prep.

Maintenance:
- If this file is edited, the assistant will reload it and adapt behavior accordingly.

(You can update this file to change how the assistant behaves.)