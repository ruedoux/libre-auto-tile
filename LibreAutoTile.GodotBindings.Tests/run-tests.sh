#!/bin/bash

# Generate .godot/ (UID cache, etc.) so UID references like the main scene
# resolve in a fresh checkout / CI where .godot/ isn't committed.
godot --path=. --headless --import

for arg in "$@"; do
  case $arg in
    --test-method=*)
      TEST_METHOD="--test-method=${arg#*=}"
      ;;
    --test-class=*)
      TEST_CLASS="--test-class=${arg#*=}"
      ;;
  esac
done

godot --path=. --headless --script=src/Run.cs $TEST_CLASS $TEST_METHOD
