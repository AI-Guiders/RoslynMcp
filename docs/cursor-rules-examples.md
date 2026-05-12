# Cursor: примеры правил (копипаст)

Те же тексты, что в [.cursor/rules](.cursor/rules) этого репозитория — удобно копировать отсюда в `.cursor/rules/*.mdc` корня workspace.

### `.cursor/rules/use-get-diagnostics.mdc`

~~~mdc
---
description: Проверка C# кода — вызывать roslyn_get_diagnostics (этот MCP), не только ReadLints
globs: "**/*.cs"
alwaysApply: false
---

# Использовать roslyn_get_diagnostics для проверки кода

При поиске проблем в C# (неиспользуемые переменные, предупреждения компилятора, анализаторы) **вызывать инструмент этого сервера `roslyn_get_diagnostics`** с `solution_or_project_path` (и при необходимости `file_path`), а не полагаться только на ReadLints или ручной просмотр.

Ответ даёт полный список диагностик (file:line:column, severity, id, message), в том числе CS0219 (unused variable). Исправлять по file:line:column через roslyn_get_code_actions и roslyn_apply_code_action.
~~~

### `.cursor/rules/sync-namespaces-after-rename.mdc`

~~~mdc
---
description: После переименования/перемещения папок в C# — вызвать roslyn_sync_namespaces
globs: "**/*.cs"
alwaysApply: false
---

# Синхронизация namespace со структурой папок

Если переименовываешь или перемещаешь папки в C# проекте, **вызови инструмент этого сервера `roslyn_sync_namespaces`**, чтобы объявления namespace совпадали со структурой папок (RootNamespace + путь).

- Сначала вызови с **dry_run: true** — получишь отчёт, какие файлы и namespace будут изменены.
- Затем вызови **без dry_run** — применит изменения и обновит using в остальных файлах.

Параметры: solution_or_project_path (обязательно), опционально project_path (для solution — только один проект), dry_run.
~~~
