Directory for tests.

1. On assembly-initialize "~/Test-resources/Root" will be created (if it already exists it will be deleted first).
2. For each test "~/Test-resources/Root/{NEW-GUID}" will be created.
3. On assembly-cleanup "~/Test-resources/Root" will be deleted.

Note: "~/Test-resources/Root" will also be deleted when building the project.
- [Target "TestCleanup"](/Source/Integration-tests/Build/Build.targets#L2)