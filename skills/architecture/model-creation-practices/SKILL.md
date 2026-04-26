---
name: model-creation-practices
description: Enforce repository-specific model creation rules for DAL and shared C# models. Use when creating or editing entities, persistence models, shared models, or related structures that must follow required-property and no-default-value conventions.
---

# Model Creation Practices

## Overview

Use this skill when creating or editing models so they follow the repository's shared model rules consistently.

Read [references/rules.md](references/rules.md) before adding or changing models.

## Core Rules

- Use `get; set;` properties only.
- Do not use `init`.
- Use `required` on mandatory fields when applicable.
- Do not assign default property values.
- Use `List<T>` for entity collections.
- Do not add marker interfaces like `IHasId<Guid>` just to expose an `Id`.

## Review Checklist

- Reject model properties that use `init`.
- Reject mandatory properties that should be `required` but are not.
- Reject default property values on model properties.
- Reject collection properties that use `IEnumerable<T>` or `IReadOnlyCollection<T>` in entities.
- Reject unnecessary marker interfaces added only for shared shape conventions.
