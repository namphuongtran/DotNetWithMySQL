//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace OrderShare.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class closeorder
    {
        public long ID { get; set; }
        public string Ticket { get; set; }
        public string Symbol { get; set; }
        public string Type { get; set; }
        public float Size { get; set; }
        public System.DateTime OpenTime { get; set; }
        public decimal OpenPrice { get; set; }
        public decimal SL { get; set; }
        public decimal TP { get; set; }
        public System.DateTime CloseTime { get; set; }
        public decimal ClosePrice { get; set; }
        public float Swap { get; set; }
        public float Profit { get; set; }
        public float PipsValue { get; set; }
        public string Comment { get; set; }
    }
}
