
#region License, Terms and Conditions
//
// IComponentAllocation.cs
//
// Authors: Kori Francis <twitter.com/djbyter>, David Ball
// Copyright (C) 2013 Clinical Support Systems, Inc. All rights reserved.
// 
//  THIS FILE IS LICENSED UNDER THE MIT LICENSE AS OUTLINED IMMEDIATELY BELOW:
//
//  Permission is hereby granted, free of charge, to any person obtaining a
//  copy of this software and associated documentation files (the "Software"),
//  to deal in the Software without restriction, including without limitation
//  the rights to use, copy, modify, merge, publish, distribute, sublicense,
//  and/or sell copies of the Software, and to permit persons to whom the
//  Software is furnished to do so, subject to the following conditions:
//
//  The above copyright notice and this permission notice shall be included in
//  all copies or substantial portions of the Software.
//
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
//  FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
//  DEALINGS IN THE SOFTWARE.
//
#endregion

// ReSharper disable once CheckNamespace
namespace ChargifyNET
{
    using Json;
    using Newtonsoft.Json;
    #region Imports
    using System;
    using System.Collections.Generic;
    using System.Xml;
    using System.Xml.Serialization;
    #endregion

    /// <summary>
    /// The scheme used if the proration was an upgrade. This is only present when the allocation was created mid-period.
    /// </summary>
    /// <remarks>
    /// The API uses hyphens for word seperation, I use underscores and replace the hyphens with 
    /// underscores during parsing so I can parse the enumerated values
    /// </remarks>
    public enum ComponentUpgradeProrationScheme
    {
        /// <summary>
        /// A charge is added for the prorated amount due, but the card is not charged until the subscription’s next renewal
        /// </summary>
        [XmlEnum("prorate-delay-capture")]
        Prorate_Delay_Capture,
        /// <summary>
        /// A charge is added and we attempt to charge the credit card on file. If it fails, the charge will be accrued until the next renewal.
        /// </summary>
        [XmlEnum("prorate-attempt-capture")]
        Prorate_Attempt_Capture,
        /// <summary>
        /// No charge is added.
        /// </summary>
        [XmlEnum("no-prorate")]
        No_Prorate,
        /// <summary>
        /// A charge is added for the full price of the component change, and we attempt to charge the credit card on file. If it fails, the charge will be accrued until the next renewal.
        /// </summary>
        [XmlEnum("full-price-attempt-capture")]
        Full_Price_Attempt_Capture,
        /// <summary>
        /// A charge is added for the full price of the component change, but the card is not charged until the subscription’s next renewal.
        /// </summary>
        [XmlEnum("full-price-delay-capture")]
        Full_Price_Delay_Capture,
        /// <summary>
        /// No value (internal to this library)
        /// </summary>
        [XmlIgnore]
        Unknown
    }

    /// <summary>
    /// The scheme used if the proration was a downgrade. This is only present when the allocation was created mid-period.
    /// </summary>
    /// <remarks>
    /// The API uses hyphens for word seperation, I use underscores and replace the hyphens with 
    /// underscores during parsing so I can parse the enumerated values
    /// </remarks>
    public enum ComponentDowngradeProrationScheme
    {
        /// <summary>
        /// A credit is added for the amount owed.
        /// </summary>
        [XmlEnum("prorate")]
        Prorate,
        /// <summary>
        /// No credit is added
        /// </summary>
        [XmlEnum("no-prorate")]
        No_Prorate,
        /// <summary>
        /// No value (internal to this library)
        /// </summary>
        [XmlIgnore]
        Unknown
    }

    /// <summary>
    /// llocations describe a change to the allocated quantity for a particular Component (either Quantity-Based or On/Off) for a particular Subscription.
    /// </summary>
    public interface IComponentAllocation
    {
        /// <summary>
        /// The allocated quantity set in to effect by the allocation
        /// </summary>
        int Quantity { get; set; }
        /// <summary>
        /// The allocated quantity that was in effect before this allocation was created
        /// </summary>
        int PreviousQuantity { get; }
        /// <summary>
        /// The integer component ID for the allocation. This references a component that you have created in your Product setup
        /// </summary>
        int ComponentID { get; }
        /// <summary>
        /// The integer subscription ID for the allocation. This references a unique subscription in your Site
        /// </summary>
        int SubscriptionID { get; }
        /// <summary>
        /// The memo passed when the allocation was created
        /// </summary>
        string Memo { get; set; }
        /// <summary>
        /// The time that the allocation was recorded, in ISO 8601 format and UTC timezone, i.e. 2012-11-20T22:00:37Z
        /// </summary>
        DateTime TimeStamp { get; }
        /// <summary>
        /// The scheme used if the proration was an upgrade. This is only present when the allocation was created mid-period.
        /// </summary>
        ComponentUpgradeProrationScheme UpgradeScheme { get; set; }
        /// <summary>
        /// The scheme used if the proration was a downgrade. This is only present when the allocation was created mid-period.
        /// </summary>
        ComponentDowngradeProrationScheme DowngradeScheme { get; set; }
    }

    public interface IComponentAllocationPreview
    {
        /// <summary>
        /// The timestamp for the subscription’s next renewal
        /// </summary>
        [XmlElement("start_date"), JsonProperty("start_date")]
        DateTime StartDate { get; }

        /// <summary>
        /// The timestamp for the subscription’s next renewal
        /// </summary>
        [XmlElement("end_date"), JsonProperty("end_date")]
        DateTime EndDate { get; }

        /// <summary>
        /// An integer representing the amount of the total pre-tax, pre-discount charges that will be assessed at the next renewal
        /// </summary>
        [XmlElement("subtotal_in_cents"), JsonProperty("subtotal_in_cents")]
        int SubtotalInCents { get; }

        /// <summary>
        /// An decimal representing the amount of the total pre-tax, pre-discount charges that will be assessed at the next renewal
        /// </summary>
        decimal Subtotal { get; }

        /// <summary>
        /// An integer representing the total tax charges that will be assessed at the next renewal
        /// </summary>
        [XmlElement("total_tax_in_cents"), JsonProperty("total_tax_in_cents")]
        int TotalTaxInCents { get; }

        /// <summary>
        /// An decimal representing the total tax charges that will be assessed at the next renewal
        /// </summary>
        decimal TotalTax { get; }

        /// <summary>
        /// An integer representing the amount of the coupon discounts that will be applied to the next renewal
        /// </summary>
        [XmlElement("total_discount_in_cents"), JsonProperty("total_discount_in_cents")]
        int TotalDiscountInCents { get; }

        /// <summary>
        /// An decimal representing the amount of the coupon discounts that will be applied to the next renewal
        /// </summary>
        decimal TotalDiscount { get; }

        /// <summary>
        /// An integer representing the total amount owed, less any discounts, that will be assessed at the next renewal
        /// </summary>
        [XmlElement("total_in_cents"), JsonProperty("total_in_cents")]
        int TotalInCents { get; }

        /// <summary>
        /// An decimal representing the total amount owed, less any discounts, that will be assessed at the next renewal
        /// </summary>
        decimal Total { get; }

        [XmlElement("direction"), JsonProperty("direction")]
        string Direction { get;  }

        [XmlElement("proration_scheme"), JsonProperty("proration_scheme")]
        string ProrationScheme { get; }

        /// <summary>
        /// An array of <see cref="RenewalLineItem"/> representing the individual transactions that will be created at the next renewal
        /// </summary>
        [XmlArray("line_items")]
        [XmlArrayItem("line_item", typeof(ComponentLineItem))]
        [JsonProperty("line_items")]
        List<ComponentLineItem> LineItems { get; }
    }

    /// <summary>
    /// The line item included in a renewal preview response
    /// </summary>
    [XmlRoot("line_item")]
    public class ComponentLineItem
    {
        #region Constructors
        /// <summary>
        /// Default constructor
        /// </summary>
        public ComponentLineItem() { }

        /// <summary>
        /// Xml parsing constructor
        /// </summary>
        /// <param name="node"></param>
        public ComponentLineItem(XmlNode node)
        {
            // Deserialize
            var obj = node.ConvertNode<ComponentLineItem>();

            TransactionType = obj.TransactionType;
            Kind = obj.Kind;
            AmountInCents = obj.AmountInCents;
            Memo = obj.Memo;
            DiscountAmountInCents = obj.DiscountAmountInCents;
            TaxableAmountInCents = obj.TaxableAmountInCents;
            ComponentId = obj.ComponentId;
        }

        /// <summary>
        /// Json parsing constructor
        /// </summary>
        /// <param name="renewalLineItem"></param>
        public ComponentLineItem(JsonObject renewalLineItem)
        {
            // Deserialize
            var obj = JsonConvert.DeserializeObject<ComponentLineItem>(renewalLineItem.ToString());

            TransactionType = obj.TransactionType;
            Kind = obj.Kind;
            AmountInCents = obj.AmountInCents;
            Memo = obj.Memo;
            DiscountAmountInCents = obj.DiscountAmountInCents;
            TaxableAmountInCents = obj.TaxableAmountInCents;
            ComponentId = obj.ComponentId;
        }
        #endregion

        #region Properties
        /// <summary>
        /// The type of transaction
        /// </summary>
        [XmlElement("transaction_type"), JsonProperty("transaction_type")]
        public string TransactionType { get; set; }

        /// <summary>
        /// The kind of transaction
        /// </summary>
        [XmlElement("kind"), JsonProperty("kind")]
        public string Kind { get; set; }

        /// <summary>
        /// The amount of the transaction in cents
        /// </summary>
        [XmlElement("amount_in_cents"), JsonProperty("amount_in_cents")]
        public int AmountInCents { get; set; }

        /// <summary>
        /// The amount of the transaction in dollars and cents
        /// </summary>
        public decimal Amount { get { return Convert.ToDecimal(AmountInCents) / 100; } }

        /// <summary>
        /// The memo of the transaction
        /// </summary>
        [XmlElement("memo"), JsonProperty("memo")]
        public string Memo { get; set; }

        /// <summary>
        /// The discount amount in cents
        /// </summary>
        [XmlElement("discount_amount_in_cents"), JsonProperty("discount_amount_in_cents")]
        public int DiscountAmountInCents { get; set; }

        /// <summary>
        /// The discount amount
        /// </summary>
        public decimal DiscountAmount { get { return Convert.ToDecimal(DiscountAmountInCents) / 100; } }

        /// <summary>
        /// The taxable amount in cents
        /// </summary>
        [XmlElement("taxable_amount_in_cents"), JsonProperty("taxable_amount_in_cents")]
        public int TaxableAmountInCents { get; set; }

        /// <summary>
        /// The taxable amount
        /// </summary>
        public decimal TaxableAmount { get { return Convert.ToDecimal(TaxableAmountInCents) / 100; } }

        [XmlElement("component_id"), JsonProperty("component_id")]
        public int? ComponentId { get; set; }

        #endregion
    }
}