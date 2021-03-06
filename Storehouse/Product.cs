//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Storehouse
{
    using System;
    using System.Collections.Generic;
    
    public partial class Product
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Product()
        {
            this.Invoices = new HashSet<Invoice>();
        }
    
        public int id { get; set; }
        public string productCode { get; set; }
        public string name { get; set; }
        public Nullable<int> category_id { get; set; }
        public Nullable<int> manufacturer_id { get; set; }
        public Nullable<int> supplier_id { get; set; }
        public Nullable<int> expire_date_id { get; set; }
        public string description { get; set; }
        public Nullable<decimal> price { get; set; }
        public Nullable<int> in_stock { get; set; }
        public Nullable<int> temperature_id { get; set; }
    
        public virtual Category Category { get; set; }
        public virtual Expire Expire { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Invoice> Invoices { get; set; }
        public virtual Manufacturer Manufacturer { get; set; }
        public virtual Supplier Supplier { get; set; }
        public virtual Temperature Temperature { get; set; }
    }
}
