using System.IO;
using System.Xml;
using System.Xml.Schema;

namespace Validators
{

    /// <summary>
    /// XML Validation base class
    /// </summary>
    public class SimpleXmlValidator
    {

        #region Internal Members

        private XmlSchema schema;
        private XmlReaderSettings settings;

        #endregion

        #region Properties

        /// <summary>
        /// Schema XML
        /// </summary>
        public string SchemaXml
        {
            get
            {
                System.Text.StringBuilder output = new System.Text.StringBuilder();
                XmlWriter schemaWriter = XmlWriter.Create(output);

                schema.Write(schemaWriter);

                return output.ToString();
            }
            set
            {
                //create schema object
                this.schema = XmlSchema.Read(new StringReader(value), null);

                //create XML Reader settings based on schema
                settings = new XmlReaderSettings();
                settings.Schemas.Add(schema);
                settings.CloseInput = true;
                settings.ValidationType = ValidationType.Schema;
            }
        }

        #endregion

        #region Constructors

        public XmlValidator(string schemaXml)
        {
            this.SchemaXml = schemaXml;
        }

        #endregion

        #region IXmlValidator Members

        /// <summary>
        /// Validate XML against schema document
        /// </summary>
        /// <param name="xmlDoc"></param>
        /// <param name="schemaDoc"></param>
        /// <returns></returns>
        public virtual XmlValidationResult Validate(string xml)
        {
            //if no xml passed for validation, return failed validation result
            if (string.IsNullOrEmpty(xml))
                throw new ArgumentException("Invalid xml specified", "xml");

            //Validate XML data
            try
            {
                using (XmlReader validator = XmlReader.Create(new System.IO.StringReader(xml), settings))
                {
                    //read through XML to allow schema to validate
                    //  no explicit processing necessary, reader will throw exception for validation errors
                    while (validator.Read()) ;

                    validator.Close();
                }
            }
            catch (XmlSchemaValidationException schemaEx)
            {
                return new XmlValidationResult(false, schemaEx.Message);
            }

            //no validation exceptions raised - return success
            return new XmlValidationResult(true, null);
        }

        #endregion

    }

}
