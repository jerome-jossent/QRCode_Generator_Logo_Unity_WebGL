﻿namespace QRCoder
{
    public static partial class PayloadGenerator
    {
        /// <summary>
        /// Generates a phone call payload.
        /// </summary>
        public class PhoneNumber : Payload
        {
            private readonly string number;

            /// <summary>
            /// Initializes a new instance of the <see cref="PhoneNumber"/> class.
            /// </summary>
            /// <param name="number">Phone number of the receiver.</param>
            public PhoneNumber(string number)
            {
                this.number = number;
            }

            /// <summary>
            /// Returns the phone call payload as a string.
            /// </summary>
            /// <returns>The phone call payload as a string.</returns>
            public override string ToString()
            {
                return $"tel:{this.number}";
            }
        }
    }
}
