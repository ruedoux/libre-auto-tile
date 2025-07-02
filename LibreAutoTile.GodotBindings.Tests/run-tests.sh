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

godot --path=. --headless --build-solutions --quit
godot --path=. --headless --script=src/Run.cs $TEST_CLASS $TEST_METHOD