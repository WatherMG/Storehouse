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
    
    public partial class Invoice
    {
        public int id { get; set; }
        public string invoice_code { get; set; }
        public Nullable<System.DateTime> sale_date { get; set; }
        public Nullable<int> product_id { get; set; }
        public Nullable<int> quantity { get; set; }
        public Nullable<decimal> price { get; set; }
    
        public virtual Product Product { get; set; }
    }
}
