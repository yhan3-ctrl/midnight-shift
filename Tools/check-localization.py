#!/usr/bin/env python3
"""Check catalog completeness and literal guide/UI coverage using the standard library."""
import json
import pathlib
import re
root = pathlib.Path(__file__).resolve().parent.parent
entries = json.loads((root / 'Assets/Resources/I18n/catalog.json').read_text())['entries']
by_key = {}
for entry in entries:
    key, values = entry['en'], entry['values']
    assert key not in by_key, f'Duplicate English key: {key}'
    assert len(values) == 7 and all(value.strip() for value in values), key
    assert values[0] == key, key
    by_key[key] = entry
literal = r'"(?:[^"\\]|\\.)*"'
missing = set()
for name in ['ShiftGuide.cs', 'ShiftInterface.cs', 'CircuitView.cs', 'PlayerMotor.cs', 'FactoryMap.cs']:
    source = (root / 'Assets/Scripts' / name).read_text()
    for match in re.finditer('(' + literal + r')\s*,\s*(' + literal + ')', source):
        chinese, english = map(json.loads, match.groups())
        if any('\u3400' <= char <= '\u9fff' for char in chinese) and not any('\u3400' <= char <= '\u9fff' for char in english):
            if english not in by_key:
                missing.add((name, english))
assert not missing, f'Missing UI/guide translations: {sorted(missing)}'
print(f'PASS: {len(entries)} complete entries × 7 languages; literal UI/guide coverage checked.')
