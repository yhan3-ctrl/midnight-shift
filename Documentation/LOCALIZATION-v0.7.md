# Seven-language revision / v0.7

The user clarified that “additional languages” meant a selection of commonly used languages, rather than English with only Chinese as the alternative. Implemented English (default), Simplified Chinese, Spanish, French, German, Japanese and Korean.

## Player-facing behaviour

- Title and pause share a Language selector. Every option uses its native name, and Japanese/Korean/Chinese names use the appropriate bundled font even when another language is active.
- L opens the selector from title/pause. Arrows move selection, Enter confirms, Esc returns. A confirmed choice is saved; fresh/unsupported preferences use English. Switching is separate from story/circuit/progression state.
- The 263-entry catalog covers menus, tutorial, formal chapter checklists, action/reason guidance, prompts, core-location descriptions, dialogue, ending and recovery text. Brand/title and physical factory signs remain English. Keyboard letters and circuit IN/OUT stay stable across languages.
- Text uses measured wrapping, limited size adjustment and concise translations. Long dialogue paginates while retaining its underlying line; Continue/Space/Enter moves through pages first. A new conversation resets the page.
- Two open-license Noto medium font subsets add about 210 KB; the catalog is about 124 KB. All automatic review launches remain silent. No external translation service or upload was used.

## Executed verification

- 105 Unity scene integration checks passed, including the animated ending. New checks exercise actual localized guide/story output without changing progress, complete seven-column resources, all catalog glyphs in the bundled language font, and unsupported-language fallback.
- Standard-library catalog audit passed: 263 entries × 7 nonempty translations, unique English keys and literal guide/UI coverage.
- A completed staged capture produced 105 screens across seven languages. It included title, picker, actual staff note, artificially doubled-note pagination stress, guide, notebook, menu, both circuit sizes, Packaging/Archive/Assembly/Delivery calendars and ending. In all seven stress cases, advancing the first page retained the same source line. This is rendering/pagination evidence, not a human playthrough.
- Initial Spanish/French/German overflows were corrected through shorter copy and wider control labels. The completed follow-up logged one remaining English delivery-calendar heading overflow; its heading was then shortened to “05 / DELIVERY”. Inspected Japanese title, Korean picker, German title and French dialogue/pagination renders, among others.
- Final package built and passed startup smoke, ZIP integrity and strict deep signature verification after extraction. SHA-256: `823a6a15355cd0323f6dbf39e93f22969c66ea3d3e8a419ff396a3b0a40f4d73`.

## Limitations

The Mac became locked before the final native keyboard/persistence review. Computer-use tooling reported it could not unlock the Mac, and the final render rerun waited at its first frame. That specific preview process was stopped. The earlier completed 105-screen pass remains the visual evidence; no completed zero-warning final rerun or native restart/persistence test is claimed. The final change after that pass was the short English heading and a whitespace-aware page-break refinement.

Translations are implementation drafts and have not received professional native-speaker proofreading. Visual completeness does not prove language-specific first-time comprehension, emotional tone, fun or duration. The English-default behaviour and saved choice are implemented; final native selection/restart verification remains pending while locked.
