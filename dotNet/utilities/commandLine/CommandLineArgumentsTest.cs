using System.Collections.Generic;
using AdSpace.Common.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace mtazva.utilities.commandline.tests
{
    #region Test Setup

    /// <summary>
    ///This is a test class for CommandLineArgumentsTest and is intended
    ///to contain all CommandLineArgumentsTest Unit Tests
    ///</summary>
    [TestClass()]
    public class CommandLineArgumentsTest
    {
        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes

        //
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //

        #endregion Additional test attributes

    #endregion Test Setup

        #region Constructor Tests

        [TestMethod]
        public void CommandLineArgumentsConstructor_NullParameter_EmptyDictionary()
        {
            IEnumerable<string> args = null;
            CommandLineArguments target = new CommandLineArguments(args);

            Assert.AreEqual(0, target.Count);
        }

        [TestMethod]
        public void CommandLineArgumentsConstructor_AllValuesWithNoArgumentName_ValuesIncludedInOneKey()
        {
            IEnumerable<string> args = @"0 1 2 3".Split(' ');
            CommandLineArguments target = new CommandLineArguments(args);

            Assert.AreEqual(1, target.Count);
        }

        [TestMethod]
        public void CommandLineArgumentsConstructor_AllArgumentsWithNoValues_OneKeyPerArgument()
        {
            IEnumerable<string> args = @"/a /b /c /d".Split(' ');
            CommandLineArguments target = new CommandLineArguments(args);

            Assert.AreEqual(4, target.Count);
        }

        [TestMethod]
        public void CommandLineArgumentsConstructor_ArgumentsWithValues_OneKeyPerArgument()
        {
            IEnumerable<string> args = @"/a /b 2 /c 3 4 /d 3 4 5 6".Split(' ');
            CommandLineArguments target = new CommandLineArguments(args);

            Assert.AreEqual(4, target.Count);
        }

        #endregion Constructor Tests

        #region ArgumentPassed Tests

        [TestMethod]
        public void ArgumentPassed_NoArguments_ReturnsFalse()
        {
            IEnumerable<string> args = null;
            CommandLineArguments target = new CommandLineArguments(args);

            string argument = "test";
            bool expected = false;
            bool actual = target.ArgumentPassed(argument);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ArgumentPassed_ArgumentNotPassed_ReturnsFalse()
        {
            IEnumerable<string> args = @"/a /b /c".Split(' ');
            CommandLineArguments target = new CommandLineArguments(args);

            string argument = "d";
            bool expected = false;
            bool actual = target.ArgumentPassed(argument);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ArgumentPassed_ArgumentPassed_ReturnsTrue()
        {
            IEnumerable<string> args = @"/a /b /c".Split(' ');
            CommandLineArguments target = new CommandLineArguments(args);

            string[] expectedArgs = { "a", "b", "c" };
            bool expected = true;
            bool actual;

            foreach (string arg in expectedArgs)
            {
                actual = target.ArgumentPassed(arg);
                Assert.AreEqual(expected, actual);
            }
        }

        [TestMethod]
        public void ArgumentPassed_ArgumentPassed_ReturnsFalseIfSlashIncluded()
        {
            IEnumerable<string> args = @"/a /b /c".Split(' ');
            CommandLineArguments target = new CommandLineArguments(args);

            string arg = @"/b"; //passed as argument, but when asking for it, slash should not be included
            bool expected = false;
            bool actual = target.ArgumentPassed(arg);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ArgumentPassed_ArgumentPassed_CaseInsensitive()
        {
            IEnumerable<string> args = @"/a /b /c".Split(' ');
            CommandLineArguments target = new CommandLineArguments(args);

            string[] expectedArgs = new string[] { "a", "A", "b", "B", "c", "C" };
            bool expected = true;
            bool actual;

            foreach (string arg in expectedArgs)
            {
                actual = target.ArgumentPassed(arg);
                Assert.AreEqual(expected, actual);
            }
        }

        #endregion ArgumentPassed Tests

        #region GetArgumentValues Tests

        [TestMethod]
        public void GetArgumentValues_NoArgumentsPassed_ReturnsEmptyList()
        {
            IEnumerable<string> args = null;
            CommandLineArguments target = new CommandLineArguments(args);

            string argument = "TEST";
            IEnumerable<string> actual = target.GetArgumentValues(argument);

            Assert.IsFalse(actual.Any());
        }

        [TestMethod]
        public void GetArgumentValues_ArgumentNotPassed_ReturnsEmptyList()
        {
            IEnumerable<string> args = @"/a 1 /b 2".Split(' ');
            CommandLineArguments target = new CommandLineArguments(args);

            string argument = "c"; //this arg not passed in
            IEnumerable<string> actual = target.GetArgumentValues(argument);

            Assert.IsFalse(actual.Any());
        }

        [TestMethod]
        public void GetArgumentValues_ArgumentPassedWithNoValues_EmptyList()
        {
            IEnumerable<string> args = "/ab /cd /ef".Split(' ');
            CommandLineArguments target = new CommandLineArguments(args);

            string argument = "cd"; //passed with no values
            IEnumerable<string> actual = target.GetArgumentValues(argument);

            Assert.IsFalse(actual.Any());
        }

        [TestMethod]
        public void GetArgumentValues_ArgumentPassedWithValues_ValuesReturned()
        {
            IEnumerable<string> args = "/ab /cd 1 2 /ef".Split(' ');
            CommandLineArguments target = new CommandLineArguments(args);

            string argument = "cd"; //passed with two values
            IEnumerable<string> actual = target.GetArgumentValues(argument);

            Assert.AreEqual(2, actual.Count());
        }

        [TestMethod]
        public void GetArgumentValues_ValuesPassedBeforeArgumentName_ParsedUnderEmptyStringArgumentName()
        {
            IEnumerable<string> args = "1 2 /a /b".Split(' ');
            CommandLineArguments target = new CommandLineArguments(args);

            string argument = ""; //passed with two values
            IEnumerable<string> actual = target.GetArgumentValues(argument);

            Assert.IsTrue(actual.Contains("1"));
            Assert.IsTrue(actual.Contains("2"));
            Assert.AreEqual(2, actual.Count());
        }

        #endregion GetArgumentValues Tests

        #region GetArgumentValues<T> Tests

        [TestMethod]
        public void GetArgumentValuesT_NoArgumentsPassed_NoValuesReturned()
        {
            IEnumerable<string> args = null;
            CommandLineArguments target = new CommandLineArguments(args);

            string argument = "TEST";
            int[] actual = target.GetArgumentValues<int>(argument).ToArray(); //force evaluation of conversion

            Assert.IsFalse(actual.Any());
        }

        [TestMethod]
        public void GetArgumentValuesT_ArgumentNotPassed_NoValuesReturned()
        {
            IEnumerable<string> args = @"/a 1 /b 2".Split(' ');
            CommandLineArguments target = new CommandLineArguments(args);

            string argument = "c"; //this arg not passed in
            int[] actual = target.GetArgumentValues<int>(argument).ToArray(); //force evaluation of conversion

            Assert.IsFalse(actual.Any());
        }

        [TestMethod]
        public void GetArgumentValuesT_ArgumentPassedWithNoValues_NoValuesReturned()
        {
            IEnumerable<string> args = "/ab /cd /ef".Split(' ');
            CommandLineArguments target = new CommandLineArguments(args);

            string argument = "cd"; //passed with no values
            int[] actual = target.GetArgumentValues<int>(argument).ToArray(); //force evaluation of conversion

            Assert.IsFalse(actual.Any());
        }

        [TestMethod]
        public void GetArgumentValuesT_ArgumentPassedWithValues_ValuesReturned()
        {
            IEnumerable<string> args = "/ab /cd 1 2 /ef".Split(' ');
            CommandLineArguments target = new CommandLineArguments(args);

            string argument = "cd"; //passed with two values
            int[] actual = target.GetArgumentValues<int>(argument).ToArray(); //force evaluation of conversion

            Assert.AreEqual(2, actual.Count());
        }

        [TestMethod]
        public void GetArgumentValuesT_ArgumentPassedWithInvalidTypeValues_NoValuesReturned()
        {
            IEnumerable<string> args = "/ab /cd XYZ /ef".Split(' ');
            CommandLineArguments target = new CommandLineArguments(args);

            string argument = "cd"; //passed with non-numeric value
            int[] actual = target.GetArgumentValues<int>(argument).ToArray(); //force evaluation of conversion

            Assert.IsFalse(actual.Any());
        }

        [TestMethod]
        public void GetArgumentValuesT_ArgumentPassedWithSomeInvalidTypeValues_ReturnsOnlyValidValuesFortype()
        {
            IEnumerable<string> args = "/ab /cd 1 Z 3 /ef".Split(' ');
            CommandLineArguments target = new CommandLineArguments(args);

            string argument = "cd"; //passed with two valid numeric values, one invalid non-numeric
            int[] actual = target.GetArgumentValues<int>(argument).ToArray(); //force evaluation of conversion

            Assert.AreEqual(2, actual.Count());
        }

        [TestMethod]
        [ExpectedException(typeof(System.NotSupportedException))]
        public void GetArgumentValuesT_ArgumentPassedWithNonParseableType_ThrowsException()
        {
            IEnumerable<string> args = "/ab /cd 1-2-2012 /ef".Split(' ');
            CommandLineArguments target = new CommandLineArguments(args);

            string argument = "cd"; //passed with date value, but NonStringParseable type cannot convert from string
            NonStringParseable[] actual = target.GetArgumentValues<NonStringParseable>(argument).ToArray(); //force evaluation of conversion

            Assert.Fail("Expected Exception not thrown");
        }

        /// <summary>
        /// Class that cannot be converted from string type
        /// </summary>
        private class NonStringParseable
        {
            public System.DateTime DateTimeProperty { get; set; }
        }

        #endregion GetArgumentValues<T> Tests
    }
}