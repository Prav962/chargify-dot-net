
#region License, Terms and Conditions
//
// ComponentAllocation.cs
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

using System;
using System.Xml;
using ChargifyNET.Json;
using System.Collections.Generic;

namespace ChargifyNET
{
    #region Imports



    #endregion

    /// <summary>
    /// Specific class when getting or setting information specfic to a components allocation history
    /// </summary>
    /// <remarks>See https://reference.chargify.com/v1/allocations/preview-allocations </remarks>
    public class ComponentAllocationPreview : ChargifyBase, IComponentAllocationPreview, IComparable<ComponentAllocationPreview>
    {
        #region Field Keys
       
        /// <summary>
        /// The XML key which represents a collection of ComponentAllocation's
        /// </summary>
        public static readonly string AllocationPreviewRootKey = "allocation_preview";
        private const string TransactionTypeKey = "transaction_type";
        private const string kindKey = "kind";
        private const string AmountinCentsKey = "amount_in_cents";
        private const string MemoKey = "memo";
        private const string DiscountAmountinCentsKey = "discount_amount_in_cents";
        private const string TaxableAmountinCentsKey = "taxable_amount_in_cents";
     
        private const string StartDatekey = "start_date";
        private const string EndDatekey = "end_date";
        private const string SubtotalInCentsKey = "subtotal_in_cents";
        private const string TotalDiscountInCentsKey = "total_discount_in_cents";
        private const string TotalTaxInCentsKey = "total_tax_in_cents";
        private const string TotalInCentsKey = "total_in_cents";
        private const string DirectionKey = "direction";
        private const string ProrationSchemeKey = "proration_scheme";
        private const string LineItemsKey = "line_items";
        
        #endregion

        #region Constructors
        /// <summary>
        /// Default Constructor
        /// </summary>
        public ComponentAllocationPreview()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="componentAllocationXml">The raw XML containing the component allocation node</param>
        public ComponentAllocationPreview(string componentAllocationXml)
        {
            // get the XML into an XML document
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(componentAllocationXml);
            if (doc.ChildNodes.Count == 0) throw new ArgumentException("XML not valid", nameof(componentAllocationXml));
            // loop through the child nodes of this node
            foreach (XmlNode elementNode in doc.ChildNodes)
            {
                if (elementNode.Name == AllocationPreviewRootKey)
                {
                    LoadFromNode(elementNode);
                    return;
                }
            }
            // if we get here, then no component info was found
            throw new ArgumentException("XML does not contain component allocation information", nameof(componentAllocationXml));
        }

        private void LoadFromJson(JsonObject obj)
        {
            // loop through the keys of this JsonObject to get component allocation info, and parse it out
            foreach (string key in obj.Keys)
            {
                switch (key)
                {

                    case StartDatekey:
                        _StartDate = obj.GetJSONContentAsDateTime(key);
                        break;
                    case EndDatekey:
                        _EndDate = obj.GetJSONContentAsDateTime(key);
                        break;
                    case SubtotalInCentsKey:
                        _subtotalInCents = obj.GetJSONContentAsInt(key);
                        break;
                    case TotalTaxInCentsKey:
                        _totalTaxInCents = obj.GetJSONContentAsInt(key);
                        break;
                    case TotalDiscountInCentsKey:
                        _totalDiscountInCents = obj.GetJSONContentAsInt(key);
                        break;
                    case TotalInCentsKey:
                        _totalInCents = obj.GetJSONContentAsInt(key);
                        break;
                    case DirectionKey:
                        _direction = obj.GetJSONContentAsString(key);
                        break;
                    case ProrationSchemeKey:
                        _prorationScheme = obj.GetJSONContentAsString(key);
                        break;
                    case LineItemsKey:
                        _lineItems = obj.GetJSONContentAsComponentLineItems(key);
                        break;
                }
            }
        }

        private void LoadFromNode(XmlNode obj)
        {
            // loop through the nodes to get component allocation info
            foreach (XmlNode dataNode in obj.ChildNodes)
            {
                switch (dataNode.Name)
                {
                    case StartDatekey:
                        _StartDate = dataNode.GetNodeContentAsDateTime();
                        break;
                    case EndDatekey:
                        _EndDate = dataNode.GetNodeContentAsDateTime();
                        break;
                    case SubtotalInCentsKey:
                        _subtotalInCents = dataNode.GetNodeContentAsInt();
                        break;
                    case TotalTaxInCentsKey:
                        _totalTaxInCents = dataNode.GetNodeContentAsInt();
                        break;
                    case TotalDiscountInCentsKey:
                        _totalDiscountInCents = dataNode.GetNodeContentAsInt();
                        break;
                    case TotalInCentsKey:
                        _totalInCents = dataNode.GetNodeContentAsInt();
                        break;
                    case DirectionKey:
                        _direction = dataNode.GetNodeContentAsString();
                        break;
                    case ProrationSchemeKey:
                        _prorationScheme = dataNode.GetNodeContentAsString();
                        break;
                    case LineItemsKey:
                        _lineItems = dataNode.GetNodeContentAsComponentLineItems();
                        break;
                }
            }
        }
        #endregion

        #region IComponentAllocation Members

        /// <summary>
        /// An array of <see cref="RenewalLineItem"/> representing the individual transactions that will be created at the next renewal
        /// </summary>
        public List<ComponentLineItem> LineItems
        {
            get
            {
                return _lineItems;
            }
        }
        private List<ComponentLineItem> _lineItems = new List<ComponentLineItem>();


        /// <summary>
        /// The time that the allocation was recorded, in ISO 8601 format and UTC timezone, i.e. 2012-11-20T22:00:37Z
        /// </summary>
        public DateTime StartDate { get { return _StartDate; } }
        private DateTime _StartDate = DateTime.MinValue;

        public DateTime EndDate { get { return _EndDate; } }
        private DateTime _EndDate = DateTime.MinValue;

        /// <summary>
        /// A decimal representing the amount of the total pre-tax, pre-discount charges that will be assessed at the next renewal
        /// </summary>
        public decimal Subtotal { get { return Convert.ToDecimal(SubtotalInCents) / 100; } }

        /// <summary>
        /// An integer representing the amount of the total pre-tax, pre-discount charges that will be assessed at the next renewal
        /// </summary>
        public int SubtotalInCents
        {
            get
            {
                return _subtotalInCents;
            }
        }
        private int _subtotalInCents = int.MinValue;

        /// <summary>
        /// A decimal representing the total amount owed, less any discounts, that will be assessed at the next renewal
        /// </summary>
        public decimal Total { get { return Convert.ToDecimal(TotalInCents) / 100; } }

        /// <summary>
        /// A decimal representing the amount of the coupon discounts that will be applied to the next renewal
        /// </summary>
        public decimal TotalDiscount { get { return Convert.ToDecimal(TotalDiscountInCents) / 100; } }

        /// <summary>
        /// An integer representing the amount of the coupon discounts that will be applied to the next renewal
        /// </summary>
        public int TotalDiscountInCents
        {
            get
            {
                return _totalDiscountInCents;
            }
        }
        private int _totalDiscountInCents = int.MinValue;

        /// <summary>
        /// An integer representing the total amount owed, less any discounts, that will be assessed at the next renewal
        /// </summary>
        public int TotalInCents
        {
            get
            {
                return _totalInCents;
            }
        }
        private int _totalInCents = int.MinValue;

        /// <summary>
        /// A decimal representing the total tax charges that will be assessed at the next renewal
        /// </summary>
        public decimal TotalTax { get { return Convert.ToDecimal(TotalTaxInCents) / 100; } }

        /// <summary>
        /// An integer representing the total tax charges that will be assessed at the next renewal
        /// </summary>
        public int TotalTaxInCents
        {
            get
            {
                return _totalTaxInCents;
            }
        }
        private int _totalTaxInCents = int.MinValue;


        public string Direction
        {
            get
            {
                return _direction;
            }
        }
        private string _direction;
        public string ProrationScheme
        {
            get
            {
                return _prorationScheme;
            }
        }
        private string _prorationScheme;


        /// <summary>
        /// The scheme used if the proration was an upgrade. This is only present when the allocation was created mid-period.
        /// </summary>
        public ComponentUpgradeProrationScheme UpgradeScheme { get; set; }

        /// <summary>
        /// The scheme used if the proration was a downgrade. This is only present when the allocation was created mid-period.
        /// </summary>
        public ComponentDowngradeProrationScheme DowngradeScheme { get; set; }

        #endregion

        #region Compare
        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(ComponentAllocationPreview other)
        {
            // TODO: Implement this method
            throw new NotImplementedException();
        }
        #endregion
    }
}