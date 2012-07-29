using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace mtazva.utilities.commandline
{
    /// <summary>
    /// This class will parse a collection of command line arguments into a dictionary of parameters and associated values
    /// </summary>
    /// <remarks>
    /// parsed parameters follow the pattern: /parameter1 value1 value2 value3 /parameter2 value1 value2
    /// </remarks>
    public class CommandLineArguments : Dictionary<string, List<string>>
    {
        #region Constructors

        public CommandLineArguments(IEnumerable<string> args)
        {
            if (args == null)
                return;

            string currentArg = string.Empty;
            foreach (string arg in args)
            {
                if (arg.StartsWith(@"/"))
                {
                    currentArg = arg.Substring(1).Trim().ToLowerInvariant();//strip the leading slash
                    AddArgumentKey(currentArg);
                }
                else
                {
                    if (currentArg == string.Empty)
                        AddArgumentKey(currentArg);

                    this[currentArg].Add(arg.Trim());
                }
            }
        }

        #endregion Constructors

        #region Public Methods

        /// <summary>
        /// Check whether argument was passed
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        /// <remarks>
        /// Arguments are defined on command line with a leading slash, as in: /argument
        /// When checking for an argument, do not include the slash in the name
        /// </remarks>
        public bool ArgumentPassed(string argument)
        {
            return this.ContainsKey(argument.ToLowerInvariant());
        }

        /// <summary>
        /// Get values passed in with an argument
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        public IEnumerable<string> GetArgumentValues(string argument)
        {
            if (!ArgumentPassed(argument))
                return new string[] { };

            return this[argument.ToLowerInvariant()].AsReadOnly();
        }

        /// <summary>
        /// Get strongly typed values passed in with an argument. Only values that can be converted will be returned in the collection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="argument"></param>
        /// <returns></returns>
        public IEnumerable<T> GetArgumentValues<T>(string argument)
        {
            var converter = TypeDescriptor.GetConverter(typeof(T));
            if (converter == null)
            {
                throw new System.NotSupportedException("Unable to access type converter for type " + typeof(T).ToString());
            }
            else if (!converter.CanConvertFrom(typeof(string)))
            {
                throw new System.NotSupportedException("Type " + typeof(T).ToString() + " cannot convert from string value");
            }
            else
            {
                return GetArgumentValues(argument).Where(a => converter.IsValid(a)).Select(a => (T)converter.ConvertFromString(a));
            }
        }

        #endregion Public Methods

        #region Private Methods

        private void AddArgumentKey(string arg)
        {
            if (!this.ContainsKey(arg))
                this.Add(arg, new List<string>());
        }

        #endregion Private Methods
    }
}