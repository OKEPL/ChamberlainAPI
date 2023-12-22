namespace Chamberlain.AppServer.Api.Contracts.ValidationAttribute
{
    #region

    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;

    #endregion

    /// <inheritdoc />
    /// <summary>
    /// The is higher than attribute.
    /// </summary>
    public sealed class IsHigherThanAttribute : ValidationAttribute
    {
        /// <summary>
        /// The _default error message.
        /// </summary>
        private const string DefaultErrorMessage = "{0} is not HigherPropertyName than {1}";

        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Chamberlain.AppServer.Api.Contracts.ValidationAttribute.IsHigherThanAttribute" /> class.
        /// </summary>
        /// <param name="lowerPropertyName">
        /// The lower property name.
        /// </param>
        /// <param name="higherPropertyName">
        /// The higher Property Name.
        /// </param>
        /// <param name="allowEqual">
        /// The allow Equal.
        /// </param>
        /// <param name="type">
        /// The type.
        /// </param>
        public IsHigherThanAttribute(string higherPropertyName, string lowerPropertyName, bool allowEqual, Type type)
            : base(DefaultErrorMessage)
        {
            this.HigherPropertyNameValue = higherPropertyName;
            this.LowerPropertyNameProperty = lowerPropertyName;
            this.AllowEqual = allowEqual;
            this.Type = type;
        }

        /// <summary>
        /// Gets a value indicating whether allow equal.
        /// </summary>
        public bool AllowEqual { get; }

        /// <summary>
        /// Gets the higher property name value.
        /// </summary>
        public string HigherPropertyNameValue { get; }

        /// <summary>
        /// Gets the lower property name property.
        /// </summary>
        public string LowerPropertyNameProperty { get; }

        /// <summary>
        /// Gets the type.
        /// </summary>
        public Type Type { get; }

        /// <inheritdoc />
        /// <summary>
        /// Gets the type id.
        /// </summary>
        public override object TypeId { get; } = new object();

        /// <inheritdoc />
        /// <summary>
        /// The format error message.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <returns>
        /// The <see cref="T:System.String" />.
        /// </returns>
        public override string FormatErrorMessage(string name)
        {
            return string.Format(
                CultureInfo.CurrentUICulture,
                this.ErrorMessageString,
                this.LowerPropertyNameProperty,
                this.HigherPropertyNameValue,
                this.Type);
        }

        /// <inheritdoc />
        /// <summary>
        /// The is valid.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="T:System.Boolean" />.
        /// </returns>
        /// <exception cref="T:System.ArgumentException">
        /// </exception>
        public override bool IsValid(object value)
        {
            var properties = TypeDescriptor.GetProperties(value);
            var lower = Convert.ChangeType(
                            properties.Find(this.LowerPropertyNameProperty, true /* ignoreCase */).GetValue(value),
                            this.Type) as IComparable;
            if (!(Convert.ChangeType(
                      properties.Find(this.HigherPropertyNameValue, true /* ignoreCase */).GetValue(value),
                      this.Type) is IComparable higher))
            {
                throw new ArgumentException(
                    $"Coulnd't properly convert: {this.HigherPropertyNameValue}, to type: {this.Type}");
            }

            switch (higher.CompareTo(lower))
            {
                case -1:
                    return false;
                case 0:
                    if (this.AllowEqual)
                    {
                        return true;
                    }

                    return false;
                case 1:
                    return true;

                default:
                    throw new ArgumentException(
                        $"Couldn't compare {this.LowerPropertyNameProperty} to {this.HigherPropertyNameValue}");
            }
        }
    }
}