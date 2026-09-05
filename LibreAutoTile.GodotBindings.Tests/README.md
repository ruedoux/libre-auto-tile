# LibreAutoTile Godot Bindings Tests

Tests for the Godot bindings, using the `Qwaitumin.SimpleTest` framework.

## Running

```sh
./run-tests.sh
```

You can filter to a single test class or method:

```sh
./run-tests.sh --test-class=ClassName --test-method=MethodName
```

The script imports the Godot project headlessly, builds the test project, and runs it.
