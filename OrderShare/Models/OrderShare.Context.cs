﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;

    [DbConfigurationType(typeof(MySql.Data.Entity.MySqlEFConfiguration))]
    public partial class orderEntities : DbContext
    {
        public orderEntities()
            : base("name=orderEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<closeorder> closeorders { get; set; }
        public virtual DbSet<openorder> openorders { get; set; }
        public virtual DbSet<pendingorder> pendingorders { get; set; }
        public virtual DbSet<account> accounts { get; set; }
    }
}
