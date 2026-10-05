# Phone app

Not created yet. Phase 0 in `docs/PROMPTS.md` generates it here:

```bash
cd apps/phone
flutter create . --project-name phone_controller --org com.kavd.phonecontroller --platforms android,ios
```

Stack is fixed: Flutter stable, Dart 3. See `docs/adr/0001-flutter-for-the-phone.md`. The Dart codec must match `tests/protocol` byte for byte.
