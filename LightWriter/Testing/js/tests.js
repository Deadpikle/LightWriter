module("Row and Column Finder Functions");
test("gridEqual() Tester", function () {
    var expected = ["E"];
    var actual = gridEqual("E");
    deepEqual(actual, expected, "Column test");

    expected = [2];
    actual = gridEqual("2");
    deepEqual(actual, expected, "Row test");
});

test("gridLess() Tester", function () {
    var expected = ["A", "B", "C"];
    var actual = gridLess("D");
    deepEqual(actual, expected, "Multiple columns");

    expected = ["A"];
    actual = gridLess("B");
    deepEqual(actual, expected, "Single column");

    expected = [1];
    actual = gridLess("2");
    deepEqual(actual, expected, "Single row");

    expected = [];
    actual = gridLess("A");
    deepEqual(actual, expected, "Empty result");
});

test("gridLessEqual() Tester", function () {
    var expected = ["A", "B", "C", "D"];
    var actual = gridLessEqual("D");
    deepEqual(actual, expected, "Multiple columns");

    expected = ["A", "B"];
    actual = gridLessEqual("B");
    deepEqual(actual, expected, "Multiple columns 2");

    expected = ["A"];
    actual = gridLessEqual("A");
    deepEqual(actual, expected, "Single column");

    expected = [1];
    actual = gridLessEqual("1");
    deepEqual(actual, expected, "Single row");


    expected = [1, 2, 3];
    actual = gridLessEqual("3");
    deepEqual(actual, expected, "Multiple rows");
});

// the greater tests will have to be rewritten after the grid expands
test("gridGreater() Tester", function () {
    var expected = ['P'];
    var actual = gridGreater("O");
    deepEqual(actual, expected, "Single column");

    expected = [16];
    actual = gridGreater("15");
    deepEqual(actual, expected, "Single row");

    expected = ['B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P'];
    actual = gridGreater("A");
    deepEqual(actual, expected, "Multiple columns");

    expected = [];
    actual = gridGreater("16");
    deepEqual(actual, expected, "Empty set");
});

test("gridGreaterEqual() Tester", function () {
    var expected = ["P"];
    var actual = gridGreaterEqual("P");
    deepEqual(actual, expected, "Single column");

    expected = [16];
    actual = gridGreaterEqual("16");
    deepEqual(actual, expected, "Single row");

    expected = ['A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P'];
    actual = gridGreaterEqual("A");
    deepEqual(actual, expected, "Multiple columns");

    expected = [13, 14, 15, 16];
    actual = gridGreaterEqual("13");
    deepEqual(actual, expected, "Multiple rows");
});

module("Miscellaneous functions");
test("isValidColor() Tester", function () {
    var expected = true;
    var actual = isValidColor("#33ccff");
    deepEqual(actual, expected, "Color test 1");

    expected = true;
    actual = isValidColor("#33cEfF");
    deepEqual(actual, expected, "Color test 2");

    expected = true;
    actual = isValidColor("#AAAAAA");
    deepEqual(actual, expected, "Color test 3");

    expected = false;
    actual = isValidColor("#3");
    deepEqual(actual, expected, "Color test 4");

    expected = false;
    actual = isValidColor("#QQQQQQ");
    deepEqual(actual, expected, "Color test 5");

    expected = true;
    actual = isValidColor("#111111");
    deepEqual(actual, expected, "Color test 6");

    expected = false;
    actual = isValidColor("");
    deepEqual(actual, expected, "Color test 7");
});

test("asciiVal() Tester", function () {
    var expected = 100;
    var actual = asciiVal('d');
    deepEqual(actual, expected, "Ascii val test 1");

    expected = 89;
    actual = asciiVal('Y');
    deepEqual(actual, expected, "Ascii val test 2");

    expected = 96;
    actual = asciiVal('`');
    deepEqual(actual, expected, "Ascii val test 3");

    expected = 32;
    actual = asciiVal(' ');
    deepEqual(actual, expected, "Ascii val test 4");
});

test("prevAsciiVal() Tester", function () {
    var expected = '@';
    var actual = prevAsciiVal('A');
    deepEqual(actual, expected, "Prev ascii val test 1");

    expected = ' ';
    actual = prevAsciiVal('!');
    deepEqual(actual, expected, "Prev ascii val test 2");

    expected = '`';
    actual = prevAsciiVal('a');
    deepEqual(actual, expected, "Prev ascii val test 3");

    expected = '}';
    actual = prevAsciiVal('~');
    deepEqual(actual, expected, "Prev ascii val test 4");
});

test("nextAsciiVal() Tester", function () {
    var expected = 'e';
    var actual = nextAsciiVal('d');
    deepEqual(actual, expected, "Next ascii val test 1");

    expected = 'Z';
    actual = nextAsciiVal('Y');
    deepEqual(actual, expected, "Next ascii val test 2");

    expected = 'a';
    actual = nextAsciiVal('`');
    deepEqual(actual, expected, "Next ascii val test 3");

    expected = '~';
    actual = nextAsciiVal('}');
    deepEqual(actual, expected, "Next ascii val test 4");
});

test("asciiVal() Tester", function () {
    var expected = 46;
    var actual = asciiVal('.');
    deepEqual(actual, expected, "Ascii val test 1");

    expected = 63;
    actual = asciiVal('?');
    deepEqual(actual, expected, "Ascii val test 2");

    expected = 125;
    actual = asciiVal('}');
    deepEqual(actual, expected, "Ascii val test 3");

    expected = 32;
    actual = asciiVal(' ');
    deepEqual(actual, expected, "Ascii val test 4");
});

test("asciiToString(asciiVal)", function () {
    var expected = '.';
    var actual = asciiToString(46);
    deepEqual(actual, expected, "Ascii to string test 1");

    expected = '?';
    actual = asciiToString(63);
    deepEqual(actual, expected, "Ascii to string test 2");

    expected = '}';
    actual = asciiToString(125);
    deepEqual(actual, expected, "Ascii to string test 3");

    expected = ' ';
    actual = asciiToString(32);
    deepEqual(actual, expected, "Ascii to string test 4");
});

test("nextVal(rowOrColumnVal)", function () {
    var expected = 12;
    var actual = nextVal(11);
    deepEqual(actual, expected, "Getting next val after 11");

    expected = "P";
    actual = nextVal("O");
    deepEqual(actual, expected, "Getting next val after 'O'");
});

test("prevVal(rowOrColumnVal)", function () {
    var expected = 10;
    var actual = prevVal(11);
    deepEqual(actual, expected, "Getting prev val before 11");

    expected = "N";
    actual = prevVal("O");
    deepEqual(actual, expected, "Getting prev val before 'O'");
});

test("verifyVariableType(variable)", function () {
    var expected = "asdf";
    var actual = verifyVariableType("asdf");
    deepEqual(actual, expected, "Strings should stay a string");

    expected = 4;
    actual = verifyVariableType("4");
    deepEqual(actual, expected, "Int strings should be converted to a number");
});

test("Random Color Tester", function () {
    var actual = isValidColor(getRandomColor());
    deepEqual(actual, true, "Random color test 1");

    actual = isValidColor(getRandomColor());
    deepEqual(actual, true, "Random color test 2");

    actual = isValidColor(getRandomColor());
    deepEqual(actual, true, "Random color test 3");

    actual = isValidColor(getRandomColor());
    deepEqual(actual, true, "Random color test 4");
});

test("Intersect rows or columns Tester", function () {
    var arrayOne = ["A", "B", "D"];
    var arrayTwo = ["A", "D"];
    var arrayThree = ["2", "3", "4"];
    var arrayFour = [];
    var arrayFive = ["5"];
    var arraySix = ["3", "4"];

    var expected = ["A", "D"];
    var actual = intersectRowsOrColumns(arrayOne, arrayTwo);
    deepEqual(actual, expected, "Intersection arrays 1");

    expected = [];
    actual = intersectRowsOrColumns(arrayOne, arrayThree);
    deepEqual(actual, expected, "Intersection arrays 2");

    expected = [];
    actual = intersectRowsOrColumns(arrayThree, arrayFour);
    deepEqual(actual, expected, "Intersection arrays 3");

    expected = ["3", "4"];
    actual = intersectRowsOrColumns(arraySix, arrayThree);
    deepEqual(actual, expected, "Intersection arrays 4");

    expected = ["A", "D"];
    actual = intersectRowsOrColumns(arrayTwo, arrayOne);
    deepEqual(actual, expected, "Intersection arrays 5");

    expected = [];
    actual = intersectRowsOrColumns(arrayFive, arrayTwo);
    deepEqual(actual, expected, "Intersection arrays 6");

    expected = [];
    actual = intersectRowsOrColumns(arrayFive, arrayThree);
    deepEqual(actual, expected, "Intersection arrays 7");
});

test("sortClassList(listToSort)", function () {
    var listOne = [5, 2, 8, 9, 1];
    var listTwo = ["N", "A", "B", "R", "C"];

    var expected = [1, 2, 5, 8, 9];
    var actual = sortClassList(listOne);
    deepEqual(actual, expected, "sortClassList() with an int array");

    expected = ["A", "B", "C", "N", "R"];
    actual = sortClassList(listTwo);
    deepEqual(actual, expected, "sortClassList() with a string array");
});

test("getRandomColor()", function () {
    var randomColor = getRandomColor();
    ok(isValidColor(randomColor), "Verifying that getRandomColor returns valid colors 1 [random returned: " + randomColor + ']');
    randomColor = getRandomColor();
    ok(isValidColor(randomColor), "Verifying that getRandomColor returns valid colors 2 [random returned: " + randomColor + ']');
    randomColor = getRandomColor();
    ok(isValidColor(randomColor), "Verifying that getRandomColor returns valid colors 3 [random returned: " + randomColor + ']');
    randomColor = getRandomColor();
    ok(isValidColor(randomColor), "Verifying that getRandomColor returns valid colors 4 [random returned: " + randomColor + ']');
});

test("isVariableANumber()", function () {
    var a = 1;
    var b = 200;
    var c = "three hundred";
    var d = "4";
    var e = "cow";

    var actual = isVariableANumber(a);
    ok(actual, "Should be a number [1]");

    actual = isVariableANumber(b);
    ok(actual, "Should still be a number [200]");

    actual = isVariableANumber(d);
    ok(actual, "Should still be seen as a number even though it is a string number ['4']");

    var expected = false;
    actual = isVariableANumber(c);
    deepEqual(actual, expected, "Yeah, strings aren't numbers ['three hundred']");

    actual = isVariableANumber(e);
    deepEqual(actual, expected, "A cow isn't a number");
});

test("Grid row/column painter tester", function () {
    // grid initialization already happened since it is a global in index.js
    // make sure to reset grid before and after testing so other tests work!
    grid.resetGrid();
    setGridRowOrColumnToColor("A", "#33ccff");

    var expected = "#33ccff";
    var actual = grid.getColorAtLocation(0, 0);
    deepEqual(actual, expected, "Grid set color 1");

    actual = grid.getColorAtLocation(4, 0);
    deepEqual(actual, expected, "Grid set color 2");

    setGridRowOrColumnToColor("4", "#33ccff");
    actual = grid.getColorAtLocation(3, 0);
    deepEqual(actual, expected, "Grid set color 3");

    actual = grid.getColorAtLocation(3, 3);
    deepEqual(actual, expected, "Grid set color 4");
    // now test overwriting
    setGridRowOrColumnToColor("C", "#AAAAAA");
    expected = "#AAAAAA";
    actual = grid.getColorAtLocation(3, 2);
    deepEqual(actual, expected, "Grid set color 4");

    grid.resetGrid();
});

test("Grid Single Square painter tester", function () {
    // grid initialization already happened since it is a global in index.js
    // make sure to reset grid before and after testing so other tests work!
    grid.resetGrid();
    setGridSquareToColor("1", "A", "#33ccff");

    var expected = "#33ccff";
    var actual = grid.getColorAtLocation(0, 0);
    deepEqual(actual, expected, "Square set color 1");

    setGridSquareToColor("3", "C", "#33ccff");
    actual = grid.getColorAtLocation(2, 2);
    deepEqual(actual, expected, "Square set color 2");

    setGridSquareToColor("5", "B", "#33ccff");
    actual = grid.getColorAtLocation(4, 1)
    deepEqual(actual, expected, "Square set color 3");

    setGridSquareToColor("2", "D", "#33ccff");
    actual = grid.getColorAtLocation(1, 3);
    deepEqual(actual, expected, "Square set color 4");

    // now test overwriting
    setGridSquareToColor("3", "C", "#AAAAAA");
    expected = "#AAAAAA";
    actual = grid.getColorAtLocation(2, 2);
    deepEqual(actual, expected, "Square override color 2");

    grid.resetGrid();
});

test("Grid CircleBlock painter tester", function () {
    // grid initialization already happened since it is a global in index.js
    // make sure to reset grid before and after testing so other tests work!
    grid.resetGrid();
    setGridCircleToColor("8", "H", 1, "#33ccff");

    var expected = "#33ccff";
    var actual = grid.getColorAtLocation(6, 7);
    deepEqual(actual, expected, "Circle 1 set color N");
    actual = grid.getColorAtLocation(8, 7);
    deepEqual(actual, expected, "Circle 1 set color S");
    actual = grid.getColorAtLocation(7, 6);
    deepEqual(actual, expected, "Circle 1 set color W");
    actual = grid.getColorAtLocation(7, 8);
    deepEqual(actual, expected, "Circle 1 set color E");

    // now test overwriting
    setGridCircleToColor("7", "G", 1, "#AAAAAA");
    expected = "#AAAAAA";
    actual = grid.getColorAtLocation(6, 7);
    deepEqual(actual, expected, "Circle 1 override color N");

    grid.resetGrid();

    setGridCircleToColor("8", "H", 4, "#33ccff");

    var expected = "#33ccff";
    var actual = grid.getColorAtLocation(3, 7);
    deepEqual(actual, expected, "Circle 2 set color N");
    actual = grid.getColorAtLocation(11, 7);
    deepEqual(actual, expected, "Circle 2 set color S");
    actual = grid.getColorAtLocation(7, 3);
    deepEqual(actual, expected, "Circle 2 set color W");
    actual = grid.getColorAtLocation(7, 11);
    deepEqual(actual, expected, "Circle 2 set color E");

    // now test overwriting
    setGridCircleToColor("7", "J", 4, "#AAAAAA");
    expected = "#AAAAAA";
    actual = grid.getColorAtLocation(3, 7);
    deepEqual(actual, expected, "Circle 2 override color N");

    grid.resetGrid();
});

module("Getting rows/columns x blocks away");
test("moveRowNumberUp(initialRowNum, amountToMove)", function () {
    // returns new row number
    var actual = moveRowNumberUp(5, 2);
    var expected = 3;
    deepEqual(actual, expected, "Moving a row number up without wrapping");

    actual = moveRowNumberUp(3, 5);
    expected = MAX_ROW - 2;
    deepEqual(actual, expected, "Moving a row number up with wrapping");

});

test("moveRowNumberDown(initialRowNum, amountToMove)", function () {
    // returns new row number
    var actual = moveRowNumberDown(5, 2);
    var expected = 7;
    deepEqual(actual, expected, "Moving a row number down without wrapping");

    actual = moveRowNumberDown(13, 5);
    if (MAX_ROW < (13 + 5)) {
        expected = MIN_ROW + (13+5-MAX_ROW-1); // -1 as it isn't 0 based
    }
    else expected = 18;
    deepEqual(actual, expected, "Moving a row number down with wrapping");
});

test("moveColumnLetterRight(initialRowNum, amountToMove)", function () {
    // returns new row number
    var actual = moveColumnLetterRight("D", 2);
    var expected = "F";
    deepEqual(actual, expected, "Moving a column letter right without wrapping");

    actual = moveColumnLetterRight("N", 5);
    expected = "C";
    deepEqual(actual, expected, "Moving a column letter right with wrapping");
});

test("moveColumnLetterLeft(initialRowNum, amountToMove)", function () {
    // returns new row number
    var actual = moveColumnLetterLeft("D", 2);
    var expected = "B";
    deepEqual(actual, expected, "Moving a column letter left without wrapping");

    actual = moveColumnLetterLeft("A", 5);
    expected = "L";
    deepEqual(actual, expected, "Moving a column letter left with wrapping");
});

module("Test Block");
function setSetToColorForBlock(blockId, hexColorString) {
    $("#" + blockId).find("input.setToColor").val(hexColorString);
}

test("Grid SingleBlock", function () {
    grid.resetGrid();
    var singleBlockId = addBlock("SingleBlock", "single-template");
    var singleBlock = blockList.getBlock(singleBlockId);
    singleBlock.setAction("randomize");
    singleBlock.rowId = '1';
    singleBlock.columnId = 'A';
    //setSetToColorForBlock(singleBlockId, "#443322");
    singleBlock.performAction();
    notExpected = DEFAULT_BOX_COLOR;
    actual = grid.getColorAtLocation(0, 0);
    notDeepEqual(actual, notExpected, "Square A1 setColor check");

});