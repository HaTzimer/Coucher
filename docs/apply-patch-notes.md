## `apply_patch` notes

`*** Move to:` is the documented rename syntax, but this environment can still fail while parsing move patches.

Safe fallback:

```text
*** Begin Patch
*** Add File: path\to\new-file.cs
...
*** Delete File: path\to\old-file.cs
*** End Patch
```

Use that pattern when a move patch fails during ACP initialization.
