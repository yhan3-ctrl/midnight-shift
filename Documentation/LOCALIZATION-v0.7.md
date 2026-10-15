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

The earlier lock-screen interruption was resolved when the user unlocked the Mac. The same release then completed its final staged pass: 105 screenshots across all seven languages, seven successful long-dialogue continuations, and no UI_LAYOUT_OVERFLOW or game-runtime exception entries in Work/languages-v07-final.log.

Native agent-operated keyboard review also completed:

1. Observed English title, opened Language with L, selected Japanese with arrows/Enter.
2. Quit using Cmd Q (process exit 0), relaunched, and observed Japanese retained on the title.
3. Began the Japanese introduction; opened Tab menu and L selector during its first dialogue line; changed to German and resumed. The same opening line remained, translated into German, with the initial checklist unchanged.
4. Restored English from the paused dialogue and resumed the same line in English. Tab/X quit exited with code 0.
5. Relaunched once more and observed English retained. Closed this silent review instance with Cmd Q (exit 0), so the user's next ordinary launch is not a muted agent session.

Raw native logs: Work/native-language-save.log, native-language-reopen.log and native-language-english-restored.log. These contain no reported UI_LAYOUT_OVERFLOW or game-runtime exceptions. Screenshots were inspected through the native app tool; they are agent-operated observations, not independent human playtest feedback. Actual restart persistence was exercised for Japanese and restored English, not separately for every language. Mouse-button activation was not validated in this keyboard review.

Translations are implementation drafts and have not received professional native-speaker proofreading. Visual completeness does not prove language-specific first-time comprehension, emotional tone, fun or duration. English default, selection, dialogue preservation and Japanese/English restart persistence now have the native evidence above.
